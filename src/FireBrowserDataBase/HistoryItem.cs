using Windows.UI.Xaml.Media.Imaging;

namespace FireBrowserDataBase
{
    public class HistoryItem
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public int VisitCount { get; set; }
        public string LastVisitTime { get; set; }
        public BitmapImage ImageSource { get; set; }
    }
}
