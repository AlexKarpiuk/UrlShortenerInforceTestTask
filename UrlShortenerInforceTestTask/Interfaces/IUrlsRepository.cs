using System.Diagnostics;
using UrlShortenerInforceTestTask.Models;

namespace UrlShortenerInforceTestTask.Interfaces
{
    public interface IUrlsRepository
    {
        Task<IEnumerable<Url>> GetAllUrls();
        Task<Url> GetUrlById(int id);
        Task<Url> GetUrlByIdNoTrackingAsync(int id);
        bool Add(Url url);
        bool Update(Url url);
        bool Delete(Url url);
        bool Save();
    }
}
