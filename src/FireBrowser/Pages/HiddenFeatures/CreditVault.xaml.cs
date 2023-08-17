using FireBrowserDataBase.BankC;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.HiddenFeatures
{
    public sealed partial class CreditVault : Page
    {
        public CreditVault()
        {
            this.InitializeComponent();
            //loaddata();
        }

        private Passer passer;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as Passer;
            passer.Tab.Header = "FireBrowser Vault";
            passer.Tab.IconSource = new muxc.FontIconSource()
            {
                Glyph = "\uF540"
            };
        }

        public ObservableCollection<BankCard> DecryptedCardList { get; } = new ObservableCollection<BankCard>();

        private async void Add_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AddCredit cn = new AddCredit();

            cn.PrimaryButtonClick += (sender, e) =>
            {
                loaddata(); 
            };

            await cn.ShowAsync();
        }

        byte[] encryptionKey = EnqHelper.GenerateEncryptionKey();
        public void loaddata()
        {
            string dbPath = $"{ApplicationData.Current.LocalFolder.Path}/datacrd.crypt";

            if (File.Exists(dbPath))
            {
                using (SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM BankCards";
                    using (SqliteCommand selectCommand = new SqliteCommand(selectQuery, connection))
                    {
                        using (SqliteDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Decrypt and assign encrypted properties
                                byte[] encryptedBankName = (byte[])reader["BankName"];
                                byte[] encryptedCreditNumber = (byte[])reader["EncryptedCreditNumber"];
                                byte[] encryptedFullName = (byte[])reader["EncryptedFullName"];
                                byte[] encryptedExpiryDate = (byte[])reader["EncryptedExpiryDate"];
                                byte[] encryptedCVV = (byte[])reader["EncryptedCVV"];
                                byte[] encryptedType = (byte[])reader["EncryptedType"];

                                // Decrypt the data
                                string bankName = Encoding.UTF8.GetString(EnqHelper.Decrypt(encryptedBankName, encryptionKey));
                                string creditNumber = Encoding.UTF8.GetString(EnqHelper.Decrypt(encryptedCreditNumber, encryptionKey));
                                string fullName = Encoding.UTF8.GetString(EnqHelper.Decrypt(encryptedFullName, encryptionKey));
                                string expiryDate = Encoding.UTF8.GetString(EnqHelper.Decrypt(encryptedExpiryDate, encryptionKey));
                                string cvv = Encoding.UTF8.GetString(EnqHelper.Decrypt(encryptedCVV, encryptionKey));
                                string type = Encoding.UTF8.GetString(EnqHelper.Decrypt(encryptedType, encryptionKey));

                                // Create a new BankCard instance and add it to the collection
                                BankCard newCard = new BankCard
                                {
                                    BankName = bankName,
                                    CreditNumber = creditNumber,
                                    FullName = fullName,
                                    ExpiryDate = expiryDate,
                                    CVV = cvv,
                                    Type = type
                                };

                                DecryptedCardList.Add(newCard);
                            }
                        }
                    }

                    connection.Close();
                }
            }             
        }
    }
}
