using FireBrowser.Core;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Media.Casting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class TimelinePage : Page
    {
        public TimelinePage()
        {
            this.InitializeComponent();
        }
    }
}
