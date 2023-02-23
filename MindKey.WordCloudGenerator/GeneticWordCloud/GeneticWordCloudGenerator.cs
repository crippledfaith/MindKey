using IronSoftware.Drawing;
using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.Fonts;
using SkiaSharp;
using System;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace MindKey.WordCloudGenerator.GeneticWordCloud
{
    public class GeneticWordCloudGenerator : AWordCloudGenerator
    {
        public GeneticWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
        }

        public override event EventHandler<WorkCloudResult> OnProgress;

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }
        protected async override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            using SKBitmap bitmap = SKBitmap.Decode(MaskFilePath);

            using SKCanvas canvas = new SKCanvas(bitmap);

            var canvasHeight = parameter.Height;
            var canvasWidth = parameter.Width;

            var service = new GeneticService(wordCount, canvasHeight, canvasWidth, bitmap.Copy());
            service.OnProgress += ServiceOnProgress;
            ChangeColor(bitmap);
            using (var stream = File.OpenWrite(Path.Combine(OutputPath, "newBackGround.jpg")))
            {
                bitmap.Encode(stream, SKEncodedImageFormat.Jpeg, 100);
            }
            var wordCloudCloud = await service.Start();

            foreach (var word in wordCloudCloud.WordCloudWords.Where(q => q.IsFit.HasValue && q.IsFit.Value))
            {
                canvas.DrawText(word.Text, word.DrawX, word.DrawY, word.Font, word.Paint);
            }
            using SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
            var base64String = Convert.ToBase64String(data.ToArray());
            var isFitCount = wordCloudCloud.WordCloudWords.Count(q => q.IsFit.HasValue && q.IsFit.Value);
            var totalCount = wordCloudCloud.WordCount.Count;
            WordCloudResult.Status = $"{wordCloudCloud.GenarationNumber} {isFitCount}/{totalCount} {isFitCount / totalCount}%";
            WordCloudResult.Image = base64String;
            Directory.Delete(OutputPath, true);
            return WordCloudResult;
        }
        public void ChangeColor(SKBitmap bitmap)
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
        private void ServiceOnProgress(object? sender, WordCloudCloud wordCloudCloud)
        {
            using SKBitmap bitmap = SKBitmap.Decode(Path.Combine(OutputPath, "newBackGround.jpg"));
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
            WordCloudResult.Status = $"{wordCloudCloud.GenarationNumber} {isFitCount}/{totalCount} {Convert.ToDouble(isFitCount) / Convert.ToDouble(totalCount) * 100d}%";
            OnProgress?.Invoke(sender, WordCloudResult);
        }
    }
}
