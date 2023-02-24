using MindKey.WordCloudGenerator.Base;
using MindKey.WordCloudGenerator.GeneticWordCloud;
using SkiaSharp;
using System.Collections.Concurrent;

namespace MindKey.WordCloudGenerator.GridPlacementWordCloud
{
    public class GridPlacementWordCloudCloud : WordCloudCloud
    {
        public GridPlacementWordCloudCloud(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap) : base(wordCount, canvasHeight, canvasWidth, bitmap)
        {
        }

        public async Task Generate()
        {
            var random = new Random();
            await RandomizeWordCount();
            var result = Parallel.ForEach(WordCount, wordKV =>
            {
                var text = wordKV.Key;
                var fontSize = wordKV.Value + 10;
                var x = 0;
                var y = 0;
                var font = new SKFont(SKTypeface.Default, fontSize, 1, 0);
                var child = new WordCloudWord(x, y, wordKV.Key, wordKV.Value, font, CanvasHeight, CanvasWidth);
                WordCloudWords.Add(child);
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

        public override object Copy()
        {
            var newObj = (GridPlacementWordCloudCloud)MemberwiseClone();
            newObj.WordCloudWords = new ConcurrentBag<WordCloudWord>(WordCloudWords.Select(q => q.Copy()).Cast<WordCloudWord>());
            return newObj;
        }

        public override void Dispose()
        {
            WordCloudWords.ToList().ForEach(x => x.Dispose());
            WordCloudWords.Clear();
            Bitmap.Dispose();
        }

       
    }
}