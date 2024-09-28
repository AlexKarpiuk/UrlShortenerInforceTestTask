using Microsoft.EntityFrameworkCore;
using UrlShortenerInforceTestTask.Data;
using UrlShortenerInforceTestTask.Interfaces;
using UrlShortenerInforceTestTask.Models;

namespace UrlShortenerInforceTestTask.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly ApplicationDbContext _context;

        public AppUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(AppUser user)
        {
            _context.Add(user);
            return Save();
        }

        public bool Delete(AppUser user)
        {
            _context.Remove(user);
            return Save();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _context.AppUser.ToListAsync();
        }

        public async Task<AppUser> GetUserById(int id)
        {
            return await _context.AppUser.FindAsync(id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}
