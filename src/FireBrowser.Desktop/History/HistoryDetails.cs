using Windows.UI.Xaml.Media.Imaging;

namespace FireBrowser.History
{
    public class HistoryDetails
    {
        public string Title { get; set; }
        public string Url { get; set; }  
        public BitmapImage ImageSource { get; set; }

        public DateTime Date { get; set; }
    }
}
