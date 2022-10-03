using BlazorChat.Shared.Data;
using BlazorChat.Shared.Models;

namespace BlazorChat.Client.Services
{
    public interface IUploadService
    {
        Task<PagedResult<Upload>> GetUploads(string name, string page);
        Task<Upload> GetUpload(int id);

        Task DeleteUpload(int id);

        Task AddUpload(Upload upload);

        Task UpdateUpload(Upload upload);
    }
}