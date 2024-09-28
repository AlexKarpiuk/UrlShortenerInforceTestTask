using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortenerInforceTestTask.ViewModels
{
    public class CreateUrlViewModel
    {
        public int UserId { get; set; }
        public string OriginalUrl { get; set; }
    }
}
