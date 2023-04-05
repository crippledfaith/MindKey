using Microsoft.AspNetCore.Components;
using MindKey.Client.Shared;
using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Client.Services
{
    public class UserService : IUserService
    {
        private IHttpService _httpService;
        private ILocalStorageService _localStorageService;
        private IPersonService _personService;
        private NavigationManager _navigationManager;
        private EventService _eventService;
        private string _userKey = "user";
        private string _userTokenKey = "userToken";
        public string ProfilePicture { get; set; }
        public User? User { get; private set; }
        public Person? CurrentPerson { get; private set; }
        public string? IdeaId { get; set; }


        public UserService(IHttpService httpService, ILocalStorageService localStorageService, NavigationManager navigationManager, EventService eventService,IPersonService personService)
        {
            _httpService = httpService;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
            _eventService = eventService;
            _personService = personService;
            _ = Initialize();
        }

        public async Task Initialize()
        {

            var userId = await _localStorageService.GetItem<long?>(_userKey);
            if (userId != null)
            {
                User = await GetUser(userId.Value);
                if (User != null)
                {
                    CurrentPerson = await _personService.GetPersonByUser(User.Id);
                    ProfilePicture = $"data:image/png;base64,{CurrentPerson.ProfilePicture}";
                }
            }
            _eventService.ChangeLoginStatus(User != null);

        }
        public async Task UpdateInformation()
        {
            if (User != null)
            {
                User = await GetUser(User.Id);
                CurrentPerson = await _personService.GetPersonByUser(User.Id);
                ProfilePicture = $"data:image/png;base64,{CurrentPerson.ProfilePicture}";
                _eventService.ChangeLoginStatus(true);
            }
        }
        public async Task Login(Login model)
        {
            User = await _httpService.Post<User>("/api/user/authenticate", model);
            await _localStorageService.SetItem(_userKey, User.Id);
            await _localStorageService.SetItem(_userTokenKey, User.Token);
            if (User != null)
            {
                User = await GetUser(User.Id);
                if (User != null)
                {
                    CurrentPerson = await _personService.GetPersonByUser(User.Id);
                    ProfilePicture = $"data:image/png;base64,{CurrentPerson.ProfilePicture}";
                }
            }
            _eventService.ChangeLoginStatus(User != null);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem(_userKey);
            _navigationManager.NavigateTo("/user/login");
        }

        public async Task<PagedResult<User>?> GetUsers(string name, string page)
        {
            return await _httpService.Get<PagedResult<User>>("api/user" + "?page=" + page + "&name=" + name);
        }

        public async Task<User?> GetUser(long id)
        {
            try
            {
                return await _httpService.Get<User>($"api/user/{id}");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task DeleteUser(long id)
        {
            await _httpService.Delete($"api/user/{id}");
            // auto logout if the user deleted their own record
            if (User != null)
            {
                if (id == User.Id)
                    await Logout();
            }

        }

        public async Task AddUser(User user)
        {
            await _httpService.Post($"api/user", user);
        }

        public async Task UpdateUser(User user)
        {

            await _httpService.Put($"api/user", user);
            // update local storage if the user updated their own record
            if (user.Id == User?.Id)
            {
                User.FirstName = user.FirstName;
                User.LastName = user.LastName;
                User.Username = user.Username;
                CurrentPerson.FirstName = user.FirstName;
                CurrentPerson.LastName = user.LastName;
                await _localStorageService.SetItem(_userKey, User);
            }
        }
    }
}