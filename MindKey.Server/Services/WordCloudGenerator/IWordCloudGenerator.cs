using MindKey.Shared.Models.MindKey;

namespace MindKey.Server.Services.WordCloudGenerator
{
    public interface IWordCloudGenerator
    {
        Task<WorkCloudResult> Generate(Dictionary<string, int> wordCount, WorkCloudParameter parameter);
    }
    public abstract class AWordCloudGenerator : IWordCloudGenerator
    {
        public WorkCloudResult WordCloudResult { get; set; } = new WorkCloudResult();
        public abstract Task<WorkCloudResult> Generate(Dictionary<string, int> wordCount, WorkCloudParameter parameter);


    }
}
