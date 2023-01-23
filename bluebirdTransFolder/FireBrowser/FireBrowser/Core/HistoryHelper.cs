using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowser.Core;

public class HistoryHelper
{
    //fixes memory leaks jumps like 10mb now are less than <2mb
    private static readonly string _path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "FireBrowserHistory.Db");

    public static void CheckHisDb()
    {
        if (!File.Exists(_path))
        {
            CreateHisDb();
        }
    }

    private static void CreateHisDb()
    {
        using var connection = new SqliteConnection($"Data Source={_path}");
        connection.Open();

        string sql = "CREATE TABLE IF NOT EXISTS urls (url VARCHAR(250), title VARCHAR(250), last_visit_time DATETIME)";
        using var command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    public static void WriteToDb(string title, string url)
    {
        using var con = new SqliteConnection($"Data Source={_path}");
        con.Open();

        using var cmd = new SqliteCommand("INSERT INTO urls(url, title, last_visit_time) VALUES(@url, @title, @last_visit_time)", con);
        cmd.Parameters.Add("@url", SqliteType.Text).Value = url;
        cmd.Parameters.Add("@title", SqliteType.Text).Value = title;
        cmd.Parameters.Add("@last_visit_time", SqliteType.Text).Value = DateTime.Now;
        cmd.ExecuteNonQuery();
    }

    public async static Task UpdateHistoryListAsync()
    {
        HistoryList = await DataAccess.GetHistoryDetails();
    }

    public static void DelHistory()
    {
        using var con = new SqliteConnection($"Data Source={_path}");
        con.Open();

        using var cmd = new SqliteCommand("DELETE FROM urls", con);
        cmd.ExecuteNonQuery();
    }

}