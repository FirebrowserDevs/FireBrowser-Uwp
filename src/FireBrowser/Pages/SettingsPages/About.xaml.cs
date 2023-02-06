using CommunityToolkit.Labs.WinUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AboutCardClicked(object sender, RoutedEventArgs e)
        {
            string url = "https://example.com";
            switch ((sender as SettingsCard).Tag)
            {
                case "Discord":
                    url = "https://discord.gg/CWGHdGVFFV";
                    break;
                case "GitHub":
                    url = "https://github.com/jarno9981/FireBrowser-Uwp";
                    break;
                case "License":
                    url = "https://github.com/jarno9981/FireBrowser-Uwp/blob/main/LICENSE";
                    break;
            }
           // MainPage mp = new();
            //To-Do: Get the currently selected tab's position and launch the new one next to it
           // param?.TabView.TabItems.Add(mp.CreateNewTab(typeof(WebContent), url));
        }
    }
}
