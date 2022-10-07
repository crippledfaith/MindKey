using Microsoft.AspNetCore.Components;
using MindKey.Client.Shared;
using MindKey.Shared.Data;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Client.Services
{
    public class IdeaService : IIdeaService
    {
        private IHttpService _httpService;
        private ILocalStorageService _localStorageService;
        private NavigationManager _navigationManager;
        private string _userKey = "user";


        public IdeaService(IHttpService httpService, ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _httpService = httpService;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }


        public async Task<PagedResult<Idea>> GetIdeas(string page, long? userId)
        {
            return await _httpService.Get<PagedResult<Idea>>("api/idea" + "?page=" + page + "&userId=" + userId);
        }
        public async Task<PagedResult<Idea>> GetIdeasOfOthers(string page, long? userId)
        {
            return await _httpService.Get<PagedResult<Idea>>("api/idea/others" + "?page=" + page + "&userId=" + userId);
        }
        public async Task<Idea> GetIdea(long id)
        {
            return await _httpService.Get<Idea>($"api/idea/{id}");
        }

        public async Task DeleteIdea(long id)
        {
            await _httpService.Delete($"api/idea/{id}");
        }

        public async Task AddIdea(Idea idea)
        {
            await _httpService.Post($"api/idea", idea);
        }

        public async Task UpdateIdea(Idea idea)
        {
            await _httpService.Put($"api/user", idea);

        }
    }
}