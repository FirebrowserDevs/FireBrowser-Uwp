using FireBrowserDataBase.BankC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.HiddenFeatures
{
    public sealed partial class AddCredit : ContentDialog
    {
       

        public AddCredit()
        {
            this.InitializeComponent();
        }

       

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
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


            string selectedCardType = Type.SelectedItem.ToString();
            string selectedCardBank = Banks.SelectedItem.ToString();

            string creditNumber = CardNum.Text.ToString();
            string fullName = HolderNm.Text.ToString();
            string expiryDate = ExpData.Text.ToString();
            string cvv = CCvDat.Text.ToString();
                

            dataCryptManager.SaveEncryptedValue("BankName", selectedCardBank);
            dataCryptManager.SaveEncryptedValue("CreditNumber", creditNumber);
            dataCryptManager.SaveEncryptedValue("FullName", fullName);
            dataCryptManager.SaveEncryptedValue("ExpiryDate", expiryDate);
            dataCryptManager.SaveEncryptedValue("CVV", cvv);
            dataCryptManager.SaveEncryptedValue("CardType", selectedCardType);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
