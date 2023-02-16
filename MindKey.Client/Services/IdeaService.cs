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


        public async Task<PagedResult<Idea>?> GetIdeas(string page, long? userId)
        {
            return await _httpService.Get<PagedResult<Idea>>("api/idea" + "?page=" + page + "&userId=" + userId);
        }
        public async Task<PagedResult<Idea>?> GetTopIdeas(string page)
        {
            return await _httpService.Get<PagedResult<Idea>?>("api/idea/top" + "?page=" + page);
        }
        public async Task<PagedResult<Idea>?> GetIdeasOfOthers(string page, long? userId)
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

        public async Task<Idea> AddIdea(Idea idea)
        {
            return await _httpService.Post<Idea>($"api/idea", idea);
        }

        public async Task UpdateIdea(Idea idea)
        {
            await _httpService.Put($"api/idea", idea);
        }

        public async Task<bool> SetComment(IdeaUserComment ideaUserComment)
        {
            return await _httpService.Post<bool>($"api/idea/SetComment", ideaUserComment);
        }

        public async Task<IdeaUserComment?> GetComment(IdeaUserComment ideaUserComment)
        {
            return await _httpService.Post<IdeaUserComment?>($"api/idea/GetComment", ideaUserComment);
        }

        public async Task<PagedResult<IdeaUserComment>> GetComments(Idea idea, string page, string pageSize = "5")
        {
            return await _httpService.Get<PagedResult<IdeaUserComment>>($"api/idea/GetComments?page={page}&pagesize={pageSize}&ideaId={idea.Id}");
        }

        public async Task<Dictionary<string, int>> GetTags(int count)
        {
            return await _httpService.Get<Dictionary<string, int>>($"api/idea/GetTags?count={count}");
        }

        public async Task<WorkCloudResult> GetWordCloud(WorkCloudParameter param)
        {
            return await _httpService.Post<WorkCloudResult>($"api/idea/GetWordCloud", param);
        }

    }
}