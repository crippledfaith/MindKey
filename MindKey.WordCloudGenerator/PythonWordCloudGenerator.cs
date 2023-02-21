using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;
using Python.Runtime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MindKey.WordCloudGenerator
{
    public class PythonWordCloudGenerator : AWordCloudGenerator
    {

        private const string PythonFileName = "WordCloud.py";


        public PythonWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
            if (string.IsNullOrEmpty(Runtime.PythonDLL))
            {
                var appdataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string pyPath = Path.Combine(appdataLocal, "Programs", "Python", "Python310", "python310.dll");

                var pythonVersion = configuration.GetValue<string>("PythonVersion");
                var pythonExePath = GetPythonPath(pythonVersion);
                if (pythonExePath != null)
                {
                    var dllName = pythonVersion.Replace(".", "");
                    pyPath = pythonExePath.Replace("python.exe", $"python{dllName}.dll");
                }
                Runtime.PythonDLL = pyPath;
            }
        }

        private static string GetPythonPath(string requiredVersion = "", string maxVersion = "")
        {
            string[] possiblePythonLocations = new string[3] {
                @"HKLM\SOFTWARE\Python\PythonCore\",
                @"HKCU\SOFTWARE\Python\PythonCore\",
                @"HKLM\SOFTWARE\Wow6432Node\Python\PythonCore\"
            };

            //Version number, install path
            Dictionary<string, string> pythonLocations = new Dictionary<string, string>();

            foreach (string possibleLocation in possiblePythonLocations)
            {
                string regKey = possibleLocation.Substring(0, 4), actualPath = possibleLocation.Substring(5);
                RegistryKey theKey = (regKey == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser);
                RegistryKey theValue = theKey.OpenSubKey(actualPath);
                if (theValue != null)
                {
                    foreach (var v in theValue.GetSubKeyNames())
                    {
                        RegistryKey productKey = theValue.OpenSubKey(v);
                        if (productKey != null)
                        {
                            try
                            {
                                string pythonExePath = productKey.OpenSubKey("InstallPath").GetValue("ExecutablePath").ToString();

                                if (pythonExePath != null && pythonExePath != "")
                                {
                                    pythonLocations.Add(v.ToString(), pythonExePath);
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            if (pythonLocations.Count > 0)
            {
                System.Version desiredVersion = new System.Version(requiredVersion == "" ? "0.0.1" : requiredVersion),
                    maxPVersion = new System.Version(maxVersion == "" ? "999.999.999" : maxVersion);

                string highestVersion = "", highestVersionPath = "";

                foreach (KeyValuePair<string, string> pVersion in pythonLocations)
                {
                    int index = pVersion.Key.IndexOf("-");
                    string formattedVersion = index > 0 ? pVersion.Key.Substring(0, index) : pVersion.Key;

                    System.Version thisVersion = new System.Version(formattedVersion);
                    int comparison = desiredVersion.CompareTo(thisVersion),
                        maxComparison = maxPVersion.CompareTo(thisVersion);

                    if (comparison <= 0)
                    {

                        if (maxComparison >= 0)
                        {
                            desiredVersion = thisVersion;

                            highestVersion = pVersion.Key;
                            highestVersionPath = pVersion.Value;
                        }

                    }

                }

                return highestVersionPath;
            }

            return "";
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

    }
}
