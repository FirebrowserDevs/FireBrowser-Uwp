using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserDataBase
{
    public class DbCreation
    {
        public static async Task CreateDatabase()
        {
            await Task.Run(async () =>
            {
                StorageFile databaseFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("History.db", CreationCollisionOption.OpenIfExists);

                var connectionStringBuilder = new SqliteConnectionStringBuilder();
                connectionStringBuilder.DataSource = databaseFile.Path;

                using (var db = new SqliteConnection(connectionStringBuilder.ConnectionString))
                {
                    await db.OpenAsync();

                    string tableCommand = "CREATE TABLE IF NOT " +
                         "EXISTS urlsDb (Url NVARCHAR(2083) PRIMARY KEY NOT NULL, " +
                         "Title NVARCHAR(2048), " +
                         "Visit_Count INTEGER, " +
                         "Last_Visit_Time DATETIME)";

                    using (SqliteCommand createTable = new SqliteCommand(tableCommand, db))
                    {
                        await createTable.ExecuteReaderAsync();
                    }
                }
            });
        }

    }
}
