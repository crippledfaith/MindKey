using MindKey.WordCloudGenerator.Base;
using SkiaSharp;

namespace MindKey.WordCloudGenerator.GeneticWordCloud
{
    public class GeneticService : AGeneratorService
    {
        public int GenerationCount { get; set; }
        public GeneticWordCloudGeneration? CurrentGeneration { get; set; }

        public GeneticService(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap) : base(wordCount, canvasHeight, canvasWidth, bitmap)
        {
        }

        public async Task<GeneticWordCloudCloud> Start()
        {
            var intialGeneration = new GeneticWordCloudGeneration(GenerationCount++, WordCount, CanvasHeight, CanvasWidth, Bitmap);
            await intialGeneration.GeneratePopulation(100);
            await intialGeneration.ComputeScore();
            await intialGeneration.RemoveWordThatCannotFit();
            GeneticWordCloudCloud bestCloud = (GeneticWordCloudCloud)intialGeneration.GetBestCloud().Copy();
            GeneticWordCloudCloud secondBestCloud = (GeneticWordCloudCloud)intialGeneration.GetSecondBestCloud().Copy();
            UpdateOnProgress(bestCloud);
            intialGeneration.Dispose();
            do
            {
                var newGeneration = new GeneticWordCloudGeneration(GenerationCount++, WordCount, CanvasHeight, CanvasWidth, Bitmap);
                await newGeneration.GeneratePopulation(bestCloud, secondBestCloud, 100);
                await newGeneration.ComputeScore();
                var newBestCloud = (GeneticWordCloudCloud)newGeneration.GetBestCloud().Copy();
                if (newBestCloud.Score >= bestCloud.Score)
                {
                    secondBestCloud = bestCloud;
                    bestCloud = newBestCloud;
                }
                else if (newBestCloud.Score >= secondBestCloud.Score)
                {
                    secondBestCloud = newBestCloud;
                }
                UpdateOnProgress(bestCloud);
                newGeneration.Dispose();

            } while (bestCloud.Score < WordCount.Count() || GenerationCount < 10);

            return bestCloud;
        }


    }
}
