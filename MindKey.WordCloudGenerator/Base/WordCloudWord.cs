using IronSoftware.Drawing;
using MindKey.WordCloudGenerator.GeneticWordCloud;
using SkiaSharp;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace MindKey.WordCloudGenerator.Base
{
    public class WordCloudWord : IDisposable, ICopyable
    {
        [JsonIgnore]
        private SKRect? _textSize;

        private float x;
        private float y;

        [JsonIgnore]
        public bool? IsFit { get; set; }
        public float X
        {
            get => x;
            set
            {
                x = value;
                IsFit = null;
            }
        }
        public float Y
        {
            get => y;
            set
            {
                y = value;
                IsFit = null;
            }
        }
        public float DrawX => _textSize.HasValue && Paint != null && Text != null ? X + _textSize.Value.Width / 2 : 0f;
        public float DrawY => _textSize.HasValue && Paint != null && Text != null ? Y + GetTextMesurements(Paint, Text).Height : 0f;
        public string? Text { get; private set; }
        public int Frequncy { get; private set; }
        public SKFont? Font { get; private set; }
        public SKPaint? Paint { get; private set; }
        public int CanvasHeight { get; set; }
        public int CanvasWidth { get; set; }
        public WordCloudWord(float x, float y, string? text, int frequncy, SKFont font, int canvasHeight, int canvasWidth)
        {
            X = x;
            Y = y;
            Text = text;
            Frequncy = frequncy;
            Font = font;
            CanvasHeight = canvasHeight;
            CanvasWidth = canvasWidth;
        }
        public void Initilize()
        {
            if (Font == null)
                throw new Exception("Font can't be null");
            IsFit = null;
            Paint = GetPaint(Font.Size);
        }

        public bool DoesFit(SKBitmap bitmap)
        {
            if (!IsFit.HasValue)
            {
                if (Paint == null || Text == null)
                    throw new Exception("Paint or Text can't be null");
                IsFit = !CheckCollisionInRender(bitmap, X, Y, GetTextMesurements(Paint, Text), CanvasHeight, CanvasWidth);
            }
            return IsFit.Value;
        }


        public void Dispose()
        {
            Paint?.Dispose();
            Font?.Dispose();
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
        private SKPaint GetPaint(float fontSize)
        {
            var paint = new SKPaint();
            paint.TextSize = fontSize;
            paint.IsAntialias = true;
            paint.Color = GetRandomColor();
            paint.IsStroke = false;
            paint.StrokeWidth = 1;
            paint.TextAlign = SKTextAlign.Center;
            return paint;
        }
        private Color GetRandomColor()
        {
            var maxValue = 1.0;
            var minValue = 0.6;

            var rnd = new Random().NextDouble();
            var l = maxValue * rnd + minValue * (1d - rnd);
            return ColorFromHSL(0, 0, l);

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
        private bool CheckIfBlack(SKColor color)
        {
            return color.Alpha == 255 && color.Red == 0 && color.Blue == 0 && color.Green == 0 && color.Hue == 0;
        }

        public SKRect GetTextMesurements(SKPaint paint, string text)
        {
            if (_textSize == null)
            {
                var bounds = new SKRect();
                paint.MeasureText(text, ref bounds);
                _textSize = bounds;
            }
            return _textSize.Value;
        }

        public object Copy()
        {
            var newObj = (WordCloudWord)MemberwiseClone();
            newObj.Paint = Paint?.Clone();
            if (Font != null)
                newObj.Font = new SKFont(Font.Typeface, Font.Size, Font.ScaleX, Font.SkewX);
            return newObj;
        }
    }
}
