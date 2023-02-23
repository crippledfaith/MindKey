using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace MindKey.WordCloudGenerator.Base
{
    public abstract class AWordCloudGenerator : IWordCloudGenerator
    {
        private const string WebRootFolder = "WebRoot";
        private const string WordCloudFolderName = "WordCloud";
        private const string OutputFolderName = "output";

        private const string MaskImageFileName = "wordcloud.png";
        private const string OutputFileName = "wordcloud_out.png";

        private const string MaskLargeFileName = "wordcloud-1.png";
        private const string MaskSmallFileName = "wordcloud-min1.png";

        public readonly IConfiguration Configuration;

        public string? AppPath { get; private set; }
        public string? WordCloudWorkingPath { get; private set; }
        public string? OutputPath { get; private set; }
        public string? ImageFilePath { get; private set; }
        public string? MaskFilePath { get; private set; }
        public string? OutputFilePath { get; private set; }
        public WorkCloudResult WordCloudResult { get; set; } = new WorkCloudResult();
        public abstract event EventHandler<WorkCloudResult> OnProgress;

        public AWordCloudGenerator(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected abstract bool NeedMaskedFile(WorkCloudParameter parameter);
        protected abstract Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter);
        public async Task<WorkCloudResult> Generate(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            WordCloudWorkingPath = Path.Combine(AppPath, WebRootFolder, WordCloudFolderName);
            OutputPath = Path.Combine(WordCloudWorkingPath, OutputFolderName, Guid.NewGuid().ToString("D"));

            ImageFilePath = Path.Combine(WordCloudWorkingPath, parameter.Width > 500 ? MaskLargeFileName : MaskSmallFileName);
            MaskFilePath = Path.Combine(OutputPath, MaskImageFileName);
            OutputFilePath = Path.Combine(OutputPath, OutputFileName);
            if (NeedMaskedFile(parameter))
            {
                Directory.CreateDirectory(OutputPath);
                using var image = Image.Load(ImageFilePath);
                using var canvas = new Image<Rgba32>(parameter.Width, parameter.Height, Color.Black);
                var x = (canvas.Width - image.Width) / 2;
                var y = (canvas.Height - image.Height) / 2;
                canvas.Mutate(ctx => ctx.DrawImage(image, new Point(x, y), 1f));
                canvas.Save(MaskFilePath);
            }
            return await Start(wordCount, parameter);
        }


    }
}
