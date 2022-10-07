using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Client.Services
{
    public interface IUploadService
    {
        Task<PagedResult<Upload>> GetUploads(string name, string page);
        Task<Upload> GetUpload(long id);

        Task DeleteUpload(long id);

        Task AddUpload(Upload upload);

        Task UpdateUpload(Upload upload);
    }
}