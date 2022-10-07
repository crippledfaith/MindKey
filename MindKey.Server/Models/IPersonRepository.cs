using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Server.Models
{
    public interface IPersonRepository
    {
        PagedResult<Person> GetPeople(string? name, int page);
        Task<Person?> GetPerson(long personId);
        Task<Person?> GetPersonByUser(long userid);
        Task<Person> AddPerson(Person person);
        Task<Person?> UpdatePerson(Person person);
        Task<Person?> DeletePerson(long personId);
    }
}