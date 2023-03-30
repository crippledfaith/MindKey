using Microsoft.AspNetCore.SignalR;
using MindKey.Shared.Models;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Server.Helpers
{
    public class BroadcastHub : Hub
    {
        public async Task SendChatMessage(ChatLine chatLine)
        {
            await Clients.All.SendAsync("ReceiveChatMessage", chatLine);
        }

        public async Task SendCommentUpdatedMessage(Idea idea)
        {
            await Clients.All.SendAsync("ReceiveCommentUpdatedMessage", idea);
        }
    }
}
