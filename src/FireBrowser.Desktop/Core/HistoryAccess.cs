using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airplane.Core;
using Windows.Storage;
using static Airplane.Core.UserData;

namespace Airplane.Core
{
    public static class HistoryAccess
    {
     
            public static async Task<List<Site>> GetHistoryDetails()
            {
                List<Site> historyDetails = new List<Site>();

                StorageFolder EBFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("EBWebView");
                StorageFolder DefaultFolder = await EBFolder.GetFolderAsync("Default");
                StorageFile HistoryFile = await DefaultFolder.GetFileAsync("History");

                string DBPath = HistoryFile.Path;

                using (SqliteConnection conn = new SqliteConnection($"Filename={DBPath}"))
                {
                    conn.Open();

                    SqliteCommand selectHistoryCommand = new SqliteCommand("SELECT url, title FROM urls", conn);

                    SqliteDataReader query = selectHistoryCommand.ExecuteReader();

                    while (query.Read())
                    {
                        Site hd = new Site();
                        hd.URL = new Uri(query.GetString(0));
                        hd.Name = query.GetString(1);

                        historyDetails.Add(hd);
                    }
                }

                return historyDetails;
            }   
    }
}
