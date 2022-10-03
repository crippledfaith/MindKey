using BlazorChat.Client.Shared;
using BlazorChat.Shared.Data;
using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorChat.Client.Services
{
    public class PersonService : IPersonService
    {
        private IHttpService _httpService;

        public PersonService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<PagedResult<Person>> GetPeople(string name, string page)
        {
            return await _httpService.Get<PagedResult<Person>>("api/person" + "?page=" + page + "&name=" + name);
        }

        public async Task<Person> GetPerson(int id)
        {
            return await _httpService.Get<Person>($"api/person/{id}");
        }

        public async Task DeletePerson(int id)
        {
            await _httpService.Delete($"api/person/{id}");
        }

        public async Task AddPerson(Person person)
        {
            await _httpService.Post($"api/person", person);
        }

        public async Task UpdatePerson(Person person)
        {
            await _httpService.Put($"api/person", person);
        }
    }
}