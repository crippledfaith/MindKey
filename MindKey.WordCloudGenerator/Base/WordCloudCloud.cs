using SkiaSharp;
using System.Collections.Concurrent;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace MindKey.WordCloudGenerator.Base
{
    public abstract class WordCloudCloud : IDisposable, ICopyable
    {
        public int CanvasHeight { get; protected set; }
        public int CanvasWidth { get; protected set; }
        [JsonIgnore]
        public SKBitmap Bitmap { get; }
        public Dictionary<string, int> WordCount { get; }
        public ConcurrentBag<WordCloudWord> WordCloudWords { get; protected set; }

        public WordCloudCloud(Dictionary<string, int> wordCount, int canvasHeight, int canvasWidth, SKBitmap bitmap)
        {
            WordCount = new Dictionary<string, int>(wordCount);
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
            Bitmap = bitmap.Copy();
            WordCloudWords = new ConcurrentBag<WordCloudWord>();
        }

        public abstract object Copy();
        public abstract void Dispose();

    }
}
