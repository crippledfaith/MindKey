using Microsoft.AspNetCore.SignalR;
using MindKey.Shared.Models;

namespace MindKey.Server.Helpers
{
    public class BroadcastHub : Hub
    {
        public async Task SendChatMessage(ChatLine chatLine)
        {
            await Clients.All.SendAsync("ReceiveChatMessage", chatLine);
        }
    }
}
