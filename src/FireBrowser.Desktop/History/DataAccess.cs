using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using FireBrowser.Core;
using System.Diagnostics;
using static FireBrowser.Core.UserData;
using Microsoft.Data.Sqlite;

namespace FireBrowser.History
{
    public static class DataAccess
    {
        public static async Task<List<HistoryDetails>> GetHistoryDetails()
        {          
            List<HistoryDetails> historyDetails = new List<HistoryDetails>();

            SqliteConnection con = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserData/{AppData.CurrentProfileCore.FriendlyID}/FireBrowserHistory.Db");
            con.Open();


            SqliteCommand selectHistoryCommand = new SqliteCommand("SELECT url, title, last_visit_time FROM urls", con);
            SqliteDataReader query = selectHistoryCommand.ExecuteReader();

                while (query.Read())
                {

                    HistoryDetails hd = new HistoryDetails();
                    hd.Url = query.GetString(0);
                    hd.Title = query.GetString(1);
                    hd.Date = query.GetDateTime(2);

                    historyDetails.Add(hd);
                }

                ///this to make sure it doesn't crash
                con.Close();


             return historyDetails;
        }
    }
}
