using MindKey.WordCloudGenerator.Base;
using MindKey.WordCloudGenerator.GeneticWordCloud;
using SkiaSharp;

namespace MindKey.WordCloudGenerator.GridPlacementWordCloud
{
    public class GridPlacementService:AGeneratorService
    {
  
        public GridPlacementService(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap) : base(wordCount, canvasHeight, canvasWidth, bitmap)
        {
        }


        public async Task<WordCloudCloud> Start()
        {
            throw new NotImplementedException();
        }
    }
}