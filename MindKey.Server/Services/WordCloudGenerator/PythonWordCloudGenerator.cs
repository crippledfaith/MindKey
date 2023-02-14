using MindKey.Shared.Models.MindKey;
using Python.Runtime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MindKey.Server.Services.WordCloudGenerator
{
    public class PythonWordCloudGenerator : AWordCloudGenerator
    {
        private const string WebRootFolder = "WebRoot";
        private const string WordCloudFolderName = "WordCloud";
        private const string OutputFolderName = "output";

        private const string MaskImageFileName = "wordcloud.png";
        private const string OutputFileName = "wordcloud_out.png";
        private const string PythonFileName = "WordCloud.py";

        private const string MaskLargeFileName = "wordcloud-1.png";
        private const string MaskSmallFileName = "wordcloud-min1.png";

        static PythonWordCloudGenerator()
        {
            var appdataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pyPath = Path.Combine(appdataLocal, "Programs", "Python", "Python310", "python310.dll");
            Runtime.PythonDLL = pyPath;
        }
        public override Task<WorkCloudResult> Generate(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            object returnedVariable = new object();

            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var wordCloudWorkingPath = Path.Combine(appPath, WebRootFolder, WordCloudFolderName);
            var imageFilePath = Path.Combine(wordCloudWorkingPath, parameter.Width > 500 ? MaskLargeFileName : MaskSmallFileName);
            var outputPath = Path.Combine(wordCloudWorkingPath, OutputFolderName, Guid.NewGuid().ToString("D"));
            Directory.CreateDirectory(outputPath);
            var maskFilePath = Path.Combine(outputPath, MaskImageFileName);
            var outputFilePath = Path.Combine(outputPath, OutputFileName);
            var codePath = Path.Combine(wordCloudWorkingPath, PythonFileName);

            using var image = Image.Load(imageFilePath);
            using var canvas = new Image<Rgba32>(parameter.Width, parameter.Height, Color.Black);
            var x = (canvas.Width - image.Width) / 2;
            var y = (canvas.Height - image.Height) / 2;
            canvas.Mutate(ctx => ctx.DrawImage(image, new Point(x, y), 1f));
            canvas.Save(maskFilePath);
            PythonEngine.Initialize();
            var d = PythonEngine.BeginAllowThreads();
            using (Py.GIL())
            {
                using (PyModule scope = Py.CreateScope())
                {
                    var list = wordCount.SelectMany(q => Enumerable.Repeat(q.Key, q.Value));
                    scope.Set("wordlist", string.Join(" ", list).ToPython());
                    scope.Set("mask_path", maskFilePath);
                    scope.Set("output_path", outputFilePath);
                    scope.Exec(File.ReadAllText(codePath));
                    returnedVariable = scope.Get<object>("path");
                }
            }
            PythonEngine.EndAllowThreads(d);
            using var outputImage = Image.Load(returnedVariable.ToString());

            // Encode the image as a JPEG using the ImageSharp JPEG encoder
            using var stream = new MemoryStream();
            outputImage.Save(stream, new JpegEncoder());

            // Convert the encoded binary data to a base64-encoded string
            var base64String = Convert.ToBase64String(stream.ToArray());
            WordCloudResult.Image = base64String;
            Directory.Delete(outputPath, true);
            return Task.FromResult(WordCloudResult);
        }

    }
}
