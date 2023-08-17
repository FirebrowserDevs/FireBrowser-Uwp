using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace FireBrowserDataBase
{
    public class DbClearTableData
    {
        public void DeleteTableData(string tablename, string where)
        {
            using (SqliteConnection m_dbConnection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/History.db;"))
            {
                m_dbConnection.Open();
                var wheret = "";
                if (where != "") wheret = $" where {where}";
                SqliteCommand selectCommand = new SqliteCommand($"delete from {tablename}{wheret}", m_dbConnection);
                SqliteDataReader query = selectCommand.ExecuteReader();
                m_dbConnection.Close();
            }
        }
    }
}
