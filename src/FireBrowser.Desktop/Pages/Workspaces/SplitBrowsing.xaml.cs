using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Workspaces
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class SplitBrowsing : Page
    {
        public SplitBrowsing()
        {
            this.InitializeComponent();
        }
        private Passer Passer { get; set; }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                Passer = e.Parameter as Passer;
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.TryGetValue("AirplaneDesktop_Tab", out object obj))
            {
                var item = (obj as TabViewItem);
                // Ensure that the obj property is set before continuing. 
                if (obj == null)
                {
                    return;
                }
                Passer.TabView.TabItems.Remove(item);
                TestingFrame.Children.Add(item.Content as UIElement);
                TestingFrame.Children.Remove(WelcomeFrame);
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey("AirplaneDesktop_Tab"))
            {
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var frame = new Frame();
            var passer = Passer;
            passer.Param = new Uri("https://google.com");
            //Temporarily WebContent because new tab is missing shortcuts in the current release
            frame.Navigate(typeof(WebContent), passer);
            TestingFrame.Children.Add(frame);
        }
    }
}
