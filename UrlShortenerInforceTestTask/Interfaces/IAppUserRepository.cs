using UrlShortenerInforceTestTask.Models;

namespace UrlShortenerInforceTestTask.Interfaces
{
    public interface IAppUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> FindUserByEmailAsync(string email);
        bool Add(AppUser user);
        bool Update(AppUser user);
        bool Delete(AppUser user);
        bool Save();
    }
}
