using FireBrowserDataBase;
using FireBrowserDialogs.DialogTypes.AreYouSureDialog;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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

        private void FetchBrowserHistory()
        {
            Batteries.Init();

            try
            {
                // Create a connection to the SQLite database
                var connectionStringBuilder = new SqliteConnectionStringBuilder();
                connectionStringBuilder.DataSource = Path.Combine(ApplicationData.Current.LocalFolder.Path, "History.db");

                using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
                {
                    // Open the database connection
                    connection.Open();

                    // Define the SQL query to fetch the browser history
                    string sql = "SELECT url, title, visit_count, last_visit_time FROM urlsDb ORDER BY last_visit_time DESC";

                    // Create a command object with the SQL query and connection
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        // Execute the SQL query and get the results
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            // Create an observable collection to store the browser history items
                            browserHistory = new ObservableCollection<HistoryItem>();

                            // Iterate through the query results and create a HistoryItem for each row
                            while (reader.Read())
                            {
                                HistoryItem historyItem = new HistoryItem
                                {
                                    Url = reader.GetString(0),
                                    Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    VisitCount = reader.GetInt32(2),
                                    LastVisitTime = reader.GetString(3),
                                };

                                // Check if the item already exists in the collection before adding it
                                if (!browserHistory.Any(item => item.Url == historyItem.Url))
                                {
                                    // Add the item to the collection
                                    historyItem.ImageSource = new BitmapImage(new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + historyItem.Url + "&size=32"));
                                    browserHistory.Add(historyItem);
                                }
                            }
                        }
                    }

                    // Close the database connection explicitly
                    connection.Close();
                }
                BigTemp.ItemsSource = browserHistory;
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
                // User clicked "Cancel" button or closed the dialog
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
    }
}
