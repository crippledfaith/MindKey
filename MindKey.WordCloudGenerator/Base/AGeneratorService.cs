using MindKey.WordCloudGenerator.GeneticWordCloud;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindKey.WordCloudGenerator.Base
{
    public abstract class AGeneratorService
    {
        public event EventHandler<WordCloudCloud>? OnProgress;

        public Dictionary<string, int> WordCount { get; protected set; }
        public int CanvasHeight { get; protected set; }
        public int CanvasWidth { get; protected set; }
        public SKBitmap Bitmap { get; protected set; }
        public AGeneratorService(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap)
        {
            WordCount = wordCount;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
            Bitmap = bitmap;
        }

        protected void UpdateOnProgress(WordCloudCloud wordCloudCloud)
        {
            if(OnProgress!=null)
            {
                OnProgress(this, wordCloudCloud);
            }
        }
    }
}
