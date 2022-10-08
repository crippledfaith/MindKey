using MindKey.Client.Shared;
using MindKey.Shared.Data;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Client.Services
{
    public class IdeaService : IIdeaService
    {
        private readonly IHttpService _httpService;
        private readonly IUserService _userService;


        public IdeaService(IHttpService httpService, IUserService userService)
        {
            _httpService = httpService;
            _userService = userService;
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
            await _httpService.Put($"api/idea", idea);
        }

        public async Task<bool> SetAurgument(IdeaUserComment ideaUserComment)
        {
            return await _httpService.Post<bool>($"api/idea/SetArgument", ideaUserComment);
        }

        public async Task<IdeaUserComment?> GetSetAgument(IdeaUserComment ideaUserComment)
        {
            return await _httpService.Post<IdeaUserComment?>($"api/idea/GetSetAgument", ideaUserComment);
        }
    }
}