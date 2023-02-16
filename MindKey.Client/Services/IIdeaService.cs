using MindKey.Shared.Data;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Client.Services
{
    public interface IIdeaService
    {
        Task<Idea> AddIdea(Idea idea);
        Task DeleteIdea(long id);
        Task<Idea> GetIdea(long id);
        Task<PagedResult<Idea>?> GetIdeas(string page, long? userId);
        Task<PagedResult<Idea>?> GetTopIdeas(string page);
        Task<PagedResult<Idea>?> GetIdeasOfOthers(string page, long? userId);
        Task<bool> SetComment(IdeaUserComment ideaUserComment);
        Task UpdateIdea(Idea idea);
        Task<IdeaUserComment?> GetComment(IdeaUserComment ideaUserComment);
        Task<PagedResult<IdeaUserComment>> GetComments(Idea idea, string page, string pageSize = "5");
        Task<Dictionary<string, int>> GetTags(int count);

        Task<WorkCloudResult> GetWordCloud(WorkCloudParameter param);
    }
}