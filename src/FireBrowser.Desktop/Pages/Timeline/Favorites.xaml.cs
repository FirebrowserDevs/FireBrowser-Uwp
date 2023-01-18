using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using static FireBrowser.Core.Favorites;
using FireBrowser.Core;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Activity
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Favorites : Page
    {
        public Favorites()
        {
            this.InitializeComponent();
            LoadFavorites();
            //TreeView.MenuItemsSource = Core.AppData.currentProfile.Favorites;
        }

        private List<JsonItems> FavoritesList;

        private async void LoadFavorites()
        {
            FavoritesList = await GetListFromJsonAsync("Favorites.json");
            if (FavoritesList != null)
            {
                FavoritesListView.ItemsSource = FavoritesList;
            }
            else
            {
                await UI.ShowDialog("Error", "Your favorites are empty!");
            }
        }
        private void NewFav(object sender, RoutedEventArgs e)
        {
           //Core.AppData.AddFavorite("Favorite", FavTitle.Text, new Uri(FavUri.Text));
        }
        private void NewFolder(object sender, RoutedEventArgs e)
        {
            //Core.AppData.AddFavorite("Folder", FlTitle.Text, null);
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
