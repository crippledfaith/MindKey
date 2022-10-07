using MindKey.Shared.Models.MindKey;

namespace MindKey.Client.Services
{
    public delegate void LoginAndOutEventHandler(bool isLoggedIn);
    public delegate void EditIdeaEventHandler(Idea idea);
    public delegate void DeleteIdeaEventHandler(Idea idea);

    public class EventService
    {
        public event LoginAndOutEventHandler LoginAndOutEvent;
        public event EditIdeaEventHandler EditIdeaEvent;
        public event DeleteIdeaEventHandler DeleteIdeaEvent;

        public void ChangeLoginStatus(bool isLoggedIn)
        {
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
    }
}
