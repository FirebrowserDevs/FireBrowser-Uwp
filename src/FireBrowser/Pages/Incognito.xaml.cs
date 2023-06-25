using FireBrowser.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Incognito : Page
    {
        public Incognito()
        {
            this.InitializeComponent();
           
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UseContent.MainPageContent.Fav.IsEnabled = false;
            UseContent.MainPageContent.His.IsEnabled = false;
        }
    }
}
