using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowserFavorites
{
    public class FavoritesHelper
    {
        public static void AddFavoritesItem(string title, string url)
        {
            Json.AddItemToJson("Favorites.json", title, url);
        }
    }
}
