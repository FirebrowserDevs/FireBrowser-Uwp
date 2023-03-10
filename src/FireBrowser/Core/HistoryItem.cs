using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace FireBrowser.Core
{
    public class HistoryItem
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public int VisitCount { get; set; }
        public DateTime LastVisitTime { get; set; }
        public BitmapImage ImageSource { get; set; }
    }
}
