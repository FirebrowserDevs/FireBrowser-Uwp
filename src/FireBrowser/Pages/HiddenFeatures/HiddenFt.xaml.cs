using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using muxc = Microsoft.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.HiddenFeatures
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HiddenFt : Page
    {
        public HiddenFt()
        {
            this.InitializeComponent();
            Stack();
        }

        public void Stack()
        {
            HiddenTb1.IsOn = test1 == "0x1";
            HiddenTb2.IsOn = test2 == "0x1";
        }

        string test1 = FireBrowserInterop.SettingsHelper.GetSetting("DragOutSideExperiment");
        string test2 = FireBrowserInterop.SettingsHelper.GetSetting("VaultExperiment");

        private Passer passer;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as Passer;
            passer.Tab.Header = "FireBrowser Experimental";
            passer.Tab.IconSource = new muxc.FontIconSource()
            {
                Glyph = "\uEC7A"
            };
        }

        private void HiddenTb1_Toggled(object sender, RoutedEventArgs e)
        {
            if (HiddenTb1.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DragOutSideExperiment", "0x1");
            }
            else
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DragOutSideExperiment", "0x0");
            }
        }

        private void HiddenTb2_Toggled(object sender, RoutedEventArgs e)
        {
            if (HiddenTb2.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("VaultExperiment", "0x1");
            }
            else
            {
                FireBrowserInterop.SettingsHelper.SetSetting("VaultExperiment", "0x0");
            }
        }
    }
}
