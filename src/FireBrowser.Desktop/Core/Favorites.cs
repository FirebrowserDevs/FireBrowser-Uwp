using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace FireBrowser.Core;

public static class Favorites
{
    // imported from PenguinApps.Core.JsonHelper.cs
    // imported from BrowserVNext.Core.FavoritesHelper.cs
    // adjusted for airplane
    public static void AddFavoritesItem(string title, string url)
    {
        AddItemToJson("Favorites.json", title, url);
    }
    static StorageFolder userFolder;
    public static async void CreateJsonFile(string file, string title, string url)
    {
        userFolder = await AppData.GetUserFolder();
        // Generate json
        string json = "[{\"title\":\"" + title + "\"," + "\"url\":\"" + url + "\"}]";
        // create json file
        await userFolder.CreateFileAsync(file, CreationCollisionOption.ReplaceExisting);
        // get json file
        var fileData = await userFolder.GetFileAsync(file);
        // write json to json file
        await FileIO.WriteTextAsync(fileData, json);
    }

    public static async void AddItemToJson(string file, string title, string url)
    {
        userFolder = await AppData.GetUserFolder();
        var fileData = await userFolder.TryGetItemAsync(file);
        if (fileData == null) CreateJsonFile(file, title, url);
        else
        {
            // get json file content
            string json = await FileIO.ReadTextAsync((IStorageFile)fileData);
            // new historyitem
            JsonItems newHistoryitem = new()
            {
                Title = title,
                Url = url
            };
            // Convert json to list
            List<JsonItems> historylist = JsonConvert.DeserializeObject<List<JsonItems>>(json);
            // Add new historyitem
            historylist.Insert(0, newHistoryitem);
            // Convert list to json
            string newJson = JsonConvert.SerializeObject(historylist);
            // Write json to json file
            await FileIO.WriteTextAsync((IStorageFile)fileData, newJson);
        }
    }

    public static async Task<List<JsonItems>> GetListFromJsonAsync(string file)
    {
        userFolder = await AppData.GetUserFolder();
        var fileData = await userFolder.TryGetItemAsync(file);
        if (fileData == null) return null;
        else
        {
            string filecontent = await FileIO.ReadTextAsync((IStorageFile)fileData);
            return JsonConvert.DeserializeObject<List<JsonItems>>(filecontent);
        }
    }

    public class JsonItems
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
