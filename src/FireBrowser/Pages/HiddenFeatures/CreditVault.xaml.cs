using FireBrowserDataBase.BankC;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            loaddata();
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


       public void loaddata()
        {
            string storedKey = FireBrowserInterop.SettingsHelper.GetSetting("keyofyours$fire$key");

            if (string.IsNullOrEmpty(storedKey))
            {
                // If no key exists, generate a new one
                storedKey = EnqHelper.GenerateAESKey();
                // Store the new key
                FireBrowserInterop.SettingsHelper.SetSetting("keyofyours$fire$key", storedKey);
            }

            DataCryptManager dataCryptManager = new DataCryptManager(storedKey);

            // Define a collection to store decrypted card data
            List<BankCard> DecryptedCardList = new List<BankCard>();


            // Fetch decrypted card data and add it to the collection
            string decryptedBankName = dataCryptManager.GetDecryptedValue("BankName");
            string decryptedCreditNumber = dataCryptManager.GetDecryptedValue("CreditNumber");
            string decryptedFullName = dataCryptManager.GetDecryptedValue("FullName");
            string decryptedExpiryDate = dataCryptManager.GetDecryptedValue("ExpiryDate");
            string decryptedCVV = dataCryptManager.GetDecryptedValue("CVV");
            string decryptedCardType = dataCryptManager.GetDecryptedValue("CardType");

            Debug.WriteLine(decryptedBankName);
            Debug.WriteLine(decryptedCreditNumber);
            Debug.WriteLine(decryptedFullName);
            Debug.WriteLine(decryptedExpiryDate);
            Debug.WriteLine(decryptedCVV);
            Debug.WriteLine(decryptedCardType);
        }
    }
}
