using FireBrowser.Core;
using FireBrowserCore.Models;
using FireBrowserDataBase;
using FireExceptions;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using Windows.Media.SpeechSynthesis;
using FireBrowser.Pages.HiddenFeatures;
using Microsoft.Toolkit.Uwp.Connectivity;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Toolkit.Uwp.Notifications;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebContent : Page
    {
        Passer param;
        public WebContent()
        {
            this.InitializeComponent();
            synthesizer = new SpeechSynthesizer();
        }



        public static bool IsIncognitoModeEnabled { get; set; } = false;
        private void ToggleIncognitoMode(object sender, RoutedEventArgs e)
        {
            IsIncognitoModeEnabled = !IsIncognitoModeEnabled;
        }
        public void LoadSettings()
        {
            var javasc = FireBrowserInterop.SettingsHelper.GetSetting("DisableJavaScript");
            var pass = FireBrowserInterop.SettingsHelper.GetSetting("DisablePassSave");
            var webmes = FireBrowserInterop.SettingsHelper.GetSetting("DisableWebMess");
            var autogen = FireBrowserInterop.SettingsHelper.GetSetting("DisableGenAutoFill");
            WebViewElement.CoreWebView2.Settings.IsScriptEnabled = javasc.Equals("false", StringComparison.OrdinalIgnoreCase);
            WebViewElement.CoreWebView2.Settings.IsPasswordAutosaveEnabled = pass.Equals("false", StringComparison.OrdinalIgnoreCase);
            WebViewElement.CoreWebView2.Settings.IsWebMessageEnabled = webmes.Equals("false", StringComparison.OrdinalIgnoreCase);
            WebViewElement.CoreWebView2.Settings.IsGeneralAutofillEnabled = autogen.Equals("false", StringComparison.OrdinalIgnoreCase);
        }

       

        public bool run = false;
        public void AfterComplete()
        {
            string url = WebViewElement.CoreWebView2.Source.ToString();
            string protocol = url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? "https://" : "http://";
            string baseUrl = protocol + url.Substring(protocol.Length);

            if (IsIncognitoModeEnabled)
            {

            }
            else
            {
                string address = WebViewElement.CoreWebView2.Source.ToString();
                string title = WebViewElement.CoreWebView2.DocumentTitle.ToString();
                var dbAddHis = new DbAddHis();
                _ = dbAddHis.AddHistData($"{address}", $"{title}");
            }

            if (WebViewElement.CoreWebView2.Source.Contains("https"))
            {
                param.ViewModel.SecurityIcon = "\uE72E";
                param.ViewModel.SecurityIcontext = "Https Secured Website";
                param.ViewModel.Securitytype = $"Link - {baseUrl}";
                param.ViewModel.Securitytext = "This Page Is Secured By A Valid SSL Certificate, Trusted By Root Authorities";
            }
            else if (WebViewElement.CoreWebView2.Source.Contains("http"))
            {
                param.ViewModel.SecurityIcon = "\uE785";
                param.ViewModel.SecurityIcontext = "Http UnSecured Website";
                param.ViewModel.Securitytype = $"Link - {baseUrl}";
                param.ViewModel.Securitytext = "This Page Is Unsecured By A Un-Valid SSL Certificate, Please Be Careful";
            }
        }

        private bool isOffline = false;
        private async void CheckNetworkStatus()
        {
            while (true)
            {
                if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
                {
                    if (isOffline)
                    {
                        // Internet is back online
                        WebViewElement.Reload();
                        Grid.Visibility = Visibility.Visible;
                        offlinePage.Visibility = Visibility.Collapsed;
                        isOffline = false;

                        new ToastContentBuilder()
                              .AddArgument("action", "viewConversation")
                              .AddArgument("conversationId", 9813)
                                  .AddButton(new ToastButton()
                                  .SetContent("Dismiss"))
                              .AddText("You're now online!")
                              .AddText("The Internet connection has been restored. You can now continue browsing.")
                              .AddAttributionText("via FireBrowser")
                              .AddAppLogoOverride(new Uri("ms-appx:///Assets/toast128.png"), ToastGenericAppLogoCrop.Circle)
                              .Show();
                    }
                }
                else
                {
                    // Internet is offline
                    offlinePage.Visibility = Visibility.Visible;
                    Grid.Visibility = Visibility.Collapsed;
                    isOffline = true;

                    // Wait for a second before checking again
                    await Task.Delay(1000);
                }

                // Wait for half a second before the next check
                await Task.Delay(500);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            await WebViewElement.EnsureCoreWebView2Async();

            LoadSettings();
            WebView2 s = WebViewElement;

            if (param?.Param != null)
            {
                WebViewElement.CoreWebView2.Navigate(param.Param.ToString());
            }

            var userAgent = s?.CoreWebView2.Settings.UserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                var edgIndex = userAgent.IndexOf("Edg/");
                if (edgIndex >= 0)
                {
                    if (IsIncognitoModeEnabled == true)
                    {
                        userAgent = userAgent.Substring(25, edgIndex - 25);
                        userAgent = userAgent.Replace("Chrome/115.0.0.0 Safari/537.36 Edg/115.0.1901.203", "NewIncog/1");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(userAgent))
                        {

                            if (edgIndex >= 0)
                            {
                                userAgent = userAgent.Substring(0, edgIndex - 0);
                                userAgent = userAgent.Replace("Chrome/115.0.0.0 Safari/537.36 Edg/115.0.1901.203", "Chrome/115.0.0.0 Safari/537.36 Edg/115.0.1901.203");
                                s.CoreWebView2.Settings.UserAgent = userAgent;
                            }
                        }
                    }
                }
            }
            s.CoreWebView2.Settings.UserAgent = userAgent;
            s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            s.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            s.CoreWebView2.Settings.IsStatusBarEnabled = true;
            s.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = true;
            s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            s.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            s.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            s.CoreWebView2.ScriptDialogOpening += async (sender, args) =>
            {
                await UI.ShowDialog("Message from this site", args.Message);
            };
            s.CoreWebView2.DocumentTitleChanged += (sender, args) =>
            {
                if (IsIncognitoModeEnabled == true)
                {
                    return;
                }
                else
                {
                    param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
                }
            };
            s.CoreWebView2.PermissionRequested += (sender, args) =>
            {
                try
                {
                    return;
                }
                catch (Exception ex)
                {
                    ExceptionsHelper.LogException(ex);
                }
            };
            s.CoreWebView2.FaviconChanged += async (sender, args) =>
            {
                if (IsIncognitoModeEnabled)
                {
                    return;
                }
                else
                {
                    try
                    {
                        var bitmapImage = new BitmapImage();
                        var stream = await sender.GetFaviconAsync(0);
                        if (stream != null)
                        {
                            await bitmapImage.SetSourceAsync(stream);
                            param.Tab.IconSource = new ImageIconSource { ImageSource = bitmapImage };
                        }
                        else
                        {
                            var bitmapImage2 = new BitmapImage();
                            await bitmapImage2.SetSourceAsync(await sender.GetFaviconAsync(CoreWebView2FaviconImageFormat.Jpeg));
                            param.Tab.IconSource = new ImageIconSource { ImageSource = bitmapImage2 };
                        }
                    }
                    catch
                    {
                        await UI.ShowDialog(".ico Not Supported Yet", "Test");
                    }
                }
            };
            s.CoreWebView2.NavigationStarting += (sender, args) =>
            {
                UseContent.MainPageContent.Hmbtn.IsEnabled = false;
                Progress.IsIndeterminate = true;
                Progress.Visibility = Visibility.Visible;
                param.ViewModel.CanRefresh = false;

                CheckNetworkStatus();
            };
            s.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                Progress.IsIndeterminate = false;
                Progress.Visibility = Visibility.Collapsed;
                UseContent.MainPageContent.Hmbtn.IsEnabled = true;
                param.ViewModel.CanRefresh = true;

                if (FireBrowser.Core.UseContent.MainPageContent.UrlBox.Text.Contains("drive"))
                {
                    WebViewElement.AllowDrop = false;
                }
                else
                {
                    WebViewElement.AllowDrop = true;
                }

                s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
                {
                    FullSys sys = new();
                    sys.FullScreen = s.CoreWebView2.ContainsFullScreenElement;
                };

                CheckNetworkStatus();
                AfterComplete();
            };
            s.CoreWebView2.SourceChanged += (sender, args) =>
            {
                if (param.TabView.SelectedItem == param.Tab)
                {
                    param.ViewModel.CurrentAddress = sender.Source;
                    param.ViewModel.Securitytype = sender.Source;
                }

            };
            s.CoreWebView2.NewWindowRequested += (sender, args) =>
            {
                MainPage mp = new();
                param?.TabView.TabItems.Add(mp.CreateNewTab(typeof(WebContent), args.Uri));
                args.Handled = true;
            };
        }


     

        string SelectionText;
        public void select()
        {
            FireBrowser.Core.UseContent.MainPageContent.SelectNewTab();
        }

     
        private void CoreWebView2_ContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            var flyout1 = (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
            OpenLinks.Visibility = Visibility.Collapsed;
            FlyoutBase.SetAttachedFlyout(WebViewElement, flyout1);
            var flyout = FlyoutBase.GetAttachedFlyout(WebViewElement);
            var options = new FlyoutShowOptions()
            {
                Position = args.Location,
                ShowMode = FlyoutShowMode.Standard
            };

            if (args.ContextMenuTarget.Kind == CoreWebView2ContextMenuTargetKind.SelectedText)
            {
                flyout = (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
                SelectionText = args.ContextMenuTarget.SelectionText;
                OpenLinks.Visibility = Visibility.Collapsed;
            }

            else if (args.ContextMenuTarget.HasLinkUri)
            {
                flyout = (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
                SelectionText = args.ContextMenuTarget.LinkText;
                SelectionText = args.ContextMenuTarget.LinkUri;
                OpenLinks.Visibility = Visibility.Visible;
            }


            flyout?.ShowAt(WebViewElement, options);
            args.Handled = true;
        }


        public static async void OpenNewWindow(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
        private async void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton button && button.Tag != null)
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
                        await WebViewElement.CoreWebView2.ExecuteScriptAsync("document.execCommand('selectAll', false, null);");
                        break;
                    case "Copy":
                        FireBrowserInterop.SystemHelper.WriteStringToClipboard(SelectionText);
                        break;
                    case "Taskmgr":
                        WebViewElement.CoreWebView2.OpenTaskManagerWindow();
                        break;
                    case "Save":

                        break;
                    case "Share":
                        FireBrowserInterop.SystemHelper.ShowShareUIURL(WebViewElement.CoreWebView2.DocumentTitle, WebViewElement.CoreWebView2.Source);
                        break;
                    case "Print":
                    
                        break;
                }
            }
            Ctx.Hide();
        }

        private SpeechSynthesizer synthesizer;

        public string OpTog = FireBrowserInterop.SettingsHelper.GetSetting("OpSw");
        public string lang = FireBrowserInterop.SettingsHelper.GetSetting("Lang");

        private async void ConvertTextToSpeech(string text)
        {    
             if (!string.IsNullOrWhiteSpace(text))
             {
                    var synthesisStream = await synthesizer.SynthesizeSsmlToStreamAsync(
                        $"<speak version='1.0' xml:lang='{lang}'><voice name='Microsoft Server Speech Text to Speech Voice ({lang}, HannaRUS)'>{text}</voice></speak>");

                    MediaElement mediaElement = new MediaElement();
                    mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
                    mediaElement.Play();
             }           
        }
        private void ContextClicked_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem button && button.Tag != null)
            {
                switch ((sender as MenuFlyoutItem).Tag)
                {
                    case "Read":
                        ConvertTextToSpeech(SelectionText);
                        break;
                    case "WebApp":

                        break;
                    case "OpenInTab":
                        if (OpTog == "True")
                        {
                            UseContent.MainPageContent.Tabs.TabItems.Add(UseContent.MainPageContent.CreateNewTab(typeof(WebContent), new Uri(SelectionText)));
                            select();
                        }
                        else if (OpTog == "0")
                        {
                            UseContent.MainPageContent.Tabs.TabItems.Add(UseContent.MainPageContent.CreateNewTab(typeof(WebContent), new Uri(SelectionText)));
                        }

                        break;
                    case "OpenInWindow":
                        OpenNewWindow(new Uri(SelectionText));
                        break;
                }
            }
            Ctx.Hide();
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (Grid.Children.Count == 0) Grid.Children.Add(WebViewElement);
        }

        private async void WebViewElement_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Any() && items[0] is StorageFile file)
                {
                    var fileStream = await file.OpenReadAsync();
                    using (var reader = new StreamReader(fileStream.AsStream()))
                    {
                        var fileContent = await reader.ReadToEndAsync();
                        WebViewElement.CoreWebView2.NavigateToString(fileContent);
                    }
                }
            }
        }

        private void WebViewElement_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }
    }
}
