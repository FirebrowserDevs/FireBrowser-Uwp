using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowser.Core;

public class HistoryHelper
{
    public static void CheckHisDb()
    {
        if (!File.Exists($"{ApplicationData.Current.LocalFolder.Path}/FireBrowserHistory.Db"))
        {
            CreateHisDb();
        }
    }

    private static void CreateHisDb()
    {
        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "FireBrowserHistory.Db");
        using (SqliteConnection connection = new SqliteConnection($"Data Source={path}"))
        {
            connection.Open();

            string sql = "CREATE TABLE IF NOT EXISTS urls (url VARCHAR(250), title VARCHAR(250), last_visit_time DATETIME)";
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }


    public static void WriteToDb(string title, string url)
    {
        using var con = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserHistory.Db");
        con.Open();

        using (var cmd = new SqliteCommand("INSERT INTO urls(url, title, last_visit_time) VALUES(@url, @title, @last_visit_time)", con))
        {
            cmd.Parameters.AddWithValue("@url", url);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@last_visit_time", DateTime.Now);
            cmd.ExecuteNonQuery();
        }
        con.Close();
    }

    public async static Task UpdateHistoryListAsync()
    {
        HistoryList = await DataAccess.GetHistoryDetails();
    }

    public static void DelHistory()
    {
        using var con = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserHistory.Db");
        con.Open();

        using (var cmd = new SqliteCommand("INSERT INTO urls(url, title, last_visit_time) VALUES(@url, @title, @last_visit_time)", con))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "DELETE FROM urls";
            cmd.ExecuteNonQuery();
        }
        con.Close();
    }
}