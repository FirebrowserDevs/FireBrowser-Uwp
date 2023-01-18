using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Settings
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Privacy : Page
    {
        public Privacy()
        {
            this.InitializeComponent();
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var RadioButton = (RadioButton)sender;
            var engine = RadioButton.Tag as string;
            var settings = await Core.Settings.GetSettingsDataAsync();
            switch (engine)
            {
                case "Google":
                    settings.Privacy.SearchEngine = "https://google.com/search?q=";
                    break;
                case "Bing":
                    settings.Privacy.SearchEngine = "https://bing.com/search?q=";
                    break;
                case "DuckDuckGo":
                    settings.Privacy.SearchEngine = "https://duckduckgo.com/?q=";
                    break;
                case "Ecosia":
                    settings.Privacy.SearchEngine = "https://ecosia.org/search?q=";
                    break;
                case "Brave":
                    settings.Privacy.SearchEngine = "https://search.brave.com/search?q=";
                    break;
            }
            Core.AppData.SaveData(settings, Core.AppData.DataType.AppSettings);
        }
    }
}
