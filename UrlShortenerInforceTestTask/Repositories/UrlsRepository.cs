using Microsoft.EntityFrameworkCore;
using UrlShortenerInforceTestTask.Data;
using UrlShortenerInforceTestTask.Interfaces;
using UrlShortenerInforceTestTask.Models;

namespace UrlShortenerInforceTestTask.Repositories
{
    public class UrlsRepository : IUrlsRepository
    {
        private readonly ApplicationDbContext _context;

        public UrlsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Url url)
        {
            _context.Add(url);
            return Save();
        }

        public bool Delete(Url url)
        {
            _context.Remove(url);
            return Save();
        }

        public async Task<IEnumerable<Url>> GetAllUrls()
        {
            return await _context.Urls.ToListAsync();
        }

        public async Task<Url> GetUrlById(int id)
        {
            return await _context.Urls.FindAsync(id);
        }

        public async Task<Url> GetUrlByIdNoTrackingAsync(int id)
        {
            return await _context.Urls.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Url url)
        {
            _context.Update(url);
            return Save();
        }
    }
}
