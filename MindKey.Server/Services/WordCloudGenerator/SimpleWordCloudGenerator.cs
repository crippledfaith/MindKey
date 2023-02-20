using MindKey.Shared.Models.MindKey;
using SixLabors.Fonts;

namespace MindKey.Server.Services.WordCloudGenerator
{
    public class SimpleWordCloudGenerator : AWordCloudGenerator
    {

        protected override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            var width = Convert.ToInt32(parameter.Width) - 50;
            var height = Convert.ToInt32(parameter.Height);

            var x = 50f;
            var y = 50f;
            var maxRowHeight = 0f;

            foreach (var word in wordCount)
            {
                WorkCloudData item = new WorkCloudData();

                item.FillStyle = GetRandomColor();
                item.Font = GetRandomFont(word.Value);
                var fo = SystemFonts.Get("Arial");
                var font = new Font(fo, word.Value + 20, FontStyle.Regular);
                var textSize = GetTextSize(word.Key, font);
                var textHeight = textSize.Y + textSize.Height;
                var textWidth = textSize.X + textSize.Width + 20;
                // set x and  y position to draw the circle
                if (x + textWidth > width)
                {
                    x = 50f;
                    y += maxRowHeight;
                    maxRowHeight = 0f;
                }
                if (textHeight > maxRowHeight)
                {
                    maxRowHeight = textHeight;
                }
                x += textWidth;
                y += textHeight;

                //update WordCloudResult.Data to  include the new word so that it can be displayed on the UI in a circle

                WordCloudResult.Data.Add(new WorkCloudData
                {
                    Word = word.Key,
                    Font = item.Font,
                    FillStyle = item.FillStyle,
                    X = x,
                    Y = y,

                });


            }
            return Task.FromResult(WordCloudResult);
        }

        private FontRectangle GetTextSize(string text, Font font)
        {
            return TextMeasurer.Measure(text, new TextOptions(font));
        }
        private string GetRandomFont(int fontSize)
        {
            var font = $"{fontSize + 20}pt Arial";
            return font;
        }
        private string GetRandomColor()
        {
            var color = string.Format("#{0:X6}", new Random().Next(0x1000000));
            return color;
        }

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return false;
        }
    }
}
