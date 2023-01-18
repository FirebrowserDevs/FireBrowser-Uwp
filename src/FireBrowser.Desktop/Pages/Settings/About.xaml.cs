using CommunityToolkit.Labs.WinUI;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Settings
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class About : Page
    {
        Passer param;

        int i = 0;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
        }
        public About()
        {
            this.InitializeComponent();
        }
        private void EasterEgg(object sender, RoutedEventArgs e)
        {
            i++;
            if (i == 7)
            {
                //Core.Settings.currentProfile.DebugSettingsEnabled = true;
                //Core.AppData.SaveData(Core.Settings.appSettings, Core.AppData.DataType.AppSettings);
            }
        }
        private void AboutCardClicked(object sender, RoutedEventArgs e)
        {
            string url = "https://example.com";
            switch((sender as SettingsCard).Tag)
            {
                case "Donate":
                    url = "https://example.com";
                    break;
                case "Discord":
                    url = "https://discord.gg/ayD26c9vT9";
                    break;
                case "Twitter":
                    //temporarily it's my twitter account but soon it'll be a different one
                    url = "https://twitter.com/AlurDesign";
                    break;
                case "GitHub":
                    url = "https://github.com/Dots-Studio/FireBrowser";
                    break;
                case "License":
                    url = "https://github.com/Dots-Studio/FireBrowser/raw/main/LICENSE";
                    break;
            }
            MainPage mp = new();
            //To-Do: Get the currently selected tab's position and launch the new one next to it
            param?.TabView.TabItems.Add(mp.CreateNewTab(typeof(WebContent), url));
        }
    }
}
