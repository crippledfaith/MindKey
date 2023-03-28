using MindKey.Client.Shared;
using MindKey.Shared.Data;
using MindKey.Shared.Models;
using MindKey.Shared.Models.MindKey;
using System.Xml.Linq;

namespace MindKey.Client.Services
{
    public class ChatLinesService : IChatLinesService
    {
        private readonly IHttpService _httpService;
        private readonly IUserService _userService;


        public ChatLinesService(IHttpService httpService, IUserService userService)
        {
            _httpService = httpService;
            _userService = userService;
        }



        public async Task<ChatLine?> Get(long id)
        {
            return await _httpService.Get<ChatLine>("api/ChatLines" + "?id" + id);
        }

        public async Task<List<ChatLine>?> GetByIdea(long ideaId)
        {
            return await _httpService.Get<List<ChatLine>?>("api/ChatLines/GetByIdea" + "?ideaId=" + ideaId);
        }

        public async Task<PagedResult<ChatLine>?> GetPaged(int page)
        {
            return await _httpService.Get<PagedResult<ChatLine>?>("api/ChatLines/GetPaged" + "?page=" + page);
        }

        public async Task<PagedResult<ChatLine>?> GetPagedByIdea(int page, long ideaId)
        {
            return await _httpService.Get<PagedResult<ChatLine>?>("api/ChatLines/GetPagedByIdea" + "?page=" + page + "&ideaId=" + ideaId);
        }

        public async Task<ChatLine?> Add(ChatLine chatLine)
        {
            return await _httpService.Post<ChatLine>("api/ChatLines", chatLine);
        }

        public async Task<ChatLine?> Update(ChatLine chatLine)
        {
            return await _httpService.Put<ChatLine>("api/ChatLines", chatLine);
        }


        public async Task<ChatLine?> Delete(long id)
        {
            return await _httpService.Delete<ChatLine>("api/ChatLines/" + id);
        }


    }
}
