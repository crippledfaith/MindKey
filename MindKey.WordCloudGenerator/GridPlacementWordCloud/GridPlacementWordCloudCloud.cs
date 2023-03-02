using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MindKey.WordCloudGenerator.Base;
using MindKey.WordCloudGenerator.GeneticWordCloud;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System.Collections.Concurrent;

namespace MindKey.WordCloudGenerator.GridPlacementWordCloud
{
    public class GridPlacementWordCloudCloud : WordCloudCloud
    {
        public List<Grid>? Grids { get; private set; }

        public GridPlacementWordCloudCloud(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap) : base(wordCount, canvasHeight, canvasWidth, bitmap)
        {
        }
        private List<Grid> GetGrids(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth)
        {
            List<Grid> grids = new List<Grid>();
            var totalArea = canvasHeight * canvasWidth;
            var widthPerItems = canvasWidth / Math.Sqrt(wordCount.Count);
            var heightPerItems = canvasHeight / Math.Sqrt(wordCount.Count);
            var numberOfRows = Math.Ceiling(canvasHeight / heightPerItems);
            var numberOfColumns = Math.Ceiling(canvasWidth / widthPerItems);
            var n = 0;
            for (int row = 1; row <= numberOfRows; row++)
            {
                for (int col = 1; col <= numberOfColumns; col++)
                {
                    grids.Add(new Grid(row, col, heightPerItems, widthPerItems));
                }
            }
            return grids;
        }

        public async Task Generate()
        {
            var random = new Random();
            await RandomizeWordCount();
            Grids = GetGrids(WordCount, CanvasHeight, CanvasWidth);
            var ratio = GetHigestFontSize(Grids[0], WordCount);
            var result = Parallel.ForEach(Grids, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, grid =>
            {
                var n = (grid.Row * grid.Columb) - 1;
                if (WordCount.Count > n)
                {
                    KeyValuePair<string, int> keyValuePair = WordCount.ElementAt(n);
                    var key = keyValuePair.Key;
                    var value = keyValuePair.Value;
                    var fontSize = (int)((value + 10) * ratio);
                    var font = new SKFont(SKTypeface.Default, fontSize);
                    var x = Convert.ToInt64((grid.Columb - 1) * grid.Width);
                    var y = Convert.ToInt64((grid.Row - 1) * grid.Height);
                    var word = new WordCloudWord(x, y, key, value, font, CanvasHeight, CanvasWidth);
                    grid.Word = word;
                    WordCloudWords.Add(word);
                }
            });
        }

        private double GetHigestFontSize(Grid grid, Dictionary<string, int> wordCount)
        {
            var maxFontSize = wordCount.Max(q => q.Value);
            var longestWordCount = wordCount.Where(q => q.Value == maxFontSize).Max(q => q.Key.Count());
            var bigestWord = wordCount.FirstOrDefault(q => q.Value == maxFontSize && q.Key.Count() == longestWordCount);
            var fontSize = bigestWord.Value + 10;
            var font = new SKFont(SKTypeface.Default, fontSize);
            var word = new WordCloudWord(0, 0, bigestWord.Key, bigestWord.Value, font, CanvasHeight, CanvasWidth);
            word.Initilize();
            var textSize = word.GetTextMesurements(word.Paint, word.Text);
            var totalWidth = textSize.Width;
            var gap =  totalWidth- grid.Width;
            var ratio = 1d;
            if (Math.Abs(gap) > 5)
            {
                var estimatedCharSize = textSize.Width / longestWordCount;
                var newEstimateCharSize = (Math.Abs(gap) / longestWordCount);
                ratio = newEstimateCharSize / estimatedCharSize;
            }
            return ratio == 0 ? 1 : ratio;
        }

        public async Task ComputeScore()
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