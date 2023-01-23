using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Data.Sqlite;
using Windows.UI.Xaml.Media.Imaging;
using System;

namespace FireBrowser.Core;

public static class DataAccess
{
    //this new code make sure it loaded and unloaded so less memory leaks
    public static async Task<List<HistoryDetails>> GetHistoryDetails()
    {
        List<HistoryDetails> historyDetails = new List<HistoryDetails>();

        using (SqliteConnection con = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserHistory.Db"))
        {
            con.Open();
            historyDetails = await SelectHistoryAsync(con);
        }

        return historyDetails;
    }

    private static async Task<List<HistoryDetails>> SelectHistoryAsync(SqliteConnection con)
    {
        List<HistoryDetails> historyDetails = new List<HistoryDetails>();
        using (SqliteCommand selectHistoryCommand = new SqliteCommand("SELECT url, title, last_visit_time FROM urls", con))
        {
            using (SqliteDataReader query = await selectHistoryCommand.ExecuteReaderAsync())
            {
                while (query.Read())
                {
                    HistoryDetails hd = new HistoryDetails
                    {
                        Url = query.GetString(0),
                        Title = query.GetString(1),
                        Date = query.GetDateTime(2)
                    };

                    historyDetails.Add(hd);
                }
            }
        }
        return historyDetails;
    }

    public record HistoryDetails
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public BitmapImage ImageSource { get; set; }
        public DateTime Date { get; set; }
    }

    public record SmallHistoryDetails
    {
        public string ImageSource { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }

}