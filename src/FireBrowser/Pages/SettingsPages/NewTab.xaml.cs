using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTab : Page
    {
        public NewTab()
        {
            this.InitializeComponent();
            ButtonVisible();
        }

        private void SearchengineSelection_Loaded(object sender, RoutedEventArgs e)
        {
            string selection = FireBrowserInterop.SettingsHelper.GetSetting("EngineFriendlyName");
            SearchengineSelection.PlaceholderText = selection ?? "Google";
        }

        private void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            if (selection == "Ask") SetEngine("Ask", "https://www.ask.com/web?q=");
            if (selection == "Baidu") SetEngine("Baidu", "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=");
            if (selection == "Bing") SetEngine("Bing", "https://www.bing.com?q=");
            if (selection == "DuckDuckGo") SetEngine("DuckDuckGo", "https://www.duckduckgo.com?q=");
            if (selection == "Ecosia") SetEngine("Ecosia", "https://www.ecosia.org/search?q=");
            if (selection == "Google") SetEngine("Google", "https://www.google.com/search?q=");
            if (selection == "Startpage") SetEngine("Startpage", "https://www.startpage.com/search?q=");
            if (selection == "Qwant") SetEngine("Qwant", "https://www.qwant.com/?q=");
            if (selection == "Qwant Lite") SetEngine("Qwant Lite", "https://lite.qwant.com/?q=");
            if (selection == "Yahoo!") SetEngine("Yahoo!", "https://search.yahoo.com/search?p=");
            if (selection == "Presearch") SetEngine("Presearch", "https://presearch.com/search?q=");
        }

        private void SetEngine(string EngineFriendlyName, string SearchUrl)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("EngineFriendlyName", EngineFriendlyName);
            FireBrowserInterop.SettingsHelper.SetSetting("SearchUrl", SearchUrl);
        }

        #region buttons

        public string ReadButton = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("Readbutton")) ? FireBrowserInterop.SettingsHelper.GetSetting("Readbutton") : "True";
        public string AdblockBtn = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("AdBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("AdBtn") : "True";
        public string Downloads = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("DwBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("DwBtn") : "True";
        public string Translate = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("TransBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("TransBtn") : "True";
        public string Favorites = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("FavBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("FavBtn") : "True";
        public string Historybtn = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("HisBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("HisBtn") : "True";
        public string QrCode = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("QrBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("QrBtn") : "True";
        public string FavBtn = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("FlBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("FlBtn") : "True";
        public string ToolB = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("ToolBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("ToolBtn") : "True";
        public string DarkTog = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("DarkBtn")) ? FireBrowserInterop.SettingsHelper.GetSetting("DarkBtn") : "True";
        public string OpTog = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("OpSw")) ? FireBrowserInterop.SettingsHelper.GetSetting("OpSw") : "True";

        public void ButtonVisible()
        {
            Read.IsOn = ReadButton switch
            {
                "True" => true,
                "0" => false,
                _ => Read.IsOn
            };

            Adbl.IsOn = AdblockBtn switch
            {
                "True" => true,
                "0" => false,
                _ => Adbl.IsOn
            };

            Dwbl.IsOn = Downloads switch
            {
                "True" => true,
                "0" => false,
                _ => Dwbl.IsOn
            };

            Trbl.IsOn = Translate switch
            {
                "True" => true,
                "0" => false,
                _ => Trbl.IsOn
            };

            Frbl.IsOn = Favorites switch
            {
                "True" => true,
                "0" => false,
                _ => Frbl.IsOn
            };

            Hsbl.IsOn = Historybtn switch
            {
                "True" => true,
                "0" => false,
                _ => Hsbl.IsOn
            };

            Qrbl.IsOn = QrCode switch
            {
                "True" => true,
                "0" => false,
                _ => Qrbl.IsOn
            };
            FlAd.IsOn = FavBtn switch
            {
                "True" => true,
                "0" => false,
                _ => FlAd.IsOn
            };
            Tlbl.IsOn = ToolB switch
            {
                "True" => true,
                "0" => false,
                _ => Tlbl.IsOn
            };
            Drbl.IsOn = DarkTog switch
            {
                "True" => true,
                "0" => false,
                _ => Drbl.IsOn
            };

            OpenNew.IsOn = OpTog switch
            {
                "True" => true,
                "0" => false,
                _ => OpenNew.IsOn
            };
        }

        #endregion
        private void Read_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("Readbutton", Read.IsOn ? "True" : "0");
        }

        private void Adbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("AdBtn", Adbl.IsOn ? "True" : "0");
        }

        private void Dwbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("DwBtn", Dwbl.IsOn ? "True" : "0");
        }

        private void Trbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("TransBtn", Trbl.IsOn ? "True" : "0");
        }

        private void Frbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("FavBtn", Frbl.IsOn ? "True" : "0");
        }

        private void Hsbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("HisBtn", Hsbl.IsOn ? "True" : "0");
        }

        private void Qrbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("QrBtn", Qrbl.IsOn ? "True" : "0");
        }

        private void FlAd_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("FlBtn", FlAd.IsOn ? "True" : "0");
        }

        private void Tlbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("ToolBtn", Tlbl.IsOn ? "True" : "0");
        }

        private void Drbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("DarkBtn", Drbl.IsOn ? "True" : "0");
        }

        private void OpenNew_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("OpSw", OpenNew.IsOn ? "True" : "0");
        }
    }
}
