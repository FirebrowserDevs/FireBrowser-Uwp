using FireBrowser.Core;
using FireBrowserCore.Models;
using FireBrowserDataBase;
using FireExceptions;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Media.SpeechSynthesis;
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

        [DefaultValue(false)]
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                ApplicationView view = ApplicationView.GetForCurrentView();
                if (value)
                {
                    try
                    {
                        view.TryEnterFullScreenMode();
                        FireBrowser.Core.UseContent.MainPageContent.HideToolbar(true);
                    }
                    catch (Exception ex)
                    {
                        FireExceptions.ExceptionsHelper.LogException(ex);
                    }
                }
                else
                {
                    try
                    {
                        view.ExitFullScreenMode();
                        FireBrowser.Core.UseContent.MainPageContent.HideToolbar(false);
                    }
                    catch (Exception ex)
                    {
                        FireExceptions.ExceptionsHelper.LogException(ex);
                    }
                }
                fullScreen = value;
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
                WebViewElement.CoreWebView2.Settings.IsScriptEnabled = false;
            }
            else
            {
                WebViewElement.CoreWebView2.Settings.IsScriptEnabled = true;
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

        public class BaseViewModel : INotifyPropertyChanged
        {
            private bool _isLoading;

            public bool IsLoading
            {
                get { return _isLoading; }
                set
                {
                    if (_isLoading != value)
                    {
                        _isLoading = value;
                        OnPropertyChanged(nameof(IsLoading));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;

            await WebViewElement.EnsureCoreWebView2Async();

            loadSetting();
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
                        userAgent = userAgent.Replace("Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.64", "NewIncog/1");
                    }
                    else
                    {
                        // Modify the user agent string as needed

                        if (!string.IsNullOrEmpty(userAgent))
                        {

                            if (edgIndex >= 0)
                            {
                                userAgent = userAgent.Substring(0, edgIndex - 0);
                                userAgent = userAgent.Replace("Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.64", "Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.64");
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

                }
                else
                {
                    param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
                }
            };
            s.CoreWebView2.PermissionRequested += async (sender, args) =>
            {
                try
                {

                }
                catch (Exception ex)
                {
                    ExceptionsHelper.LogException(ex);
                }
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
                        BitmapImage bitmapImage = new BitmapImage();
                        var stream = await sender.GetFaviconAsync(0);
                        if (stream != null)
                        {
                            await bitmapImage.SetSourceAsync(stream);
                            param.Tab.IconSource = new ImageIconSource() { ImageSource = bitmapImage };
                        }
                        else
                        {
                            BitmapImage bitmapImage2 = new BitmapImage();
                            await bitmapImage2.SetSourceAsync(await sender.GetFaviconAsync(CoreWebView2FaviconImageFormat.Jpeg));
                            param.Tab.IconSource = new ImageIconSource() { ImageSource = bitmapImage2 };
                        }
                    }
                    catch
                    {
                        await UI.ShowDialog(".ico Not Supported Yet", "Test");
                    }
                }
            };
            s.CoreWebView2.NavigationStarting += async (sender, args) =>
            {
                UseContent.MainPageContent.Hmbtn.IsEnabled = false;
                Progress.IsIndeterminate = true;
                Progress.Visibility = Visibility.Visible;
            };

            s.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                Progress.IsIndeterminate = false;
                Progress.Visibility = Visibility.Collapsed;
                UseContent.MainPageContent.Hmbtn.IsEnabled = true;

                if (FireBrowser.Core.UseContent.MainPageContent.UrlBox.Text.Contains("drive"))
                {
                    WebViewElement.AllowDrop = false;
                }

                s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
                {
                    this.FullScreen = s.CoreWebView2.ContainsFullScreenElement;
                };

                if (IsIncognitoModeEnabled == true)
                {

                }
                else
                {
                    string address = WebViewElement.CoreWebView2.Source.ToString();
                    string title = WebViewElement.CoreWebView2.DocumentTitle.ToString();
                    var dbAddHis = new DbAddHis();
                    dbAddHis.AddHistData(address, title);
                }
            };
            s.CoreWebView2.SourceChanged += (sender, args) =>
            {
                if (param.TabView.SelectedItem == param.Tab)
                {
                    param.ViewModel.CurrentAddress = sender.Source;
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
                // Position shows the flyout next to the pointer.
                // "Transient" ShowMode makes the flyout open in its collapsed state.
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

        private async Task ReadAloudAsync()
        {
            try
            {
                // Get the currently focused element in the web view
                var focusedElementId = await WebViewElement.CoreWebView2.ExecuteScriptAsync("document.activeElement.id");

                // Get the text content of the focused element
                var textContent = await WebViewElement.CoreWebView2.ExecuteScriptAsync($"document.getElementById('{focusedElementId}').textContent");

                // Create a list of voices for the system language
                var systemLanguage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
                var voices = SpeechSynthesizer.AllVoices.Where(v => v.Language.Contains(systemLanguage)).ToList();

                // If there are no voices for the system language, use the default language
                if (voices.Count == 0)
                {
                    voices = (List<VoiceInformation>)SpeechSynthesizer.AllVoices;
                }

                // Create a new speech synthesizer
                var synthesizer = new SpeechSynthesizer();

                // Set the language for the speech synthesis
                synthesizer.Voice = voices.First();

                // Speak the text content
                SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(textContent);
                MediaElement mediaElement = new MediaElement();
                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading aloud: {ex.Message}");
            }
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
                        WebViewElement.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
                        break;
                }
            }
            Ctx.Hide();
        }

        private async void ContextClicked_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem button && button.Tag != null)
            {
                switch ((sender as MenuFlyoutItem).Tag)
                {
                    case "Read":
                        await ReadAloudAsync();
                        break;
                    case "WebApp":

                        break;
                    case "OpenInTab":
                        UI.ShowDialog("Disabled", "Coming Soon");
                        break;
                    case "OpenInWindow":
                        OpenNewWindow(new Uri(SelectionText));
                        break;
                }
            }
            Ctx.Hide();
        }

        private async void Grid_Loaded_1(object sender, RoutedEventArgs e)
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

        private async void WebViewElement_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }
    }
}
