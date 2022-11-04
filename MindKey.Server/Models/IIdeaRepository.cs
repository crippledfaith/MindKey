using MindKey.Shared.Data;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Server.Models
{
    public interface IIdeaRepository
    {
        Task<Idea?> AddIdea(Idea person);
        Task<Idea?> DeleteIdea(long id);
        Task<Idea?> GetIdea(long id);
        PagedResult<Idea>? GetIdeas(int page, long? userId);
        PagedResult<Idea>? GetIdeasOfOthers(int page, long? userId);
        Task<bool?> SetArgument(IdeaUserComment ideaUserComment);
        Task<Idea?> UpdateIdea(Idea idea);
        Task<IdeaUserComment?> GetSetAgument(IdeaUserComment ideaUserComment);
        PagedResult<IdeaUserComment> GetComments(int page, int pageSize, long? ideaId);

        Task<Dictionary<string, int>> GetTags(int count);
    }
}