using IronSoftware.Drawing;
using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace MindKey.WordCloudGenerator
{
    public class GeneticWordCloudGenerator : AWordCloudGenerator
    {
        public GeneticWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
        }

        public override event EventHandler<WorkCloudResult> OnProgress;

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }
        protected async override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            using SKBitmap bitmap = SKBitmap.Decode(MaskFilePath);
            using SKCanvas canvas = new SKCanvas(bitmap);

            var canvasHeight = parameter.Height;
            var canvasWidth = parameter.Width;
         
            var service = new GeneticService(wordCount, canvasHeight, canvasWidth, bitmap);
            service.OnProgress += ServiceOnProgress;
            var wordCloudCloud = await service.Start();
            foreach (var word in wordCloudCloud.WordCloudWords)
            {
                canvas.DrawText(word.Text, word.DrawX, word.DrawY, word.Font, word.Paint);
            }
            using SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
            var base64String = Convert.ToBase64String(data.ToArray());
            WordCloudResult.Image = base64String;
            Directory.Delete(OutputPath, true);
            return WordCloudResult;
        }

        private void ServiceOnProgress(object? sender, WordCloudCloud e)
        {
            using SKBitmap bitmap = SKBitmap.Decode(MaskFilePath);
            using SKCanvas canvas = new SKCanvas(bitmap);

            foreach (var word in e.WordCloudWords)
            {
                canvas.DrawText(word.Text, word.DrawX, word.DrawY, word.Font, word.Paint);
            }
            using SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
            var base64String = Convert.ToBase64String(data.ToArray());
            WordCloudResult.Image = base64String;
            OnProgress.Invoke(sender, WordCloudResult);
        }
    }
    public class GeneticService
    {
        public event EventHandler<WordCloudCloud> OnProgress;
        public int GenerationCount { get; set; }
        public Dictionary<string, int> WordCount { get; }
        public int CanvasHeight { get; }
        public int CanvasWidth { get; }
        public SKBitmap Bitmap { get; }

        public WordCloudGeneration? CurrentGeneration { get; set; }

        public GeneticService(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap)
        {
            WordCount = wordCount;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
            Bitmap = bitmap;
        }
        public async Task<WordCloudCloud> Start()
        {
            var intialGeneration = new WordCloudGeneration(GenerationCount++, WordCount, CanvasHeight, CanvasWidth, Bitmap);
            await intialGeneration.GeneratePopulation(100);
            await intialGeneration.ComputeScore();
            WordCloudCloud bestCloud = intialGeneration.GetBestCloud().Copy();
            WordCloudCloud secondBestCloud = intialGeneration.GetSecondBestCloud();
            OnProgress(this, bestCloud);
            intialGeneration.Dispose();
            do
            {
                var newGeneration = new WordCloudGeneration(GenerationCount++, WordCount, CanvasHeight, CanvasWidth, Bitmap);
                await newGeneration.GeneratePopulation(bestCloud, secondBestCloud);
                await newGeneration.ComputeScore();
                var newBestCloud = newGeneration.GetBestCloud().Copy();
                if (newBestCloud.Score > bestCloud.Score)
                {
                    secondBestCloud = bestCloud;
                    bestCloud = newBestCloud;
                }
                else if (newBestCloud.Score > secondBestCloud.Score)
                {
                    secondBestCloud = newBestCloud;
                }
                OnProgress(this, bestCloud);
                newGeneration.Dispose();

            } while (bestCloud.Score < WordCount.Count() || GenerationCount < 10);

            return bestCloud;
        }


    }

    public class WordCloudGeneration : IDisposable
    {
        public int GenerationNumber { get; set; }
        public int CanvasHeight { get; private set; }
        public int CanvasWidth { get; private set; }
        public SKBitmap Bitmap { get; }
        public Dictionary<string, int> WordCount { get; }
        public ConcurrentBag<WordCloudCloud> WordCloudClouds { get; private set; }


        public WordCloudGeneration(int generationNumber, Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap)
        {
            GenerationNumber = generationNumber;
            WordCount = wordCount;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
            Bitmap = bitmap;
            WordCloudClouds = new ConcurrentBag<WordCloudCloud>();
        }


        public async Task GeneratePopulation(int limit = 10)
        {
            await Task.Factory.StartNew(async () =>
            {
                Parallel.For(0, limit, (x) =>
                {
                    var cloud = new WordCloudCloud(x, WordCount, CanvasHeight, CanvasWidth, Bitmap);
                    WordCloudClouds.Add(cloud);
                });
                foreach (var cloud in WordCloudClouds)
                {
                    await cloud.Generate();
                }
            });
        }
        public async Task GeneratePopulation(WordCloudCloud wordCloudCloud1, WordCloudCloud wordCloudCloud2, int limit = 10)
        {
            await Task.Factory.StartNew(async () =>
            {
                Parallel.For(0, limit, (x) =>
                {
                    var cloud = new WordCloudCloud(x, WordCount, CanvasHeight, CanvasWidth, Bitmap);
                    WordCloudClouds.Add(cloud);
                });
                foreach (var cloud in WordCloudClouds)
                {
                    await cloud.CrossOver(wordCloudCloud1, wordCloudCloud2);
                    await cloud.Mutate();
                }
            });
        }
        public async Task ComputeScore()
        {
            foreach (var cloud in WordCloudClouds)
            {
                await cloud.Compute();
            }
        }
        public void Dispose()
        {
            WordCloudClouds.ToList().ForEach(cloud => cloud.Dispose());
            WordCloudClouds.Clear();
        }

        public WordCloudCloud GetBestCloud()
        {
            return WordCloudClouds.ToList().OrderByDescending(q => q.Score).Take(1).First();
        }

        public WordCloudCloud GetSecondBestCloud()
        {
            return WordCloudClouds.ToList().OrderByDescending(q => q.Score).Skip(1).Take(1).First();
        }
    }

    public class WordCloudCloud : IDisposable
    {
        public int CanvasHeight { get; private set; }
        public int CanvasWidth { get; private set; }
        public int CloudNumber { get; }
        [JsonIgnore]
        public SKBitmap Bitmap { get; }
        public Dictionary<string, int> WordCount { get; }
        public int Score => WordCloudWords.Count(q => q.IsFit.HasValue && q.IsFit.Value);
        public ConcurrentBag<WordCloudWord> WordCloudWords { get; private set; }

        public WordCloudCloud(int cloudNumber, Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap)
        {
            WordCount = new Dictionary<string, int>(wordCount);
            CloudNumber = cloudNumber;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
            Bitmap = bitmap.Copy();
            WordCloudWords = new ConcurrentBag<WordCloudWord>();
        }
        public async Task Generate()
        {
            var random = new Random();
            await Task.Factory.StartNew(async () =>
            {
                await RandomizeWordCount();
                var result = Parallel.ForEach(WordCount, wordKV =>
                {
                    var text = wordKV.Key;
                    var fontSize = wordKV.Value + 10;
                    var x = (float)random.Next(0, CanvasWidth - 1);
                    var y = (float)random.Next(0, CanvasHeight - 1);
                    var font = new SKFont(SKTypeface.Default, fontSize, 1, 0);
                    var child = new WordCloudWord(x, y, wordKV.Key, wordKV.Value, font, CanvasHeight, CanvasWidth);
                    WordCloudWords.Add(child);
                });
            });
        }
        public async Task CrossOver(WordCloudCloud wordCloudCloud1, WordCloudCloud wordCloudCloud2)
        {
            await Task.Factory.StartNew(() =>
            {
                wordCloudCloud1.WordCloudWords.ToList().ForEach(word => WordCloudWords.Add(word.Copy()));
                var random = new Random();
                var rIndex1 = random.Next(0, WordCloudWords.Count);
                var rIndex2 = random.Next(0, WordCloudWords.Count);

                for (var i = Math.Min(rIndex1, rIndex2); i <= Math.Max(rIndex1, rIndex2); i++)
                {
                    var wordToReplace = WordCloudWords.ToList()[i];
                    var wordToReplaceWith = wordCloudCloud2.WordCloudWords.ToList()[i];
                    wordToReplace = wordToReplaceWith.Copy();
                }
            });
        }
        public async Task Mutate()
        {
            await Task.Factory.StartNew(() =>
            {
                var random = new Random();
                var rIndex1 = random.Next(0, WordCloudWords.Count);
                var rIndex2 = random.Next(0, WordCloudWords.Count);

                for (var i = Math.Min(rIndex1, rIndex2); i <= Math.Max(rIndex1, rIndex2); i++)
                {
                    if (random.Next(0, 10) > 8)
                    {
                        var wordToMutate = WordCloudWords.ToList()[i];
                        wordToMutate.X = (float)random.Next(0, CanvasWidth - 1);
                        wordToMutate.Y = (float)random.Next(0, CanvasHeight - 1);
                    }
                }
            });
        }
        public async Task Compute()
        {
            await Task.Factory.StartNew(() =>
            {
                var wordCloudChildrenList = WordCloudWords.ToList();
                wordCloudChildrenList.ForEach(q => q.Initilize());
                wordCloudChildrenList.ForEach(async (wordCloudChild) =>
                {
                    _ = await wordCloudChild.DoesFit(Bitmap);
                });
            });
        }

        public void Dispose()
        {
            WordCloudWords.ToList().ForEach(x => x.Dispose());
            WordCloudWords.Clear();
        }
        private async Task RandomizeWordCount()
        {
            await Task.Factory.StartNew(() =>
            {
                var copyWordCount = new Dictionary<string, int>(WordCount);
                Random rnd = new Random();
                WordCount.Clear();
                List<string> keys = new List<string>(copyWordCount.Keys);
                foreach (var word in copyWordCount)
                {
                    string randomKey = keys[rnd.Next(0, keys.Count)];
                    WordCount.Add(randomKey, copyWordCount[randomKey]);
                    keys.Remove(randomKey);
                }
            });
        }

        internal WordCloudCloud Copy() => JsonConvert.DeserializeObject<WordCloudCloud>(JsonConvert.SerializeObject(this));
    }

    public class WordCloudWord : IDisposable
    {
        [JsonIgnore]
        private SKRect? _textSize;
        
        private float x;
        private float y;

        [JsonIgnore]
        public bool? IsFit { get; set; }
        public float X
        {
            get => x;
            set
            {
                x = value;
                IsFit = null;
            }
        }
        public float Y
        {
            get => y;
            set
            {
                y = value;
                IsFit = null;
            }
        }
        public float DrawX => _textSize.HasValue ? X + _textSize.Value.Width / 2 : 0f;
        public float DrawY => _textSize.HasValue ? Y + _textSize.Value.Height : 0f;
        public string? Text { get; private set; }
        public int Frequncy { get; private set; }
        public SKFont? Font { get; private set; }
        public SKPaint? Paint { get; private set; }
        public int CanvasHeight { get; set; }
        public int CanvasWidth { get; set; }
        public WordCloudWord(float x, float y, string? text, int frequncy, SKFont font, int canvasHeight, int canvasWidth)
        {
            X = x;
            Y = y;
            Text = text;
            Frequncy = frequncy;
            Font = font;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
        }
        public void Initilize()
        {
            if (Font == null)
                throw new Exception("Font can't be null");
            Paint = GetPaint(Font.Size);
        }

        public async Task<bool> DoesFit(SKBitmap bitmap)
        {
            if (!IsFit.HasValue)
            {
                if (Paint == null || Text == null)
                    throw new Exception("Paint or Text can't be null");
                using var copyBitmap = bitmap.Copy();
                IsFit = await CheckCollisionInRender(bitmap, X, Y, GetTextMesurements(Paint, Text), CanvasHeight, CanvasWidth);
            }
            return IsFit.Value;
        }

        public WordCloudWord Copy() => JsonConvert.DeserializeObject<WordCloudWord>(JsonConvert.SerializeObject(this));

        public void Dispose()
        {
            Paint?.Dispose();
            Font?.Dispose();
        }

        private async Task<bool> CheckCollisionInRender(SKBitmap bitmap, float x, float y, SKRect bounds, int canvasHeight, int canvasWidth)
        {
            return await Task.Factory.StartNew(() =>
            {
                var endX = x + bounds.Width;
                var endY = y + bounds.Height;
                if (endX > canvasWidth || endY > canvasHeight)
                    return true;
                for (var xCheck = x; xCheck < endX; xCheck++)
                {
                    for (var yCheck = y; yCheck < endY; yCheck++)
                    {
                        var color = bitmap.GetPixel((int)xCheck, (int)yCheck);
                        if (!CheckIfBlack(color))
                        {
                            return true;
                        }
                    }

                }
                return false;
            });
        }
        private SKPaint GetPaint(float fontSize)
        {
            var paint = new SKPaint();
            paint.TextSize = fontSize;
            paint.IsAntialias = true;
            paint.Color = SKColors.White;
            paint.IsStroke = false;
            paint.StrokeWidth = 1;
            paint.TextAlign = SKTextAlign.Center;
            return paint;
        }
        private bool CheckIfBlack(SKColor color)
        {
            return color.Alpha == 255 && color.Red == 0 && color.Blue == 0 && color.Green == 0 && color.Hue == 0;
        }

        private SKRect GetTextMesurements(SKPaint paint, string text)
        {
            if (_textSize == null)
            {
                var bounds = new SKRect();
                paint.MeasureText(text, ref bounds);
                _textSize = bounds;
            }
            return _textSize.Value;
        }

    }
}
