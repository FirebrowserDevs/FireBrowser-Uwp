using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserDataBase
{
    public class DbAddHis
    {
        public async Task AddHistData(string address, string title)
        {
            await Task.Run(() =>
            {
                using (SqliteConnection m_dbConnection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/History.db;"))
                {
                    m_dbConnection.Open();

                    var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    using (var selectCmd = m_dbConnection.CreateCommand())
                    {
                        selectCmd.CommandText = "SELECT * FROM urlsDb WHERE url = @url AND title = @title AND last_visit_time = @lastVisitTime";
                        selectCmd.Parameters.AddWithValue("@url", address);
                        selectCmd.Parameters.AddWithValue("@title", title);
                        selectCmd.Parameters.AddWithValue("@lastVisitTime", currentTime);

                        using (var reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                using (var updateCmd = m_dbConnection.CreateCommand())
                                {
                                    updateCmd.CommandText = "UPDATE urlsDb SET visit_count = visit_count + 1, last_visit_time = @lastVisitTime WHERE url = @url AND title = @title AND last_visit_time = @lastVisitTime";
                                    updateCmd.Parameters.AddWithValue("@url", address);
                                    updateCmd.Parameters.AddWithValue("@title", title);
                                    updateCmd.Parameters.AddWithValue("@lastVisitTime", currentTime);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (var insertCmd = m_dbConnection.CreateCommand())
                                {
                                    insertCmd.CommandText = "INSERT OR IGNORE INTO urlsDb (url, title, visit_count, last_visit_time) VALUES (@url, @title, @visitCount, @lastVisitTime)";
                                    insertCmd.Parameters.AddWithValue("@url", address);
                                    insertCmd.Parameters.AddWithValue("@title", title);
                                    insertCmd.Parameters.AddWithValue("@visitCount", 1);
                                    insertCmd.Parameters.AddWithValue("@lastVisitTime", currentTime);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
