using MindKey.Server.Models;
using MindKey.Server.Services.WordCloudGenerator;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Server.Services
{
    public class WordCloudService
    {

        private readonly IIdeaRepository _ideaRepository;
        private readonly IWordCloudGenerator _wordCloudGenerator;
        private readonly IConfiguration _configuration;

        public WordCloudService(IIdeaRepository ideaRepository, IWordCloudGenerator wordCloudGenerator,IConfiguration configuration)
        {
            _ideaRepository = ideaRepository;
            _wordCloudGenerator = wordCloudGenerator;
            _configuration = configuration;
        }

        public async Task<WorkCloudResult> GenerateWordCloud(WorkCloudParameter parameter)
        {
            var wordCount = await _ideaRepository.GetTags(20);
            return await _wordCloudGenerator.Generate(wordCount, parameter);
        }


    }
}
