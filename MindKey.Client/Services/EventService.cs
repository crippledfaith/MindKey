namespace MindKey.Client.Services
{
    public delegate void LoginAndOutEventHandler(bool isLoggedIn);

    public class EventService
    {
        public event LoginAndOutEventHandler LoginAndOutEvent;

        public void ChangeLoginStatus(bool isLoggedIn)
        {
            LoginAndOutEvent?.Invoke(isLoggedIn);
        }
    }
}
