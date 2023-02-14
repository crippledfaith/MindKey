using MindKey.Server.Models;
using MindKey.Server.Services.WordCloudGenerator;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Server.Services
{
    public class WordCloudService
    {

        private readonly IIdeaRepository _ideaRepository;
        private readonly IWordCloudGenerator _wordCloudGenerator;

        public WordCloudService(IIdeaRepository ideaRepository, IWordCloudGenerator wordCloudGenerator)
        {
            _ideaRepository = ideaRepository;
            _wordCloudGenerator = wordCloudGenerator;
        }

        public async Task<WorkCloudResult> GenerateWordCloud(WorkCloudParameter parameter)
        {
            var wordCount = await _ideaRepository.GetTags(20);
            return await _wordCloudGenerator.Generate(wordCount, parameter);
        }


    }
}
