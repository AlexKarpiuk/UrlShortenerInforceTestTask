using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortenerInforceTestTask.Models
{
    public class Urls
    {
        public int Id { get; set; }

        [ForeignKey("AppUser")]
        public int UserId { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public AppUser? AppUser { get; set; }
    }
}
