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

        public event LoginAndOutEventHandler? LoginAndOutEvent;
        public event EditIdeaEventHandler? EditIdeaEvent;
        public event LastIdeaUpdatedEventHandler? LastIdeaUpdatedEvent;
        public event DeleteIdeaEventHandler? DeleteIdeaEvent;
        public event DeletedIdeaEventHandler? DeletedIdeaEvent;
        public event HideAllStoriesEventHandler? HideAllStoriesEvent;
        public event LoadingStatusChanagedEventHandler? LoadingStatusChanagedEvent;
        public event ShowMessageEventHandler? ShowMessageEvent;
        public event AgreeDisagreeChangedEventHandler? AgreeDisagreeChangedEvent;
        public EventService(ILogger<EventService> logger)
        {
            Logger = logger;
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

    }
}
