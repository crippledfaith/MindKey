using IronSoftware.Drawing;
using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using SixLabors.ImageSharp;
using SkiaSharp;
using System.Drawing;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace MindKey.WordCloudGenerator
{
    public class GeneticWordCloudGenerator : AWordCloudGenerator
    {
        public GeneticWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
        }

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }

        protected async override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var random = new Random();
                using SKBitmap bitmap = SKBitmap.Decode(MaskFilePath);
                using SKCanvas canvas = new SKCanvas(bitmap);

                canvas.DrawCircle(0, 0, 50, new SKPaint { Color = SKColors.Red });
                foreach (var item in wordCount)
                {
                    var x = random.Next(0, parameter.Width - 1);
                    var y = random.Next(0, parameter.Height - 1);
                    //bitmap.GetPixel()
                    using var paint = new SKPaint();
                    paint.TextSize = 64.0f;
                    paint.IsAntialias = true;
                    paint.Color = SKColors.White;
                    paint.IsStroke = true;
                    paint.StrokeWidth = 3;
                    paint.TextAlign = SKTextAlign.Center;
                    using var font = new SKFont(SKTypeface.Default, 40, 1, 0);
                    var bounds = new SKRect();
                    paint.MeasureText("Skia (UTF-32)", ref bounds);
                    //var glyphs = font.MeasureText(item.Key.ToCharArray(), paint);
                    canvas.DrawText(item.Key, x, y, font, paint);
                }

                using SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                var base64String = Convert.ToBase64String(data.ToArray());
                WordCloudResult.Image = base64String;
                Directory.Delete(OutputPath, true);
                return WordCloudResult;
            });
        }
    }
}
