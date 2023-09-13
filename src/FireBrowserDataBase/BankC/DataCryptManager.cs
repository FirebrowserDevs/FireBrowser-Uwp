using System;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using FireBrowserDataBase.BankC;

public class DataCryptManager
{
    private readonly string databasePath;
    private readonly string storedKey;

    public DataCryptManager(string storedKey, string databaseFileName = "datacrd.crypt")
    {
        this.storedKey = storedKey;
        this.databasePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, databaseFileName);

        // Initialize the database (create tables if they don't exist)
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (!File.Exists(databasePath))
        {
            using (var connection = new SqliteConnection($"Data Source={databasePath}"))
            {
                connection.Open();

                // Create a table to store encrypted data and IVs
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS EncryptedData (
                        Id INTEGER PRIMARY KEY,
                        Name BLOB,
                        EncryptedValue BLOB,
                        IV BLOB
                    );";
                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }

    public void SaveEncryptedValue(string name, string value)
    {
        // Encrypt the value and get IV
        (string encryptedValue, string iv) = EnqHelper.EncryptAES(value, storedKey);

        using (var connection = new SqliteConnection($"Data Source={databasePath}"))
        {
            connection.Open();

            // Insert the encrypted value and IV into the database
            string insertQuery = "INSERT INTO EncryptedData (Name, EncryptedValue, IV) VALUES (@Name, @EncryptedValue, @IV);";
            using (var command = new SqliteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@EncryptedValue", encryptedValue);
                command.Parameters.AddWithValue("@IV", iv);
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public string GetDecryptedValue(string name)
    {
        string decryptedValue = null;

        using (var connection = new SqliteConnection($"Data Source={databasePath}"))
        {
            connection.Open();

            // Retrieve the encrypted value and IV from the database
            string selectQuery = "SELECT EncryptedValue, IV FROM EncryptedData WHERE Name = @Name;";
            using (var command = new SqliteCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string encryptedValue = reader.GetString(0);
                        string iv = reader.GetString(1);

                        // Decrypt the value
                        decryptedValue = EnqHelper.DecryptAES(encryptedValue, storedKey, iv);
                    }
                }
            }

            connection.Close();
        }

        return decryptedValue;
    }
}
