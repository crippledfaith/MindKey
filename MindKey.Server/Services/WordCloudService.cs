using MindKey.Server.Models;
using MindKey.Shared.Models.MindKey;
using MindKey.WordCloudGenerator.Base;

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
            var wordCount = await _ideaRepository.GetTags(_configuration.GetSection("WordCloudGenerator").GetValue<int>("Limit"));

            return await _wordCloudGenerator.Generate(wordCount, parameter);
        }


    }
}
