using Microsoft.AspNetCore.Mvc;
using MindKey.Server.Authorization;
using MindKey.Server.Models;
using MindKey.Shared.Models;

namespace MindKey.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;

        public PersonController(IPersonRepository personRepository, IUserRepository userRepository)
        {
            _personRepository = personRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns a list of paginated people with a default page size of 5.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetPeople([FromQuery] string? name, int page)
        {
            return Ok(_personRepository.GetPeople(name, page));
        }

        /// <summary>
        /// Gets a specific person by Id.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPerson(long id)
        {
            return Ok(await _personRepository.GetPerson(id));
        }
        [AllowAnonymous]
        [HttpGet("byUser/{id}")]
        public async Task<ActionResult> GetPersonByUser(long id)
        {
            return Ok(await _personRepository.GetPersonByUser(id));
        }
        /// <summary>
        /// Creates a person with child addresses.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddPerson(Person person)
        {
            return Ok(await _personRepository.AddPerson(person));
        }

        /// <summary>
        /// Updates a person with a specific Id.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult> UpdatePerson(Person person)
        {
            person.User.FirstName = person.FirstName;
            person.User.LastName = person.LastName;
            await _userRepository.UpdateUser(person.User);
            return Ok(await _personRepository.UpdatePerson(person));
        }

        /// <summary>
        /// Deletes a person with a specific Id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePerson(long id)
        {
            return Ok(await _personRepository.DeletePerson(id));
        }
    }
}
