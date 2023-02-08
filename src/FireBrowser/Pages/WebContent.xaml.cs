using FireBrowser.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public Controls.WebView WebViewElement = new();
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
            Controls.WebView s = WebViewElement;

            //the Param is the uri that the WebView should go to
            if (param?.Param != null)
            {
                WebViewElement.CoreWebView2.Navigate(param.Param.ToString());
            }
            //sender.Source = new(param.Param.ToString());

            //MESS
            s.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            s.CoreWebView2.Settings.IsStatusBarEnabled = true;
            s.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = true;
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
               
            };
            s.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                if (!args.IsSuccess)
                {

                }


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
                      SystemHelper.ShowShareUIURL(WebViewElement.CoreWebView2.DocumentTitle, WebViewElement.CoreWebView2.Source);
                    break;
                case "Print":
                   
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
