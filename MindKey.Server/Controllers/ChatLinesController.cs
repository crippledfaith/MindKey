using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MindKey.Server.Models;
using MindKey.Shared.Models;
using MindKey.Server.Authorization;

namespace MindKey.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatLinesController : ControllerBase
    {
        private readonly IChatLineRepository _chatLineRepository;
        public ChatLinesController(IChatLineRepository chatLineRepository)
        {
            _chatLineRepository = chatLineRepository;
        }
        [HttpGet("GetPaged")]
        public ActionResult GetPaged(int page)
        {
            return Ok(_chatLineRepository.GetPaged(page));
        }
        [HttpGet("GetPagedByIdea")]
        public ActionResult GetPagedByIdea(int page, long ideaId)
        {
            return Ok(_chatLineRepository.GetPagedByIdea(page, ideaId));
        }
        [HttpGet("GetByIdea")]
        public async Task<ActionResult> GetByIdea(long ideaId)
        {
            return Ok(await _chatLineRepository.GetByIdea(ideaId));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            return Ok(await _chatLineRepository.Get(id));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ChatLine value)
        {
            return Ok(await _chatLineRepository.Add(value));
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] ChatLine value)
        {
            return Ok(await _chatLineRepository.Update(value));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return Ok(await _chatLineRepository.Delete(id));
        }
    }
}
