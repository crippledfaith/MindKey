using MindKey.Shared.Data;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Client.Services
{
    public interface IIdeaService
    {
        Task AddIdea(Idea idea);
        Task DeleteIdea(long id);
        Task<Idea> GetIdea(long id);
        Task<PagedResult<Idea>> GetIdeas(string page, long? userId);
        Task<PagedResult<Idea>> GetIdeasOfOthers(string page, long? userId);
        Task<bool> SetAurgument(IdeaUserComment ideaUserComment);
        Task UpdateIdea(Idea idea);
        Task<IdeaUserComment?> GetSetAgument(IdeaUserComment ideaUserComment);
        Task<PagedResult<IdeaUserComment>> GetComments(Idea idea, string page, string pageSize = "5");
    }
}