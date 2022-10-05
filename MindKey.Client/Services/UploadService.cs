using MindKey.Client.Shared;
using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Client.Services
{
    public class UploadService : IUploadService
    {
        private readonly IHttpService _httpService;

        public UploadService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<PagedResult<Upload>> GetUploads(string name, string page)
        {
            return await _httpService.Get<PagedResult<Upload>>("api/upload" + "?page=" + page + "&name=" + name);
        }

        public async Task<Upload> GetUpload(int id)
        {
            return await _httpService.Get<Upload>($"api/upload/{id}");
        }

        public async Task DeleteUpload(int id)
        {
            await _httpService.Delete($"api/upload/{id}");
        }

        public async Task AddUpload(Upload upload)
        {
            await _httpService.Post($"api/upload", upload);
        }

        public async Task UpdateUpload(Upload upload)
        {
            await _httpService.Put($"api/upload", upload);
        }
    }
}