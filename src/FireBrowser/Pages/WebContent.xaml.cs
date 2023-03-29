using FireBrowser.Core;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebContent : Page
    {
        public WebContent()
        {
            this.InitializeComponent();
        }

        private bool fullScreen = false;
        public static MainPage MainPageContent
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        [DefaultValue(false)]
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                ApplicationView view = ApplicationView.GetForCurrentView();
                bool isfullmode = view.IsFullScreenMode;
                if (isfullmode)
                {
                    view.ExitFullScreenMode();
                    MainPageContent.HideToolbar(false);
                }
                else
                {
                    view.TryEnterFullScreenMode();
                    MainPageContent.HideToolbar(true);
                }
            }
        }

        string javasc = FireBrowserInterop.SettingsHelper.GetSetting("DisableJavaScript");
        string pass = FireBrowserInterop.SettingsHelper.GetSetting("DisablePassSave");
        string webmes = FireBrowserInterop.SettingsHelper.GetSetting("DisableWebMess");
        string autogen = FireBrowserInterop.SettingsHelper.GetSetting("DisableGenAutoFill");

        Passer param;
        public static bool IsIncognitoModeEnabled { get; set; } = false;
        private void ToggleIncognitoMode(object sender, RoutedEventArgs e)
        {
            IsIncognitoModeEnabled = !IsIncognitoModeEnabled;
        }
        public async void loadSetting()
        {
            if (javasc.Equals("true"))
            {
                WebViewElement.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            }
            else
            {
                WebViewElement.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
            }
            if (pass.Equals("true"))
            {
                WebViewElement.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            }
            else
            {
                WebViewElement.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            }
            if (webmes.Equals("true"))
            {
                WebViewElement.CoreWebView2.Settings.IsWebMessageEnabled = false;
            }
            else
            {
                WebViewElement.CoreWebView2.Settings.IsWebMessageEnabled = true;
            }
            if (autogen.Equals("true"))
            {
                WebViewElement.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            }
            else
            {
                WebViewElement.CoreWebView2.Settings.IsGeneralAutofillEnabled = true;
            }
        }

        public void checkif()
        {
            if (WebViewElement.CoreWebView2.CanGoBack == true)
            {
                MainPageContent.Back.IsEnabled = true;
            }
            else
            {
                MainPageContent.Back.IsEnabled = false;
            }
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            await WebViewElement.EnsureCoreWebView2Async();
            WebView2 s = WebViewElement;

            if (param?.Param != null)
            {
                WebViewElement.CoreWebView2.Navigate(param.Param.ToString());
            }
           
            var userAgent = s?.CoreWebView2.Settings.UserAgent;
            userAgent = userAgent.Substring(1, userAgent.IndexOf("Edg/"));
            userAgent = userAgent.Replace("Chrome/111.0.0.0 Safari/537.36 Edg/111.0.1661.51", "Chrome/111.0.0.0 Safari/537.36 Edg/111.0.1661.51");
            s.CoreWebView2.Settings.UserAgent = userAgent;


            loadSetting();
            s.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            s.CoreWebView2.Settings.IsStatusBarEnabled = true;
            s.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = true;
            s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

            s.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            s.CoreWebView2.ScriptDialogOpening += async (sender, args) =>
            {
                await UI.ShowDialog("Message from this site", args.Message);
            };
            s.CoreWebView2.DocumentTitleChanged += (sender, args) =>
            {
                if (IsIncognitoModeEnabled == true)
                {

                }
                else
                {
                    param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
                }
            };
            s.CoreWebView2.PermissionRequested += async (sender, args) =>
            {

            };
            s.CoreWebView2.FaviconChanged += async (sender, args) =>
            {
                if (IsIncognitoModeEnabled == true)
                {

                }
                else
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        await bitmapImage.SetSourceAsync(await sender.GetFaviconAsync(0));
                        param.Tab.IconSource = new ImageIconSource() { ImageSource = bitmapImage };
                    }
                    catch { }
                }
            };
            s.CoreWebView2.NavigationStarting += async (sender, args) =>
            {
                param.ViewModel.LoadingState = new Microsoft.UI.Xaml.Controls.ProgressRing()
                {
                    Width = 16,
                    Height = 16
                };
            };

            s.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                if (!args.IsSuccess)
                {

                }
                else
                {

                }

                param.ViewModel.LoadingState = new FontIcon()
                {
                    Glyph = "\uE72C",
                    FontSize = 16
                };

                s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
                {
                    this.FullScreen = s.CoreWebView2.ContainsFullScreenElement;
                };

                if (IsIncognitoModeEnabled == true)
                {

                }
                else
                {
                    AddHistData();
                }

            };
            s.CoreWebView2.SourceChanged += (sender, args) =>
            {
                if (param.TabView.SelectedItem == param.Tab)
                {
                    param.ViewModel.CurrentAddress = sender.Source;
                }
                //checkif();
            };
            s.CoreWebView2.NewWindowRequested += (sender, args) =>
            {
                //To-Do: Check if it should be a popup or tab. Can use args.something for that.
                //To-Do: Get the currently selected tab's position and launch the new one next to it

                MainPage mp = new();
                param?.TabView.TabItems.Add(mp.CreateNewTab(typeof(WebContent), args.Uri));
                args.Handled = true;
            };
        }
        string SelectionText;
        public void select()
        {
            MainPageContent.SelectNewTab();
        }
        public void AddHistData()
        {
            string address = WebViewElement.CoreWebView2.Source.ToString();
            string title = WebViewElement.CoreWebView2.DocumentTitle;
            SqliteConnection m_dbConnection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/History.db;");
            m_dbConnection.Open();

            var selectCmd = m_dbConnection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM urlsDb WHERE url = @url AND title = @title";
            selectCmd.Parameters.AddWithValue("@url", address);
            selectCmd.Parameters.AddWithValue("@title", title);

            var reader = selectCmd.ExecuteReader();
            if (reader.Read())
            {
                var updateCmd = m_dbConnection.CreateCommand();
                updateCmd.CommandText = "UPDATE urlsDb SET visit_count = visit_count + 1, last_visit_time = @lastVisitTime WHERE url = @url AND title = @title";
                updateCmd.Parameters.AddWithValue("@url", address);
                updateCmd.Parameters.AddWithValue("@title", title);
                updateCmd.Parameters.AddWithValue("@lastVisitTime", DateTimeOffset.Now.ToUnixTimeSeconds());
                updateCmd.ExecuteNonQuery();
            }
            else
            {
                var insertCmd = m_dbConnection.CreateCommand();
                insertCmd.CommandText = "INSERT INTO urlsDb (url, title, visit_count, last_visit_time) VALUES (@url, @title, @visitCount, @lastVisitTime)";
                insertCmd.Parameters.AddWithValue("@url", address);
                insertCmd.Parameters.AddWithValue("@title", title);
                insertCmd.Parameters.AddWithValue("@visitCount", 1);
                insertCmd.Parameters.AddWithValue("@lastVisitTime", DateTimeOffset.Now.ToUnixTimeSeconds());
                insertCmd.ExecuteNonQuery();
            }

            m_dbConnection.Close();

        }
        private void CoreWebView2_ContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            var flyout1 = (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
            FlyoutBase.SetAttachedFlyout(WebViewElement, flyout1);
            var flyout = FlyoutBase.GetAttachedFlyout(WebViewElement);
            var options = new FlyoutShowOptions()
            {
                // Position shows the flyout next to the pointer.
                // "Transient" ShowMode makes the flyout open in its collapsed state.
                Position = args.Location,
                ShowMode = FlyoutShowMode.Standard
            };

            if (args.ContextMenuTarget.Kind == CoreWebView2ContextMenuTargetKind.SelectedText)
            {
                flyout = (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
                SelectionText = args.ContextMenuTarget.SelectionText;
            }

            else if (args.ContextMenuTarget.HasLinkUri)
            {
                flyout = (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
                SelectionText = args.ContextMenuTarget.LinkText;
                SelectionText = args.ContextMenuTarget.LinkUri;
            }

            flyout?.ShowAt(WebViewElement, options);
            args.Handled = true;
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as AppBarButton).Tag)
            {
                case "MenuBack":
                    if (WebViewElement.CanGoBack == true)
                    {
                        WebViewElement.CoreWebView2.GoBack();
                    }
                    break;
                case "Forward":
                    if (WebViewElement.CanGoForward == true)
                    {
                        WebViewElement.CoreWebView2.GoForward();
                    }
                    break;
                case "Source":
                    WebViewElement.CoreWebView2.OpenDevToolsWindow();
                    break;
                case "Select":

                    break;
                case "Copy":
                    FireBrowserInterop.SystemHelper.WriteStringToClipboard(SelectionText);
                    break;
                case "Taskmgr":
                    WebViewElement.CoreWebView2.OpenTaskManagerWindow();
                    break;
                case "Read":

                    break;
                case "Save":

                    break;
                case "Share":
                    FireBrowserInterop.SystemHelper.ShowShareUIURL(WebViewElement.CoreWebView2.DocumentTitle, WebViewElement.CoreWebView2.Source);
                    break;
                case "Print":

                    break;
            }
            Ctx.Hide();
        }

        private async void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (Grid.Children.Count == 0) Grid.Children.Add(WebViewElement);
        }
    }
}
