using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserDataBase
{
    public class DbReadTable
    {    
        public async Task<SqliteDataReader> ReadTableData(string tablename, string column, string orderby, string where)
        {
            StorageFile databaseFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("History.db", CreationCollisionOption.OpenIfExists);

            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databaseFile.Path;

            using (var db = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                db.Open();
                string orderbyt = "";
                if (orderby != "") orderbyt = " order by " + orderby;
                string wheret = "";
                if (where != "") wheret = " where " + where;
                SqliteCommand selectCommand = new SqliteCommand($"Select {column} From {tablename}{orderby}{wheret}", db);
                SqliteDataReader query = selectCommand.ExecuteReader();
                return query;
            }
        }
    }
}
