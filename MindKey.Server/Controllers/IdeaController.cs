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
        /// Returns a list of paginated ideas with a default page size of 5.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetIdeas([FromQuery] int page, long? userId)
        {
            return Ok(_ideaRepository.GetIdeas(page, userId));
        }

        /// <summary>
        /// Returns a list of top ideas with a default page size of 5.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("top")]
        public ActionResult GetTopIdeas([FromQuery] int page)
        {
            return Ok(_ideaRepository.GetTopIdeas(page));
        }
        /// <summary>
        /// Returns a list of ideas of other users.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("others")]
        public ActionResult GetIdeasOfOthers([FromQuery] int page, long? userId)
        {
            return Ok(_ideaRepository.GetIdeasOfOthers(page, userId));
        }

        /// <summary>
        /// Gets a specific idea by Id.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetIdea(long id)
        {
            return Ok(await _ideaRepository.GetIdea(id));
        }

        /// <summary>
        /// Creates a new Idea.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddIdea(Idea idea)
        {
            return Ok(await _ideaRepository.AddIdea(idea));
        }
        /// <summary>
        /// Delete an Idea.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> DeleteIdea(Idea idea)
        {
            return Ok(await _ideaRepository.DeleteIdea(idea.Id));
        }


        /// <summary>
        /// Updates an Idea with a specific Id.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult> UpdateIdea(Idea idea)
        {
            return Ok(await _ideaRepository.UpdateIdea(idea));
        }

        /// <summary>
        /// Deletes a Idea with a specific Id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIdea(long id)
        {
            return Ok(await _ideaRepository.DeleteIdea(id));
        }
        /// <summary>
        /// Set an Argument to and Idea
        /// </summary>
        [HttpPost("SetComment")]
        public async Task<ActionResult> SetComment(IdeaUserComment ideaUserComment)
        {
            return Ok(await _ideaRepository.SetComment(ideaUserComment));
        }

        /// <summary>
        /// Get an Argument by id 
        /// </summary>
        [AllowAnonymous]
        [HttpPost("GetComment")]
        public async Task<ActionResult> GetComment(IdeaUserComment ideaUserComment)
        {
            var result = await _ideaRepository.GetComment(ideaUserComment);
            return Ok(result ?? new IdeaUserComment());
        }


        /// <summary>
        /// Get a list of comment
        /// </summary>
        [AllowAnonymous]
        [HttpGet("GetComments")]
        public ActionResult GetComments([FromQuery] int page, int pageSize, long? ideaId)
        {
            return Ok(_ideaRepository.GetComments(page, pageSize, ideaId));
        }

        /// <summary>
        /// Get a list of Tags
        /// </summary>
        [AllowAnonymous]
        [HttpGet("GetTags")]
        public async Task<ActionResult> GetTags([FromQuery] int count)
        {
            return Ok(await _ideaRepository.GetTags(count));
        }
        /// <summary>
        /// Generate WordCloud
        /// </summary>
        [AllowAnonymous]
        [HttpPost("GetWordCloud")]
        public async Task<WorkCloudResult> GetWordCloud(WorkCloudParameter param)
        {
            return await _wordCloudService.GenerateWordCloud(param);
        }
    }
}
