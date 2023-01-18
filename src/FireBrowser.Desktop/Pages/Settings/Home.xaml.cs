using FireBrowser.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Settings
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
        }
        public AccountPageViewModel ViewModel { get; set; }
        public partial class AccountPageViewModel : ObservableObject
        {
            public ObservableCollection<AppData.ProfileCore> UserList;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await AppData.CreateAppDataCore();
        }
        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            await AppData.CreateProfileCore("New profile test with emojis 😀😀😀");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await NewProfileDialog.ShowWithAnimationAsync();
        }

        private async void NewProfileDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var newUser = await AppData.CreateProfileCore(NewUserName.Text);
            ViewModel.UserList.Add(newUser);
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var el = ((listView.SelectedItem as ListViewItem).Content);
            var id = el.GetType();
            Debug.WriteLine("Id is:"+ id);
           // await Windows.System.Launcher.LaunchUriAsync(new Uri($"airplane://?profile={id}"));
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<AppData.ProfileCore> list = new();
            var appdata = await AppData.GetAppDataCore();

            foreach (var user in appdata.Profiles)
            {
                list.Add(user);
            }

            ViewModel = new()
            {
                UserList = list
            };
            
        }
    }
}
