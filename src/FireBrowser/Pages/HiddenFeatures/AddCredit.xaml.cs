using FireBrowserDataBase.BankC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            BankCard bankCard = new BankCard
            {
                BankName = Banks.Text,
                CreditNumber = CardNum.Text,
                FullName = HolderNm.Text,
                ExpiryDate = ExpData.Text,
                CVV = CCvDat.Text,
                Type = Type.Text
            };

            DataHelper dbHelper = new DataHelper();
            await dbHelper.SaveBankCardAsync(bankCard);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
