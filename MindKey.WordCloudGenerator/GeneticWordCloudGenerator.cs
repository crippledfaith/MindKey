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

                //canvas.DrawCircle(0, 0, 50, new SKPaint { Color = SKColors.Red });
                var canvasHeight = parameter.Height;
                var canvasWidth = parameter.Width;
                foreach (var item in wordCount)
                {
                    var text = item.Key;
                    var fontSize = item.Value + 10;

                    //var x = 0f;
                    //var y = 0f;
                    using SKPaint paint = GetPaint(fontSize);

                    SKRect bounds = GetTextMesurements(paint, text);
                    var x = (float)random.Next(0, parameter.Width - 1);
                    var y = (float)random.Next(0, parameter.Height - 1);

                    using var font = new SKFont(SKTypeface.Default, fontSize, 1, 0);
                    bool collision = CheckCollisionInRender(bitmap, x, y, bounds, canvasHeight, canvasWidth);
                    if (collision) continue;
                    x = x + bounds.Width / 2;
                    y = y + bounds.Height;
                    canvas.DrawText(item.Key, x, y, font, paint);
                    //break;
                }

                using SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                var base64String = Convert.ToBase64String(data.ToArray());
                WordCloudResult.Image = base64String;
                Directory.Delete(OutputPath, true);
                return WordCloudResult;
            });
        }

        private bool CheckCollisionInRender(SKBitmap bitmap, float x, float y, SKRect bounds, int canvasHeight, int canvasWidth)
        {
            var endX = x + bounds.Width;
            var endY = y + bounds.Height;
            if (endX > canvasWidth || endY > canvasHeight)
                return true;
            for (var xCheck = x; xCheck < endX; xCheck++)
            {
                for (var yCheck = y; yCheck < endY; yCheck++)
                {
                    var color = bitmap.GetPixel((int)xCheck, (int)yCheck);
                    if (!CheckIfBlack(color))
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        private bool CheckIfBlack(SKColor color)
        {
            return color.Alpha == 255 && color.Red == 0 && color.Blue == 0 && color.Green == 0 && color.Hue == 0;
        }

        private static SKRect GetTextMesurements(SKPaint paint, string text)
        {
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            return bounds;
        }

        private static SKPaint GetPaint(float fontSize)
        {
            var paint = new SKPaint();
            paint.TextSize = fontSize;
            paint.IsAntialias = true;
            paint.Color = SKColors.White;
            paint.IsStroke = false;
            paint.StrokeWidth = 1;
            paint.TextAlign = SKTextAlign.Center;
            return paint;
        }
    }
}
