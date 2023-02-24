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

       
    }
}
