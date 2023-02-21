using MindKey.Shared.Models.MindKey;

namespace MindKey.WordCloudGenerator.Base
{
    public interface IWordCloudGenerator
    {
        Task<WorkCloudResult> Generate(Dictionary<string, int> wordCount, WorkCloudParameter parameter);
    }
}
