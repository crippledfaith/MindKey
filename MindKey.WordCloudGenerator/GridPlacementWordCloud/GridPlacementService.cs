using MindKey.WordCloudGenerator.Base;
using MindKey.WordCloudGenerator.GeneticWordCloud;
using SkiaSharp;

namespace MindKey.WordCloudGenerator.GridPlacementWordCloud
{
    public struct Grid
    {
        public int Row { get; set; }
        public int Columb { get; set; }
        public WordCloudWord? Word { get; set; }
        public bool IsOccupited => Word != null;
        public double Width { get; set; }
        public double Height { get; set; }

        public Grid(int row, int columb)
        {
            Row = row;
            Columb = columb;
        }

        public Grid(int row, int columb, double height, double width) : this(row, columb)
        {
            Height = height;
            Width = width;
        }
    }
    public class GridPlacementService : AGeneratorService
    {
        public GridPlacementService(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap) : base(wordCount, canvasHeight, canvasWidth, bitmap)
        {
        }

        public async Task<WordCloudCloud> Start()
        {
            var gridPlacementWordCloud = new GridPlacementWordCloudCloud(WordCount, CanvasHeight, CanvasWidth, Bitmap);
            await gridPlacementWordCloud.Generate();
            await gridPlacementWordCloud.ComputeScore();
            return gridPlacementWordCloud;
        }
    }
}