using MindKey.Shared.Models.MindKey;
using Python.Runtime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MindKey.Server.Services.WordCloudGenerator
{
    public class PythonWordCloudGenerator : AWordCloudGenerator
    {

        private const string PythonFileName = "WordCloud.py";

        static PythonWordCloudGenerator()
        {
            var appdataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pyPath = Path.Combine(appdataLocal, "Programs", "Python", "Python310", "python310.dll");
            Runtime.PythonDLL = pyPath;
        }

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }

        protected override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            object returnedVariable = new object();
            var codePath = Path.Combine(WordCloudWorkingPath, PythonFileName);

            PythonEngine.Initialize();

            var d = PythonEngine.BeginAllowThreads();
            using (Py.GIL())
            {
                using (PyModule scope = Py.CreateScope())
                {
                    var list = wordCount.SelectMany(q => Enumerable.Repeat(q.Key, q.Value));
                    scope.Set("wordlist", string.Join(" ", list).ToPython());
                    scope.Set("mask_path", MaskFilePath);
                    scope.Set("output_path", OutputFilePath);
                    scope.Exec(File.ReadAllText(codePath));
                    returnedVariable = scope.Get<object>("path");
                }
            }
            PythonEngine.EndAllowThreads(d);

            using var outputImage = Image.Load(returnedVariable.ToString());

            using var stream = new MemoryStream();
            outputImage.Save(stream, new JpegEncoder());

            var base64String = Convert.ToBase64String(stream.ToArray());
            WordCloudResult.Image = base64String;
            Directory.Delete(OutputPath, true);

            return Task.FromResult(WordCloudResult);
        }

    }
}
