using FireBrowser.Core;
using FireBrowser.Pages.FirstLaunch;
using Dots.SDK;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Workspaces
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class FreeformBrowsing : Page
    {
        public FreeformBrowsing()
        {
            this.InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Set XAML element as a drag region.
            Window.Current.SetTitleBar(AppTitleBar);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dots.SDK.UWP.Log.WriteLog(canvas.ExportAsJson());
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LeaveCanvasMode.IsSelected)
            {
                //To-Do: animation and improve this terrible code
                Frame.Navigate(typeof(FirstLaunchPage), null);
            }
        }
    }
}
