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

        public async void PrepareForNoting()
        {
            var height = Convert.ToInt16(await WebViewElement.ExecuteScriptAsync("document.body.scrollHeight"));
            var width = await WebViewElement.ExecuteScriptAsync("document.body.scrollWidth");
            var PageParam = @"{""format"": ""png"", ""captureBeyondViewport"": true, ""clip"": {""x"": 0, ""y"": 0, ""width"":" + width + @", ""height"":" + height + @", ""scale"": 1.0" + "}}";
        }

            Passer param;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            await WebViewElement.EnsureCoreWebView2Async();
            WebView2 s = WebViewElement;
            //the Param is the uri that the WebView should go to

            if (param?.Param != null)
            {
                WebViewElement.CoreWebView2.Navigate(param.Param.ToString());
            }
            //sender.Source = new(param.Param.ToString());
            var userAgent = s?.CoreWebView2.Settings.UserAgent;
            userAgent = userAgent.Substring(0, userAgent.IndexOf("Edg/"));
            userAgent = userAgent.Replace("Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.46", "Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.46");
            s.CoreWebView2.Settings.UserAgent = userAgent;
            //MESS
            s.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            s.CoreWebView2.Settings.IsStatusBarEnabled = true;
            s.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = true;
            s.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

            s.CoreWebView2.ContextMenuRequested += async (se, args) =>
            {
               
            };
            s.CoreWebView2.ScriptDialogOpening += async (sender, args) =>
            {
             
            };
            s.CoreWebView2.DocumentTitleChanged += (sender, args) =>
            {
                param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
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

        private async void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (Grid.Children.Count == 0) Grid.Children.Add(WebViewElement);

            await WebViewElement.EnsureCoreWebView2Async();
        }
    }
}
