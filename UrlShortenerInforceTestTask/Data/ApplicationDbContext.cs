using Microsoft.EntityFrameworkCore;
using UrlShortenerInforceTestTask.Models;

namespace UrlShortenerInforceTestTask.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Url> Urls { get; set; }

    }
}
