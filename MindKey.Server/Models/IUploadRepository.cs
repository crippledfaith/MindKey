using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Server.Models
{
    public interface IUploadRepository
    {
        PagedResult<Upload> GetUploads(string? name, int page);
        Task<Upload?> GetUpload(long Id);
        Task<Upload> AddUpload(Upload upload);
        Task<Upload?> UpdateUpload(Upload upload);
        Task<Upload?> DeleteUpload(long id);
    }
}