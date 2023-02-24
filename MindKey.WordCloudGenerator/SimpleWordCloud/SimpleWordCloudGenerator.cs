using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using SixLabors.Fonts;
using System.Drawing;

namespace MindKey.WordCloudGenerator.SimpleWordCloud
{
    public class SimpleWordCloudGenerator : AWordCloudGenerator
    {
        public override event EventHandler<WorkCloudResult>? OnProgress;
        public SimpleWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
        }

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return false;
        }
        protected async override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var width = Convert.ToInt32(parameter.Width) - 50;
                var height = Convert.ToInt32(parameter.Height);

                var x = 50f;
                var y = 50f;
                var maxRowHeight = 0f;

                foreach (var word in wordCount)
                {
                    WorkCloudData item = new WorkCloudData();
                    var fontSize = (word.Value + 5) * 5;
                    item.FillStyle = GetRandomColor();
                    item.Font = GetRandomFont(fontSize);
                    var fo = SixLabors.Fonts.SystemFonts.Get("Arial");
                    var font = new SixLabors.Fonts.Font(fo, fontSize, SixLabors.Fonts.FontStyle.Regular);
                    var textSize = GetTextSize(word.Key, font);
                    var textHeight = textSize.Y + textSize.Height;
                    var textWidth = textSize.X + textSize.Width + 20;
                    // set x and  y position to draw the circle

                    if (x + textWidth >= width)
                    {
                        x = 50;
                        y += maxRowHeight;
                        maxRowHeight = 0;
                        if (y > height)
                            break;
                    }

                    maxRowHeight = Math.Max(maxRowHeight, textHeight);
                    item.Word = word.Key;
                    item.X = x;
                    item.Y = y;
                    item.Rotate = 0;

                    WordCloudResult.Data.Add(item);

                    x += textWidth + 25;
                }
                return WordCloudResult;
            });
        }

        private FontRectangle GetTextSize(string text, SixLabors.Fonts.Font font)
        {
            return TextMeasurer.Measure(text, new TextOptions(font));
        }
        private string GetRandomFont(int fontSize)
        {
            var font = $"{fontSize}pt Arial";
            return font;
        }
        private string GetRandomColor()
        {
            var maxValue = 1.0;
            var minValue = 0.6;

            var rnd = new Random().NextDouble();
            var l = maxValue * rnd + minValue * (1d - rnd);
            var color = ColorFromHSL(0, 0, l);
            var colorString = string.Format($"#{color.R:X2}{color.G:X2}{color.B:X2}");
            return colorString;
        }
        public static Color ColorFromHSL(double h, double s, double l)
        {
            double r = 0, g = 0, b = 0;
            if (l != 0)
            {
                if (s == 0)
                    r = g = b = l;
                else
                {
                    double temp2;
                    if (l < 0.5)
                        temp2 = l * (1.0 + s);
                    else
                        temp2 = l + s - l * s;

                    double temp1 = 2.0 * l - temp2;

                    r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, h);
                    b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));

        }
        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;

            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + (temp2 - temp1) * (2.0 / 3.0 - temp3) * 6.0;
            else
                return temp1;
        }


    }
}
