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
        public Grid(int row, int columb)
        {
            Row = row;
            Columb = columb;
        }
    }
    public class GridPlacementService : AGeneratorService
    {

        public GridPlacementService(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap) : base(wordCount, canvasHeight, canvasWidth, bitmap)
        {
        }


        public async Task<WordCloudCloud> Start()
        {
            var grids = CalculateGridSize(WordCount, CanvasHeight, CanvasWidth);
            var gridPlacementWordCloud = new GridPlacementWordCloudCloud(WordCount, CanvasHeight, CanvasWidth, Bitmap);
            await gridPlacementWordCloud.Generate();
            return gridPlacementWordCloud;
        }

        private List<Grid> CalculateGridSize(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth)
        {
            List<Grid> grids = new List<Grid>();
            

            return grids;
        }
    }
}