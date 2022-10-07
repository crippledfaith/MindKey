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
        Task<Idea?> UpdateIdea(Idea idea);
    }
}