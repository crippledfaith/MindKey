using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using Python.Runtime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MindKey.WordCloudGenerator.PythonWordCloud
{
    public class PythonWordCloudGenerator : AWordCloudGenerator
    {
        private const string PythonFileName = "WordCloud.py";
        
        public PythonWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
            if (string.IsNullOrEmpty(Runtime.PythonDLL))
            {
                var appdataLocal = configuration.GetValue<string>("PythonPath");
                string pyPath = Path.Combine(appdataLocal, "python311.dll");

                var pythonVersion = configuration.GetValue<string>("PythonVersion");
                pyPath = appdataLocal;
                //if (pythonExePath != null)
                //{
                //    var dllName = pythonVersion.Replace(".", "");
                //    pyPath = pythonExePath.Replace("python.exe", $"python{dllName}.dll");
                //}
                Runtime.PythonDLL = pyPath;
            }
        }
        
        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }
        protected async override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
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

                return WordCloudResult;
            });
        }
        
        private static string GetPythonPath(string requiredVersion = "", string maxVersion = "")
        {

            //Version number, install path
            Dictionary<string, string> pythonLocations = new Dictionary<string, string>();
            return "";
            
        }

    }
}
