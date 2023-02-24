using MindKey.WordCloudGenerator.Base;
using SixLabors.ImageSharp;
using SkiaSharp;
using System.Collections.Concurrent;

namespace MindKey.WordCloudGenerator.GeneticWordCloud
{
    public class GeneticWordCloudGeneration : IDisposable, ICopyable
    {
        public int GenerationNumber { get; set; }
        public int CanvasHeight { get; private set; }
        public int CanvasWidth { get; private set; }
        public SKBitmap Bitmap { get; }
        public Dictionary<string, int> WordCount { get; }
        public ConcurrentBag<GeneticWordCloudCloud> WordCloudClouds { get; private set; }


        public GeneticWordCloudGeneration(int generationNumber, Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap)
        {
            GenerationNumber = generationNumber;
            WordCount = wordCount;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
            Bitmap = bitmap;
            WordCloudClouds = new ConcurrentBag<GeneticWordCloudCloud>();
        }


        public async Task GeneratePopulation(int limit = 10)
        {
            await Task.Run(async () =>
            {
                Parallel.For(0, limit, (x) =>
                {
                    var cloud = new GeneticWordCloudCloud(GenerationNumber, x, WordCount, CanvasHeight, CanvasWidth, Bitmap);
                    WordCloudClouds.Add(cloud);
                });
                foreach (var cloud in WordCloudClouds)
                {
                    await cloud.Generate();
                }
            });
        }
        public async Task GeneratePopulation(GeneticWordCloudCloud wordCloudCloud1, GeneticWordCloudCloud wordCloudCloud2, int limit = 10)
        {
            await Task.Run(async () =>
            {
                Parallel.For(0, limit, (x) =>
                {
                    var cloud = new GeneticWordCloudCloud(GenerationNumber, x, WordCount, CanvasHeight, CanvasWidth, Bitmap);
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

        public GeneticWordCloudCloud GetBestCloud()
        {
            return WordCloudClouds.ToList().OrderByDescending(q => q.Score).Take(1).First();
        }

        public GeneticWordCloudCloud GetSecondBestCloud()
        {
            return WordCloudClouds.ToList().OrderByDescending(q => q.Score).Skip(1).Take(1).First();
        }
        public async Task RemoveWordThatCannotFit()
        {
            await Task.Run(() => { WordCloudClouds.ToList().ForEach(q => q.RemoveWordThatCannotFit()); });
        }
        public object Copy()
        {
            return MemberwiseClone();
        }

        
    }
}
