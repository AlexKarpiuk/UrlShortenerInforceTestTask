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
        public bool Add(Urls url)
        {
            _context.Add(url);
            return Save();
        }

        public bool Delete(Urls url)
        {
            _context.Add(url);
            return Save();
        }

        public async Task<IEnumerable<Urls>> GetAllUrls()
        {
            return await _context.Urls.ToListAsync();
        }

        public async Task<Urls> GetUrlById(string id)
        {
            return await _context.Urls.FindAsync(id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Urls url)
        {
            _context.Update(url);
            return Save();
        }
    }
}
