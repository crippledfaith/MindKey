using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.GeneticWordCloud;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;

namespace MindKey.WordCloudGenerator.Base
{
    public abstract class AWordCloudGenerator : IWordCloudGenerator
    {
        private const string WebRootFolder = "WebRoot";
        private const string WordCloudFolderName = "WordCloud";
        private const string OutputFolderName = "output";

        private const string MaskImageFileName = "wordcloud.png";
        private const string OutputFileName = "wordcloud_out.png";

        private const string MaskLargeFileName = "wordcloud-1.png";
        private const string MaskSmallFileName = "wordcloud-min1.png";
        private const string FinalMaskFileName = "newBackGround.jpg";
        public readonly IConfiguration Configuration;

        public string? AppPath { get; private set; }
        public string? WordCloudWorkingPath { get; private set; }
        public string? OutputPath { get; private set; }
        public string? ImageFilePath { get; private set; }
        public string? MaskFilePath { get; private set; }
        public string? OutputFilePath { get; private set; }
        public string? FinalMaskPath { get; private set; }
        public WorkCloudResult WordCloudResult { get; set; } = new WorkCloudResult();
        public event EventHandler<WorkCloudResult>? OnProgress;
        public AWordCloudGenerator(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected abstract bool NeedMaskedFile(WorkCloudParameter parameter);
        protected abstract Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter);
        public async Task<WorkCloudResult> Generate(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            WordCloudWorkingPath = Path.Combine(AppPath, WebRootFolder, WordCloudFolderName);
            OutputPath = Path.Combine(WordCloudWorkingPath, OutputFolderName, Guid.NewGuid().ToString("D"));

            ImageFilePath = Path.Combine(WordCloudWorkingPath, parameter.Width > 500 ? MaskLargeFileName : MaskSmallFileName);
            MaskFilePath = Path.Combine(OutputPath, MaskImageFileName);
            OutputFilePath = Path.Combine(OutputPath, OutputFileName);
            FinalMaskPath = Path.Combine(OutputPath, FinalMaskFileName);
            if (NeedMaskedFile(parameter))
            {
                Directory.CreateDirectory(OutputPath);
                using var image = Image.Load(ImageFilePath);
                using var canvas = new Image<Rgba32>(parameter.Width, parameter.Height, Color.Black);
                var x = (canvas.Width - image.Width) / 2;
                var y = (canvas.Height - image.Height) / 2;
                canvas.Mutate(ctx => ctx.DrawImage(image, new Point(x, y), 1f));
                canvas.Save(MaskFilePath);
                using SKBitmap bitmap = SKBitmap.Decode(MaskFilePath);
                ChangeColor(bitmap);
                using (var stream = File.OpenWrite(FinalMaskPath))
                {
                    bitmap.Encode(stream, SKEncodedImageFormat.Jpeg, 100);
                }
            }
            return await Start(wordCount, parameter);
        }
        protected void ServiceOnProgress(object? sender, WordCloudCloud wordCloudCloud)
        {
            using SKBitmap bitmap = SKBitmap.Decode(Path.Combine(FinalMaskPath));
            using SKCanvas canvas = new SKCanvas(bitmap);
            //ChangeColor(bitmap);
            foreach (var word in wordCloudCloud.WordCloudWords.Where(q => q.IsFit.HasValue && q.IsFit.Value))
            {
                canvas.DrawText(word.Text, word.DrawX, word.DrawY, word.Font, word.Paint);
            }
            using SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
            var base64String = Convert.ToBase64String(data.ToArray());
            WordCloudResult.Image = base64String;
            var isFitCount = wordCloudCloud.WordCloudWords.Count(q => q.IsFit.HasValue && q.IsFit.Value);
            var totalCount = wordCloudCloud.WordCount.Count;
            WordCloudResult.Status = $"{((GeneticWordCloudCloud)wordCloudCloud).GenarationNumber} {isFitCount}/{totalCount} {Convert.ToDouble(isFitCount) / Convert.ToDouble(totalCount) * 100d}%";
            OnProgress?.Invoke(sender, WordCloudResult);
        }
        protected void ChangeColor(SKBitmap bitmap)
        {
            int pixelCount = bitmap.Width * bitmap.Height;
            SKColor[] colors = new SKColor[pixelCount];
            SKColor gray = new SKColor(34, 34, 34);
            SKColor steelBlue = new SKColor(70, 130, 180);
            var n = 0;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    SKColor color = bitmap.GetPixel(x, y);
                    if (CheckIfBlack(color))
                    {
                        colors[n] = gray;
                    }
                    //else if (!CheckIfWhite(color))
                    //{
                    //    colors[n] = steelBlue;
                    //}
                    else
                    {
                        colors[n] = steelBlue;
                    }
                    n++;
                }
            }
            bitmap.Pixels = colors;
        }
        private bool CheckIfBlack(SKColor color)
        {
            return color.Alpha == 255 && color.Red == 0 && color.Blue == 0 && color.Green == 0 && color.Hue == 0;
        }
        private bool CheckIfWhite(SKColor color)
        {
            return color.Alpha == 255 && color.Red == 255 && color.Blue == 255 && color.Green == 255 && color.Hue == 0;
        }

    }
}
