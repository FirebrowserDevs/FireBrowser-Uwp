using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserDataBase
{
    public class DbClear
    {
        public static async Task ClearDb()
        {
            await Task.Run(() =>
            {
                // open a connection to the database
                using (var connection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}\\History.db"))
                {
                    connection.Open();

                    // create a command that deletes all rows from the table
                    var command = new SqliteCommand("DELETE FROM urlsDb", connection);

                    // execute the command to clear the table
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            });
        }
    }
}