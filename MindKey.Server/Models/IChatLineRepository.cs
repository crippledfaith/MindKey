using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Server.Models
{
    public interface IChatLineRepository
    {
        PagedResult<ChatLine> GetPaged(int page);
        PagedResult<ChatLine> GetPagedByIdea(int page,long ideaId);
        Task<IEnumerable<ChatLine>?> GetByIdea(long ideaId);
        Task<ChatLine?> Get(long id);
        Task<ChatLine> Add(ChatLine chatLine);
        Task<ChatLine> Update(ChatLine chatLine);
        Task<ChatLine> Delete(ChatLine chatLine);
        Task<ChatLine> Delete(long id);
    }
}
