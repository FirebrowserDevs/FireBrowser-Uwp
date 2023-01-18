using System.ComponentModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using FireBrowser.Core;
using Microsoft.Web.WebView2.Core;
using static FireBrowser.MainPage;
using Windows.Security.Authorization.AppCapabilityAccess;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Data.Sqlite;
using static FireBrowser.Core.AppData;
using Windows.UI.Xaml.Controls;
using FireBrowser.History;
using Windows.UI.Popups;
using Syncfusion.UI.Xaml.Controls.Input;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public partial class WebContent : Page
    {
       
        public WebContent()
        {
            this.InitializeComponent();
        }

        public class NotePasser
        {
            public string Base64 { get; set; }
            public int WebViewHeight { get; set; }
            public string WebViewWidth { get; set; }
            public int ScrollPos { get; set; }
            public Passer Passer { get; set; }
        }

      
        public async void PrepareForNoting()
        {

            var height = Convert.ToInt16(await WebViewElement.CoreWebView2.ExecuteScriptAsync("document.body.scrollHeight"));
            var width = await WebViewElement.CoreWebView2.ExecuteScriptAsync("document.body.scrollWidth");
            var PageParam = @"{""format"": ""png"", ""captureBeyondViewport"": true, ""clip"": {""x"": 0, ""y"": 0, ""width"":" + width + @", ""height"":" + height + @", ""scale"": 1.0" + "}}";
            var base64 = await WebViewElement.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", PageParam);


            var passer = new NotePasser()
            {
                Base64 = base64,
                Passer = param,
                WebViewHeight = height
            };
            Frame.Navigate(typeof(SiteNoting), base64);
        }
        Passer param;
        TaskCompletionSource<CoreWebView2PermissionState> flyoutPermissionState = new();
        public void PermissionChanged(bool allowed)
        {
            if (allowed) flyoutPermissionState.SetResult(CoreWebView2PermissionState.Allow);
            else flyoutPermissionState.SetResult(CoreWebView2PermissionState.Deny);
        }
        private class SizePasser
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }

        private bool fullScreen = false;

        private MainPage MainPage
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
                    MainPage.HideToolbar(false);
                }
                else
                {
                    view.TryEnterFullScreenMode();
                    MainPage.HideToolbar(true);
                    
                }
            }
        }

        public Controls.WebView WebViewElement = new();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            await WebViewElement.EnsureCoreWebView2Async();
            Controls.WebView s = WebViewElement;
            //the Param is the uri that the WebView should go to
            if (param?.Param != null)
            {
                Dots.SDK.UWP.Log.WriteLog(param.Param.ToString(), "WebViewParam");
                WebViewElement.CoreWebView2.Navigate(param.Param.ToString());
            }
            //sender.Source = new(param.Param.ToString());

            //MESS
            s.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            s.CoreWebView2.Settings.IsStatusBarEnabled = true;
            s.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            s.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            s.CoreWebView2.ContextMenuRequested += async (se, args) =>
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
                flyout?.ShowAt(s, options);
                if (args.ContextMenuTarget.HasSelection) { }//todo
                else { } //todo
                args.Handled = true;
            };
            s.CoreWebView2.ScriptDialogOpening += async (sender, args) =>
            {
                await UI.ShowDialog("Message from this site", args.Message);
            };
            s.CoreWebView2.DocumentTitleChanged += (sender, args) =>
            {
                if (WebViewElement.CoreWebView2.IsMuted == true)
                {
                    param.Tab.Header = "Muted -" + WebViewElement.CoreWebView2.DocumentTitle;
                }
                else
                {
                    param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
                }
                writeToDb();
            };
            s.CoreWebView2.PermissionRequested += async (sender, args) =>
            {
                //required for some reason, won't work otherwise
                string permissionName = string.Empty;
                bool hasWindowsPermission = false;
                var def = args.GetDeferral();
                switch (args.PermissionKind)
                {
                    case CoreWebView2PermissionKind.Camera:
                        var camera = await AppCapability.Create("webcam").RequestAccessAsync();
                        permissionName = Core.Resources.resourceLoader.GetString("PermissionsCamera");
                        if (camera == AppCapabilityAccessStatus.Allowed)
                        {
                            //To-Do: keep a list of sites that can/can't access a permission.
                            //If this is the first time the user sees the Windows consent dialog,
                            //don't show the flyout for the 2nd time.
                            //If the user removes or denies the permission, show a dialog messaging them
                            //about that with instructions on how to make it work again
                            hasWindowsPermission = true;
                        }
                        if (camera == AppCapabilityAccessStatus.DeniedByUser)
                        {
                            await UI.ShowDialog("Camera access not allowed by user", args.State.ToString());
                            hasWindowsPermission = false;
                        }
                        break;
                }
                if (hasWindowsPermission)
                {
                    param.ViewModel.PermissionDialogTitle =
                    string.Format(Core.Resources.resourceLoader.GetString("PermissionDialogTitleSingle"),
                    new Uri(sender.Source).Host, permissionName);
                    param.ViewModel.PermissionDialogIsOpen = true;
                }
                var t = await flyoutPermissionState.Task;

                if (t == CoreWebView2PermissionState.Allow)
                {
                    args.State = CoreWebView2PermissionState.Allow;
                }
                else
                {
                    args.State = CoreWebView2PermissionState.Deny;
                }
                def.Complete();
                //flyoutPermissionState.SetResult(CoreWebView2PermissionState.Default);
            };
            s.CoreWebView2.FaviconChanged += async (sender, args) =>
            {
                try
                {
                    BitmapImage bitmapImage = new();
                    await bitmapImage.SetSourceAsync(await sender.GetFaviconAsync(0));
                    param.Tab.IconSource = new ImageIconSource() { ImageSource = bitmapImage };
                }
                catch { }
            };
            s.CoreWebView2.NavigationStarting += (sender, args) => {
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

                param.ViewModel.LoadingState = new FontIcon()
                {
                    Glyph = "\uF13E",
                    FontSize = 16
                };

                WebViewElement.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
                {
                    this.FullScreen = WebViewElement.CoreWebView2.ContainsFullScreenElement;
                };
            };
            s.CoreWebView2.SourceChanged += (sender, args) =>
            {
                if (param.TabView.SelectedItem == param.Tab)
                {
                    param.ViewModel.CurrentAddress = sender.Source;
                    param.ViewModel.SecurityIcon = sender.Source.Contains("https") ? "\uE72E" : "\uE785";
                }
                if (Core.Focusing.isFocusing)
                {
                    var uri = new Uri(new Uri(sender.Source).GetLeftPart(UriPartial.Authority));
                    //To-Do: the Uri class has other interesting functions, we can use those for some cool features
                    if (Core.UserData.currentProfile.Focus[Core.Focusing.selectedList].SiteList.Contains(uri))
                    {
                        Debug.WriteLine("Hello");
                        Frame.Navigate(typeof(Focus.BlockedSite));
                    }
                    else
                    {
                        foreach(var item in Core.UserData.currentProfile.Focus[Core.Focusing.selectedList].SiteList)
                        {
                            Debug.WriteLine("Current item is: " + item);
                            Debug.WriteLine("Current uri is: " + uri);
                        }
                    }
                }
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

        public void writeToDb()
        {
            string url = WebViewElement.CoreWebView2.Source;
            string title = WebViewElement.CoreWebView2.DocumentTitle;

            using (var con = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserData/{AppData.CurrentProfileCore.FriendlyID}/FireBrowserHistory.Db"))
            {
                con.Open();

                using (var cmd = new SqliteCommand("INSERT INTO urls(url, title, last_visit_time) VALUES(@url, @title, @last_visit_time)", con))
                {
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@last_visit_time", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as AppBarButton).Tag)
            {
                case "MenuBack":
                    if(WebViewElement.CanGoBack == true)
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
                case "Read":

                    break;
                case "Save":

                    break;
                case "Share":

                    break;
                case "Print":
                    WebViewElement.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.System);
                    break;
            }
        }


        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Grid.Children.Count == 0) Grid.Children.Add(WebViewElement);

            await WebViewElement.EnsureCoreWebView2Async();

        }
    }
}
