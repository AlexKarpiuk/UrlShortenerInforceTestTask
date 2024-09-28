using UrlShortenerInforceTestTask.Models;

namespace UrlShortenerInforceTestTask.Interfaces
{
    public interface IUrlsRepository
    {
        Task<IEnumerable<Urls>> GetAllUrls();
        Task<Urls> GetUrlById(string id);
        bool Add(Urls url);
        bool Update(Urls url);
        bool Delete(Urls url);
        bool Save();
    }
}
