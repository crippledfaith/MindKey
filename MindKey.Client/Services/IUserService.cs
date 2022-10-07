using MindKey.Client.Shared;
using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Client.Services
{
    public interface IUserService
    {
        User User { get; }

        Task Initialize();
        Task Login(Login model);
        Task Logout();
        Task<PagedResult<User>> GetIdeas(string name, string page);
        Task<User> GetUser(long id);
        Task DeleteIdea(long id);
        Task AddIdea(User user);
        Task UpdateUser(User user);
    }
}