using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MindKey.Shared.Models;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Client.Services
{
    public delegate void LoginAndOutEventHandler(bool isLoggedIn);
    public delegate void EditIdeaEventHandler(Idea idea);
    public delegate void DeleteIdeaEventHandler(Idea idea);
    public delegate void DeletedIdeaEventHandler(Idea idea);
    public delegate void LastIdeaUpdatedEventHandler(string ideaId);
    public delegate void HideAllStoriesEventHandler(Idea idea);
    public delegate void LoadingStatusChanagedEventHandler(bool loading);
    public delegate void ShowMessageEventHandler(string message, MessageType type);
    public delegate void AgreeDisagreeChangedEventHandler(Idea idea);
    public delegate void ReceiveCommentUpdatedMessageEventHandler(Idea idea);
    public delegate void ReceiveChatMessageEventHandler(ChatLine chatLine);
    public enum MessageType
    {
        Error = 0,
        Info = 1,
        Warning = 2,
        Success = 3,
    }
    public class EventService
    {
        public static int LoadingCount = 0;
        public static bool Loading = false;

        public ILogger<EventService> Logger { get; }
        private HubConnection hubConnection;


        public event LoginAndOutEventHandler? LoginAndOutEvent;
        public event EditIdeaEventHandler? EditIdeaEvent;
        public event LastIdeaUpdatedEventHandler? LastIdeaUpdatedEvent;
        public event DeleteIdeaEventHandler? DeleteIdeaEvent;
        public event DeletedIdeaEventHandler? DeletedIdeaEvent;
        public event HideAllStoriesEventHandler? HideAllStoriesEvent;
        public event LoadingStatusChanagedEventHandler? LoadingStatusChanagedEvent;
        public event ShowMessageEventHandler? ShowMessageEvent;
        public event AgreeDisagreeChangedEventHandler? AgreeDisagreeChangedEvent;
        public event ReceiveCommentUpdatedMessageEventHandler? ReceiveCommentUpdatedMessageEvent;
        public event ReceiveChatMessageEventHandler? ReceiveChatMessageEvent;

        public EventService(ILogger<EventService> logger, NavigationManager navigationManager)
        {
            Logger = logger;
            hubConnection = new HubConnectionBuilder().WithUrl(navigationManager.ToAbsoluteUri("/broadcastHub")).Build();
            if (hubConnection != null)
            {
                hubConnection.On("ReceiveCommentUpdatedMessage", async (Idea idea) =>
                {
                    ReceiveCommentUpdatedMessageEvent?.Invoke(idea);
                });
                hubConnection.On("ReceiveChatMessage", (ChatLine chatLine) =>
                {
                    ReceiveChatMessageEvent?.Invoke(chatLine);
                });
                hubConnection.StartAsync();
            }

        }
        public void ChangeLoginStatus(bool isLoggedIn)
        {
            ResetLoader();
            LoginAndOutEvent?.Invoke(isLoggedIn);
        }
        public void EditIdea(Idea idea)
        {
            EditIdeaEvent?.Invoke(idea);
        }
        public void DeleteIdea(Idea idea)
        {
            DeleteIdeaEvent?.Invoke(idea);
        }
        public void DeletedIdea(Idea idea)
        {
            DeletedIdeaEvent?.Invoke(idea);
        }
        public void HideAllStories(Idea idea)
        {
            HideAllStoriesEvent?.Invoke(idea);
        }
        public void ResetLoader()
        {
            Loading = false;
            LoadingCount = 0;
            LoadingStatusChanagedEvent?.Invoke(false);
        }
        public void ChangeLoadingStatus(bool loading, string location)
        {
            if (!Loading && loading)
            {
                LoadingCount = 1;
                Loading = true;

            }
            else if (Loading && loading)
            {
                LoadingCount++;
                Loading = true;
                //if (LoadingCount > 0)
                //{
                //    LoadingStatusChanagedEvent?.Invoke(true);
                //}
            }
            else if (Loading && !loading)
            {
                LoadingCount--;
                if (LoadingCount == 0)
                {
                    Loading = false;
                }
                else if (LoadingCount < 0)
                {
                    Loading = false;
                    LoadingCount = 0;
                }
            }

            LoadingStatusChanagedEvent?.Invoke(Loading);
            Logger.LogInformation($"{loading} - Loading: {Loading} Count: {LoadingCount} - {location}");

        }
        public void ShowMessage(string message, MessageType type)
        {
            ShowMessageEvent?.Invoke(message, type);
        }
        public void AgreeDisagreeChanged(Idea idea)
        {
            AgreeDisagreeChangedEvent?.Invoke(idea);
        }
        public void LastIdeaUpdatedEventChanged(string idea)
        {
            LastIdeaUpdatedEvent?.Invoke(idea);
        }
        public async Task SendCommentUpdatedMessage(Idea Idea)
        {
            if (IsConnected == true)
                await hubConnection.SendAsync("SendCommentUpdatedMessage", Idea);
        }
        public async Task SendChatMessage(ChatLine chatLine)
        {
            if (IsConnected == true)
                await hubConnection.SendAsync("SendChatMessage", chatLine);
        }
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
    }
}
