using Microsoft.Data.Sqlite;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserDataBase.BankC
{
    public class DataHelper
    {
        string DbPath = $"{ApplicationData.Current.LocalFolder.Path}/datacrd.crypt";

        public async Task<int> SaveBankCardAsync(BankCard bankCard)
        {
            byte[] encryptionKey = EnqHelper.GenerateEncryptionKey();

            if (!File.Exists(DbPath))
            {
                using (SqliteConnection connection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/datacrd.crypt;"))
                {
                    connection.Open();

                    // Create the table using a stored procedure
                    string createTableProcedure = @"
            CREATE TABLE IF NOT EXISTS BankCards (
                Id INTEGER PRIMARY KEY,
                BankName BLOB,
                EncryptedCreditNumber BLOB,
                EncryptedFullName BLOB,
                EncryptedExpiryDate BLOB,
                EncryptedCVV BLOB,
                EncryptedType BLOB
            );
        ";

                    using (SqliteCommand createTableCommand = new SqliteCommand(createTableProcedure, connection))
                    {
                        await createTableCommand.ExecuteNonQueryAsync();
                    }

                    connection.Close();
                }
            }

            using (SqliteConnection connection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/datacrd.crypt;"))
            {
                connection.Open();

                // Encrypt sensitive data and insert the data using a stored procedure
                string insertDataProcedure = @"
        INSERT INTO BankCards (
            BankName,
            EncryptedCreditNumber,
            EncryptedFullName,
            EncryptedExpiryDate,
            EncryptedCVV,
            EncryptedType
        ) VALUES (
            @BankName,
            @EncryptedCreditNumber,
            @EncryptedFullName,
            @EncryptedExpiryDate,
            @EncryptedCVV,
            @EncryptedType
        );
    ";

                using (SqliteCommand saveCommand = new SqliteCommand(insertDataProcedure, connection))
                {
                    byte[] encryptedBankName = EnqHelper.Encrypt(Encoding.UTF8.GetBytes(bankCard.BankName), encryptionKey);
                    byte[] encryptedCreditNumber = EnqHelper.Encrypt(Encoding.UTF8.GetBytes(bankCard.CreditNumber), encryptionKey);
                    byte[] encryptedFullName = EnqHelper.Encrypt(Encoding.UTF8.GetBytes(bankCard.FullName), encryptionKey);
                    byte[] encryptedExpiryDate = EnqHelper.Encrypt(Encoding.UTF8.GetBytes(bankCard.ExpiryDate), encryptionKey);
                    byte[] encryptedCVV = EnqHelper.Encrypt(Encoding.UTF8.GetBytes(bankCard.CVV), encryptionKey);
                    byte[] encryptedType = EnqHelper.Encrypt(Encoding.UTF8.GetBytes(bankCard.Type), encryptionKey);

                    saveCommand.Parameters.AddWithValue("@BankName", encryptedBankName);
                    saveCommand.Parameters.AddWithValue("@EncryptedCreditNumber", encryptedCreditNumber);
                    saveCommand.Parameters.AddWithValue("@EncryptedFullName", encryptedFullName);
                    saveCommand.Parameters.AddWithValue("@EncryptedExpiryDate", encryptedExpiryDate);
                    saveCommand.Parameters.AddWithValue("@EncryptedCVV", encryptedCVV);
                    saveCommand.Parameters.AddWithValue("@EncryptedType", encryptedType);

                    await saveCommand.ExecuteNonQueryAsync();
                }

                connection.Close();
            }


            return 1; // You can return the inserted/updated record ID here
        }
    }
}