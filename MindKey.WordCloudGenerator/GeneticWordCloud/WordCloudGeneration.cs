using SixLabors.ImageSharp;
using SkiaSharp;
using System.Collections.Concurrent;

namespace MindKey.WordCloudGenerator.GeneticWordCloud
{
    public class WordCloudGeneration : IDisposable, ICopyable
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
            await Task.Run(async () =>
            {
                Parallel.For(0, limit, (x) =>
                {
                    var cloud = new WordCloudCloud(GenerationNumber, x, WordCount, CanvasHeight, CanvasWidth, Bitmap);
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
            await Task.Run(async () =>
            {
                Parallel.For(0, limit, (x) =>
                {
                    var cloud = new WordCloudCloud(GenerationNumber, x, WordCount, CanvasHeight, CanvasWidth, Bitmap);
                    WordCloudClouds.Add(cloud);
                });
               
                foreach (var cloud in WordCloudClouds)
                {
                    await cloud.CrossOver(wordCloudCloud1, wordCloudCloud2);
                    await cloud.Mutate();
                    await cloud.RandomizeUnFitWords();

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

        public object Copy()
        {
            return MemberwiseClone();
        }
    }
}
