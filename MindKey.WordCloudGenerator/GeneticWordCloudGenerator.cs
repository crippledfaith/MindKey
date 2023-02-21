using Microsoft.Extensions.Configuration;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;

namespace MindKey.WordCloudGenerator
{
    public class GeneticWordCloudGenerator : AWordCloudGenerator
    {
        public GeneticWordCloudGenerator(IConfiguration configuration) : base(configuration)
        {
        }

        protected override bool NeedMaskedFile(WorkCloudParameter parameter)
        {
            return true;
        }

        protected override Task<WorkCloudResult> Start(Dictionary<string, int> wordCount, WorkCloudParameter parameter)
        {
            throw new NotImplementedException();
        }
    }
}
