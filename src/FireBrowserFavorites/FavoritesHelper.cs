using System.Threading.Tasks;

namespace FireBrowserFavorites
{
    public class FavoritesHelper
    {
        public static async Task AddFavoritesItem(string title, string url)
        {
            await Task.Run(() =>
            {
                Json.AddItemToJson("Favorites.json", title, url);
            });
        }
    }
}
