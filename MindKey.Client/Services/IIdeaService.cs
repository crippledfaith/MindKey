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
        Task UpdateIdea(Idea idea);
    }
}