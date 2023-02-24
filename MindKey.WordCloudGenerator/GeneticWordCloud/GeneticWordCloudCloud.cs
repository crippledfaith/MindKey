using MindKey.WordCloudGenerator.Base;
using SkiaSharp;
using System.Collections.Concurrent;

namespace MindKey.WordCloudGenerator.GeneticWordCloud
{
    public class GeneticWordCloudCloud : WordCloudCloud
    {
        public int CloudNumber { get; }

        public int GenarationNumber { get; }

        public int Score => WordCloudWords.Count(q => q.IsFit.HasValue && q.IsFit.Value);

        public GeneticWordCloudCloud(int genarationNumber, int cloudNumber, Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap) : base(wordCount, canvasHeight, canvasWidth, bitmap)
        {
            GenarationNumber = genarationNumber;
            CloudNumber = cloudNumber;
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
        public async Task CrossOver(GeneticWordCloudCloud wordCloudCloud1, GeneticWordCloudCloud wordCloudCloud2)
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

        public void RemoveWordThatCannotFit()
        {
            var wordCloudChildrenList = WordCloudWords.ToList();
            WordCloudWords.Clear();
            wordCloudChildrenList.RemoveAll(q => q.GetTextMesurements(q.Paint, q.Text).Width> CanvasWidth);
            wordCloudChildrenList.ForEach(q => WordCloudWords.Add(q));
        }

        public override void Dispose()
        {
            WordCloudWords.ToList().ForEach(x => x.Dispose());
            WordCloudWords.Clear();
            Bitmap.Dispose();
        }



        public override object Copy()
        {
            var newObj = (GeneticWordCloudCloud)MemberwiseClone();
            newObj.WordCloudWords = new ConcurrentBag<WordCloudWord>(WordCloudWords.Select(q => q.Copy()).Cast<WordCloudWord>());
            return newObj;
        }

      
    }
}
