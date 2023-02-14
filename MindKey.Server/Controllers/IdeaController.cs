using Microsoft.AspNetCore.Mvc;
using MindKey.Server.Authorization;
using MindKey.Server.Models;
using MindKey.Server.Services;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IdeaController : ControllerBase
    {
        private readonly IIdeaRepository _ideaRepository;
        private readonly IUserRepository _userRepository;
        private readonly WordCloudService _wordCloudService;

        public IdeaController(IIdeaRepository ideaRepository, IUserRepository userRepository, WordCloudService wordCloudService)
        {
            _ideaRepository = ideaRepository;
            _userRepository = userRepository;
            _wordCloudService = wordCloudService;
        }

        /// <summary>
        /// Returns a list of paginated people with a default page size of 5.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetIdeas([FromQuery] int page, long? userId)
        {
            return Ok(_ideaRepository.GetIdeas(page, userId));
        }
        [AllowAnonymous]
        [HttpGet("top")]
        public ActionResult GetTopIdeas([FromQuery] int page)
        {
            return Ok(_ideaRepository.GetTopIdeas(page));
        }

        [AllowAnonymous]
        [HttpGet("others")]
        public ActionResult GetIdeasOfOthers([FromQuery] int page, long? userId)
        {
            return Ok(_ideaRepository.GetIdeasOfOthers(page, userId));
        }

        /// <summary>
        /// Gets a specific person by Id.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetIdea(long id)
        {
            return Ok(await _ideaRepository.GetIdea(id));
        }

        /// <summary>
        /// Creates a person with child addresses.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddIdea(Idea idea)
        {
            return Ok(await _ideaRepository.AddIdea(idea));
        }
        /// <summary>
        /// Creates a person with child addresses.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> DeleteIdea(Idea idea)
        {
            return Ok(await _ideaRepository.DeleteIdea(idea.Id));
        }
        /// <summary>
        /// Creates a person with child addresses.
        /// </summary>
        [HttpPost("SetArgument")]
        public async Task<ActionResult> SetArgument(IdeaUserComment ideaUserComment)
        {
            return Ok(await _ideaRepository.SetArgument(ideaUserComment));
        }

        /// <summary>
        /// Creates a person with child addresses.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("GetSetArgument")]
        public async Task<ActionResult> GetSetArgument(IdeaUserComment ideaUserComment)
        {
            var result = await _ideaRepository.GetSetArgument(ideaUserComment);
            return Ok(result ?? new IdeaUserComment());
        }

        /// <summary>
        /// Updates a person with a specific Id.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult> UpdateIdea(Idea idea)
        {
            return Ok(await _ideaRepository.UpdateIdea(idea));
        }

        /// <summary>
        /// Deletes a person with a specific Id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIdea(long id)
        {
            return Ok(await _ideaRepository.DeleteIdea(id));
        }

        [AllowAnonymous]
        [HttpGet("GetComments")]
        public ActionResult GetComments([FromQuery] int page, int pageSize, long? ideaId)
        {
            return Ok(_ideaRepository.GetComments(page, pageSize, ideaId));
        }

        [AllowAnonymous]
        [HttpGet("GetTags")]
        public async Task<ActionResult> GetTags([FromQuery] int count)
        {
            return Ok(await _ideaRepository.GetTags(count));
        }

        [AllowAnonymous]
        [HttpPost("GetWordCloud")]
        public async Task<WorkCloudResult> GetWordCloud(WorkCloudParameter param)
        {
            return await _wordCloudService.GenerateWordCloud(param);
        }
    }
}
