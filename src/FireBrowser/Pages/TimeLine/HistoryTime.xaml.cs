using FireBrowserDataBase;
using FireBrowserDialogs.DialogTypes.AreYouSureDialog;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
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
        private async void FetchBrowserHistory()
        {
            Batteries.Init();
            try
            {
                using (var connection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/History.db;"))
                {
                    // Open the database connection
                    connection.Open();
                    // Open the database connection asynchronously
                    await connection.OpenAsync();

                    // Define the SQL query to fetch the browser history
                    string sql = "SELECT url, title, visit_count, last_visit_time FROM urlsDb ORDER BY last_visit_time DESC";

                    // Create a command object with the SQL query and connection
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            // Create an observable collection to store the browser history items
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


                            // Bind the browser history items to the ListView
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
                // User clicked "Cancel" button or closed the dialog
            }
        }

        private void Ts_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
