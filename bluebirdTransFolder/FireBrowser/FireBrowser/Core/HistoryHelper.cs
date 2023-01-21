using Microsoft.Data.Sqlite;
using System;
using System.IO;
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
        SqliteConnection m_dbConnection = new($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserHistory.Db");
        m_dbConnection.Open();

        string sql = "create table urls (url varchar(250), title varchar(250), last_visit_time DATETIME)";
        
        SqliteCommand command = new(sql, m_dbConnection);
        command.ExecuteNonQuery();

        m_dbConnection.Close();
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
}