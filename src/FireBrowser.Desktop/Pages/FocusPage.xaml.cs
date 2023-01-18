using FireBrowser.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using static FireBrowser.Core.UserData;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FocusPage : Page
    {
        public FocusPage()
        {
            this.InitializeComponent();
            ObservableCollection<UserData.FocusList> list = new();

            foreach(var item in UserData.currentProfile.Focus)
            {
                list.Add(item);
            }

            ViewModel = new() 
            {
                NewListSites = new(),
                FocusLists = list
            };
        }
        public partial class FocusViewModel : ObservableObject
        {
            [ObservableProperty]
            ObservableCollection<Uri> newListSites;
            [ObservableProperty]
            ObservableCollection<Core.UserData.FocusList> focusLists;
        }
        public FocusViewModel ViewModel;
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Core.Focusing.selectedList = (sender as ListView).SelectedIndex;
            Core.Focusing.isFocusing = true;
        }

        private void FocusingBanner_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as NavigationViewItem).Content = string.Format(ResourceLoader.GetForCurrentView().GetString("FocusingBanner"), "Refocus");
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //Start focusing
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Create a new list
            await CreateFocusListDialog.ShowWithAnimationAsync();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://google.com");
            ViewModel.NewListSites.Add(uri);
        }

        private void CreateFocusListDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var newFocusList = new FocusList()
            {
                SiteList = new()
            };

            foreach (var item in ViewModel.NewListSites)
            {
                newFocusList.SiteList.Add(item);
            }
            newFocusList.Name = NewListName.Text;

            newFocusList.Icon = (IconGridView.SelectedItem as GridViewItem).Content.ToString();
            newFocusList.Mode = (FocusMode)NewListMode.SelectedIndex;

            ViewModel.FocusLists.Add(newFocusList);
            UserData.currentProfile.Focus.Add(newFocusList);
            //AppData.SaveData(UserData.appSettings);
        }
    }
}
