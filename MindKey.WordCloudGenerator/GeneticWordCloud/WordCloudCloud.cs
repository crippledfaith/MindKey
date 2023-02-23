using MindKey.WordCloudGenerator.Base;
using SkiaSharp;
using System.Collections.Concurrent;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace MindKey.WordCloudGenerator.GeneticWordCloud
{
    public class WordCloudCloud : IDisposable, ICopyable
    {
        public int CanvasHeight { get; private set; }
        public int CanvasWidth { get; private set; }
        public int GenarationNumber { get; }
        public int CloudNumber { get; }
        [JsonIgnore]
        public SKBitmap Bitmap { get; }
        public Dictionary<string, int> WordCount { get; }
        public int Score => WordCloudWords.Count(q => q.IsFit.HasValue && q.IsFit.Value);
        public ConcurrentBag<WordCloudWord> WordCloudWords { get; private set; }

        public WordCloudCloud(int genarationNumber, int cloudNumber, Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap)
        {
            WordCount = new Dictionary<string, int>(wordCount);
            GenarationNumber = genarationNumber;
            CloudNumber = cloudNumber;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
            Bitmap = bitmap.Copy();
            WordCloudWords = new ConcurrentBag<WordCloudWord>();
        }
        public async Task Generate()
        {
            var random = new Random();
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
        }
        public async Task RandomizeUnFitWords()
        {
            var random = new Random();
            await Task.Run(() =>
            {
                foreach (var word in WordCloudWords.Where(q => q.IsFit.HasValue && !q.IsFit.Value))
                {
                    var x = (float)random.Next(0, CanvasWidth - 1);
                    var y = (float)random.Next(0, CanvasHeight - 1);
                    word.X = x;
                    word.Y = y;
                }
            });
        }
        public async Task CrossOver(WordCloudCloud wordCloudCloud1, WordCloudCloud wordCloudCloud2)
        {
            await Task.Run(() =>
              {
                  wordCloudCloud1.WordCloudWords.ToList().ForEach(word => WordCloudWords.Add((WordCloudWord)word.Copy()));
                  var random = new Random();
                  var rIndex1 = random.Next(0, WordCloudWords.Count);
                  var rIndex2 = random.Next(0, WordCloudWords.Count);

                  for (var i = Math.Min(rIndex1, rIndex2); i <= Math.Max(rIndex1, rIndex2); i++)
                  {
                      var wordToReplace = WordCloudWords.ToList()[i];
                      var wordToReplaceWith = wordCloudCloud2.WordCloudWords.First(word => word.Text == wordToReplace.Text);
                      wordToReplace.X = wordToReplaceWith.X;
                      wordToReplace.Y = wordToReplaceWith.Y;
                  }
              });
        }
        public async Task Mutate()
        {
            await Task.Run(() =>
            {
                var random = new Random();
                var rIndex1 = random.Next(0, WordCloudWords.Count);
                var rIndex2 = random.Next(0, WordCloudWords.Count);

                for (var i = Math.Min(rIndex1, rIndex2); i <= Math.Max(rIndex1, rIndex2); i++)
                {
                    if (random.Next(0, 10) > 3)
                    {
                        var wordToMutate = WordCloudWords.ToList()[i];
                        wordToMutate.X = random.Next(0, CanvasWidth - 1);
                        wordToMutate.Y = random.Next(0, CanvasHeight - 1);
                    }
                }
            });
        }
        public async Task Compute()
        {
            await Task.Run(() =>
            {
                using var bitmap = Bitmap.Copy();
                using SKCanvas canvas = new SKCanvas(bitmap);

                var wordCloudChildrenList = WordCloudWords.ToList();
                wordCloudChildrenList.ForEach(q => q.Initilize());
                foreach (var wordCloudChild in wordCloudChildrenList)
                {
                    if (wordCloudChild.DoesFit(bitmap))
                        canvas.DrawText(wordCloudChild.Text, wordCloudChild.DrawX, wordCloudChild.DrawY, wordCloudChild.Font, wordCloudChild.Paint);
                }
            });
        }

        public void Dispose()
        {
            WordCloudWords.ToList().ForEach(x => x.Dispose());
            WordCloudWords.Clear();
            Bitmap.Dispose();
        }
        private async Task RandomizeWordCount()
        {
            await Task.Run(() =>
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


        public object Copy()
        {
            var newObj = (WordCloudCloud)MemberwiseClone();
            newObj.WordCloudWords = new ConcurrentBag<WordCloudWord>(WordCloudWords.Select(q => q.Copy()).Cast<WordCloudWord>());
            return newObj;
        }


    }
}
