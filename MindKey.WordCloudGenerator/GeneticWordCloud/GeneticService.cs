using SkiaSharp;

namespace MindKey.WordCloudGenerator.GeneticWordCloud
{
    public class GeneticService
    {
        public event EventHandler<WordCloudCloud>? OnProgress;
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
            await intialGeneration.RemoveWordThatCannotFit();
            WordCloudCloud bestCloud = (WordCloudCloud)intialGeneration.GetBestCloud().Copy();
            WordCloudCloud secondBestCloud = (WordCloudCloud)intialGeneration.GetSecondBestCloud().Copy();
            OnProgress(this, bestCloud);
            intialGeneration.Dispose();
            do
            {
                var newGeneration = new WordCloudGeneration(GenerationCount++, WordCount, CanvasHeight, CanvasWidth, Bitmap);
                await newGeneration.GeneratePopulation(bestCloud, secondBestCloud, 100);
                await newGeneration.ComputeScore();
                var newBestCloud = (WordCloudCloud)newGeneration.GetBestCloud().Copy();
                if (newBestCloud.Score >= bestCloud.Score)
                {
                    secondBestCloud = bestCloud;
                    bestCloud = newBestCloud;
                }
                else if (newBestCloud.Score >= secondBestCloud.Score)
                {
                    secondBestCloud = newBestCloud;
                }
                OnProgress(this, bestCloud);
                newGeneration.Dispose();

            } while (bestCloud.Score < WordCount.Count() || GenerationCount < 10);

            return bestCloud;
        }


    }
}
