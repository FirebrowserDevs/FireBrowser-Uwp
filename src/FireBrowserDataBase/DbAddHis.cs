using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserDataBase
{
    public class DbAddHis
    {
        public async Task<SqliteDataReader> AddHistData(string command)
        {
            using (SqliteConnection m_dbConnection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/History.db;"))
            {
                m_dbConnection.Open();
                SqliteCommand selectCommand = new SqliteCommand(command, m_dbConnection);
                SqliteDataReader query = selectCommand.ExecuteReader();
                m_dbConnection.Close();
                return query;
            }
        }
    }
}
