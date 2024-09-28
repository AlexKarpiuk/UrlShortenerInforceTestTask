namespace UrlShortenerInforceTestTask.ViewModels
{
    public class EditUrlViewModel
    {
        public string OriginalUrl { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
