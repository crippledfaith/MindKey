using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Client.Services
{
    public interface IChatLinesService
    {
        Task<PagedResult<ChatLine>> GetPaged(int page);
        Task<PagedResult<ChatLine>> GetPagedByIdea(int page, long ideaId);
        Task<List<ChatLine?>> GetByIdea(long ideaId);
        Task<ChatLine?> Get(long id);
        Task<ChatLine> Add(ChatLine chatLine);
        Task<ChatLine> Update(ChatLine chatLine);
        Task<ChatLine> Delete(long id);
    }
}
