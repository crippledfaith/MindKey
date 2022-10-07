using Microsoft.AspNetCore.Mvc;
using MindKey.Server.Authorization;
using MindKey.Server.Models;
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

        public IdeaController(IIdeaRepository ideaRepository, IUserRepository userRepository)
        {
            _ideaRepository = ideaRepository;
            _userRepository = userRepository;
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
    }
}
