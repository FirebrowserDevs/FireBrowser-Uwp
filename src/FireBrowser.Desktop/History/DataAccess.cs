﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using FireBrowser.Core;
using System.Diagnostics;
using static FireBrowser.Core.UserData;
using Microsoft.Data.Sqlite;
using Windows.UI.Xaml.Media.Imaging;

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

        public class HistoryDetails
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public BitmapImage ImageSource { get; set; }

            public DateTime Date { get; set; }
        }

        public class SmallHistoryDetails
        {
            public string ImageSource { get; set; }
            public string Title { get; set; }

            DateTime Date
            {
                get; set;
            }
        }
    }
}
