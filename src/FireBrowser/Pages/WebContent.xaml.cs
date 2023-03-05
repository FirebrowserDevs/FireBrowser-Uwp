using FireBrowser.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.UI.Interop;
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

        Passer param;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            await WebViewElement.EnsureCoreWebView2Async();
            WebView2 s = WebViewElement;
            //the Param is the uri that the WebView should go to

            var PageParam = @"{""format"": ""png"", ""captureBeyondViewport"": true, ""clip"": {""x"": 0, ""y"": 0, ""width"":" + "document.body.scrollWidth" + @", ""height"":" + "document.body.scrollHeight" + @", ""scale"": 1.0" + "}}";
            WebViewElement.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", PageParam);

            if (launchurl != null)
            {
                WebViewElement.Source = new Uri(launchurl);
                launchurl = null;
            }
            //sender.Source = new(param.Param.ToString());
            var userAgent = s?.CoreWebView2.Settings.UserAgent;
            userAgent = userAgent.Substring(0, userAgent.IndexOf("Edg/"));
            userAgent = userAgent.Replace("Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63", "Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63");
            s.CoreWebView2.Settings.UserAgent = userAgent;
            //MESS
            s.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            s.CoreWebView2.Settings.IsStatusBarEnabled = true;
            s.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = true;
            s.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

            s.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
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
            };
            s.CoreWebView2.PermissionRequested += async (sender, args) =>
            {
               
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

                s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
                {
                    this.FullScreen = s.CoreWebView2.ContainsFullScreenElement;
                };


            };
            s.CoreWebView2.SourceChanged += (sender, args) =>
            {
                if (param.TabView.SelectedItem == param.Tab)
                {
                    param.ViewModel.CurrentAddress = sender.Source;
                    param.ViewModel.SecurityIcon = sender.Source.Contains("https") ? "\uE72E" : "\uE785";
                }              
            };
            s.CoreWebView2.NewWindowRequested += (sender, args) =>
            {
                //To-Do: Check if it should be a popup or tab. Can use args.something for that.
                //To-Do: Get the currently selected tab's position and launch the new one next to it
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Not Working Is Alpha Currently";
                dialog.PrimaryButtonText = "OK";      
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.ShowAsync();

                MainPage mp = new();
                param?.TabView.TabItems.Add(mp.CreateNewTab(typeof(WebContent), args.Uri));
                args.Handled = true;
            };
        }

        string SelectionText;
        string LinkUri;
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
            flyout?.ShowAt(WebViewElement, options);
            if (args.ContextMenuTarget.HasSelection) { }//todo
            else { } //todo
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
