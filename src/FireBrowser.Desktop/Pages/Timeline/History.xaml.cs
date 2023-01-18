using FireBrowser.Core;
using FireBrowser.History;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Timeline
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class History : Page
    {
        public History()
        {
            this.InitializeComponent();
        }

        public async void GetHistoryBig()
        {
            var historyList = await DataAccess.GetHistoryDetails();
            foreach (var item in historyList)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ContentTemplate = Application.Current.Resources["BigHistoryDataTemplate"] as DataTemplate;
                item.ImageSource = new BitmapImage(new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + item.Url + "&size=16"));
                lvi.DataContext = item;
                lvi.Tag = item.Url;

                // Check if the item already exists in the ListView
                bool itemExists = false;
                foreach (ListViewItem existingItem in BigTemp.Items)
                {
                    if (existingItem.Tag.Equals(item.Url))
                    {
                        itemExists = true;
                        break;
                    }
                }

                // If the item does not exist, add it to the ListView
                if (!itemExists)
                {
                    BigTemp.Items.Add(lvi);
                    BigTemp.AllowFocusOnInteraction = true;
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHistoryBig();
        }

  
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            BigTemp.Items.Clear();
            DelHistory();
        }

        public void DelHistory()
        {
            using (var con = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserData/{AppData.CurrentProfileCore.FriendlyID}/FireBrowserHistory.Db"))
            {
                con.Open();

                using (var command = new SqliteCommand("DELETE FROM urls", con))
                {
                    command.ExecuteNonQuery();
                }

                con.Close();
            }

        }
    }
}
