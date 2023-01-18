//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.FirstLaunch
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class DataSwitchPage : Page
    {
        public static FirstLaunchPageBase PageData = new FirstLaunchPageBase()
        {
            PageContent = typeof(DataSwitchPage),
            PageID = "ExamplePage",
            PageTitleResource = "ExamplePage",
            CanContinue = true,
        };
        public DataSwitchPage()
        {
            this.InitializeComponent();
        }
    }
}
