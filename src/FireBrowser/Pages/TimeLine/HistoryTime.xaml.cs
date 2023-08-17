using FireBrowserDataBase;
using FireBrowserDialogs.DialogTypes.AreYouSureDialog;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.TimeLine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryTime : Page
    {
        public HistoryTime()
        {
            this.InitializeComponent();
            FetchBrowserHistory();
        }

        private ObservableCollection<HistoryItem> browserHistory;
        private async void FetchBrowserHistory()
        {
            Batteries.Init();
            try
            {
                using (var connection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/History.db;"))
                {
 
                    await connection.OpenAsync();

                    string sql = "SELECT url, title, visit_count, last_visit_time FROM urlsDb ORDER BY last_visit_time DESC";

                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            browserHistory = new ObservableCollection<HistoryItem>();

                            while (reader.Read())
                            {
                                HistoryItem historyItem = new HistoryItem
                                {
                                    Url = reader.GetString(0),
                                    Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    VisitCount = reader.GetInt32(2),
                                    LastVisitTime = reader.GetString(3),
                                };


                                historyItem.ImageSource = new BitmapImage(new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + historyItem.Url + "&size=32"));
                                browserHistory.Add(historyItem);
                            }

                            BigTemp.ItemsSource = browserHistory;
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might be thrown during the execution of the code
                Debug.WriteLine($"Error: {ex.Message}");
            }

        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FetchBrowserHistory();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            SureDialog customDialog = new SureDialog(); // Create an instance of your custom content dialog class
            customDialog.State = DialogState.History; // Set the state of the dialog 
            ContentDialogResult result = await customDialog.ShowAsync(); // Show the dialog and wait for the user to respond
            if (result == ContentDialogResult.Primary)
            {
                await DbClear.ClearDb();
                BigTemp.ItemsSource = null;
            }
            else
            {
                return;
            }
        }

        private void FilterBrowserHistory(string searchText)
        {
            if (browserHistory == null) return;

            // Clear the collection to start fresh with filtered items
            BigTemp.ItemsSource = null;

            // Filter the browser history based on the search text
            var filteredHistory = new ObservableCollection<HistoryItem>(browserHistory
                .Where(item => item.Url.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                               item.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true));

            // Bind the filtered browser history items to the ListView
            BigTemp.ItemsSource = filteredHistory;
        }

        private void Ts_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = Ts.Text;
            FilterBrowserHistory(searchText);
        }

        private string selectedHistoryItem;
        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            // Get the selected HistoryItem object
            HistoryItem historyItem = ((FrameworkElement)sender).DataContext as HistoryItem;
            selectedHistoryItem = historyItem.Url;

            // Create a context menu flyout
            var flyout = new MenuFlyout();

            // Add a menu item for "Delete This Record" button
            var deleteMenuItem = new MenuFlyoutItem
            {
                Text = "Delete This Record",
            };

            // Set the icon for the menu item using the Unicode escape sequence
            deleteMenuItem.Icon = new FontIcon
            {
                Glyph = "\uE74D" // Replace this with the Unicode escape sequence for your desired icon
            };

            // Handle the click event directly within the right-tapped event handler
            deleteMenuItem.Click += (s, args) =>
            {
                DbClearTableData db = new();
                db.DeleteTableData("urlsDb", $"Url = '{selectedHistoryItem}'");
                if (BigTemp.ItemsSource is ObservableCollection<HistoryItem> historyItems)
                {
                    var itemToRemove = historyItems.FirstOrDefault(item => item.Url == selectedHistoryItem);
                    if (itemToRemove != null)
                    {
                        historyItems.Remove(itemToRemove);
                    }
                }
                // After deletion, you may want to update the UI or any other actions
            };

            flyout.Items.Add(deleteMenuItem);

            // Show the context menu flyout
            flyout.ShowAt((FrameworkElement)sender, e.GetPosition((FrameworkElement)sender));
        }
    }
}
