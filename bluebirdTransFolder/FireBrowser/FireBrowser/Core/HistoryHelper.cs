namespace FireBrowser.Core;

public class HistoryHelper
{
    public static void AddHistoryItem(string title, string url)
    {
        // To prevent unnessary items in history file with homepage entries
        if (url != HomepageUrl)
        {
            Json.AddItemToJson("History.json", title, url);
        }
    }
}