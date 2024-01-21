using FireBrowser.Launch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using static FireBrowserCore.Overlay.AppOverlay;

namespace FireBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            LoadSettings();
            NullCheck();
            this.Suspending += OnSuspending;
            this.LeavingBackground += App_LeavingBackground;
        }

      
        string registerjm = FireBrowserInterop.SettingsHelper.GetSetting("regJump");

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            switch (registerjm)
            {
                case null:
                case "0":
                    register();
                    break;
                case "1":
                    return;
                default:
                    // Handle any other cases if needed
                    break;
            }

        }

        public void NullCheck()
        {
            SetDefaultColorSetting("ColorTool", "#000000");
            SetDefaultColorSetting("ColorTV", "#000000");
        }

        private void SetDefaultColorSetting(string settingName, string defaultValue)
        {
            var toolColor = FireBrowserInterop.SettingsHelper.GetSetting(settingName);
            if (string.IsNullOrEmpty(toolColor))
            {
                FireBrowserInterop.SettingsHelper.SetSetting(settingName, defaultValue);
            }
        }

        public static string IsFirstLaunch { get; set; }
        private void LoadSettings()
        {
            SearchUrl = FireBrowserInterop.SettingsHelper.GetSetting("SearchUrl");
            FireBrowserUrlHelper.TLD.LoadKnownDomains();
        }

        private void TryEnablePrelaunch()
        {
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            AppLaunchPasser passer = new()
            {
                LaunchType = AppLaunchType.FilePDF,
                LaunchData = args.Files
            };

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (rootFrame.Content == null) rootFrame.Navigate(typeof(MainPage), passer);

                Window.Current.Activate();
                Window.Current.Content = rootFrame;
            }
        }

        public async void register()
        {
            JumpList jumpList = await JumpList.LoadCurrentAsync();

            jumpList.Items.Clear();

            JumpListItem jumpListItemNewWindow = JumpListItem.CreateWithArguments("newwindow", "New Window");
            jumpListItemNewWindow.Description = "Open in a new window";
            jumpListItemNewWindow.Logo = new Uri("ms-appx:///Assets/logo.png");

            jumpList.Items.Add(jumpListItemNewWindow);

            FireBrowserInterop.SettingsHelper.SetSetting("regJump", "1");

            // Save the JumpList
            await jumpList.SaveAsync();
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol && args is ProtocolActivatedEventArgs protocolArgs)
            {
                Frame rootFrame = Window.Current.Content as Frame;


                Uri uri = protocolArgs.Uri;
                string query = uri.Query;

                // Parse the query string to extract the username
                string username = string.Empty;
                if (!string.IsNullOrEmpty(query))
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    string[] queryParts = query.TrimStart('?').Split('&');
                    foreach (string part in queryParts)
                    {
                        string[] keyValue = part.Split('=');
                        if (keyValue.Length == 2)
                        {
                            string key = Uri.UnescapeDataString(keyValue[0]);
                            string value = Uri.UnescapeDataString(keyValue[1]);
                            parameters[key] = value;
                        }
                    }

                    if (parameters.ContainsKey("username"))
                    {
                        username = parameters["username"];
                    }
                }

                if (rootFrame == null)
                {
                    AppLaunchType kind = AppLaunchType.LaunchBasic;

                    if (protocolArgs.Uri.Scheme == "firebrowser")
                    {
                        kind = AppLaunchType.URIFireBrowser;
                        //Part of the URL after firebrowser://
                        switch (protocolArgs.Uri.Authority)
                        {
                            case "incognito":
                                kind = AppLaunchType.LaunchIncognito;
                                break;
                            case "reset":
                                kind = AppLaunchType.Reset;
                                StorageFile fileToDelete = await ApplicationData.Current.LocalFolder.GetFileAsync("Params.json");
                                await fileToDelete.DeleteAsync();
                                await FireBrowserInterop.SystemHelper.RestartApp();
                                break;
                        }

                    }
                    else
                    {
                        kind = AppLaunchType.URIHttp;
                    }


                    AppLaunchPasser passer = new AppLaunchPasser()
                    {
                        LaunchType = kind,
                        LaunchData = protocolArgs.Uri,
                        UserName = username,
                    };

                    rootFrame = new Frame();
                    rootFrame.NavigationFailed += OnNavigationFailed;

                    rootFrame.Navigate(typeof(MainPage), passer);

                    Window.Current.Activate();
                    Window.Current.Content = rootFrame;
                }
            }
            else
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null)
                {
                    rootFrame = new Frame();
                    Window.Current.Content = rootFrame;
                }

                string payload = string.Empty;
                AppLaunchPasser passer = new AppLaunchPasser()
                {
                    LaunchType = AppLaunchType.LaunchStartup,
                    LaunchData = payload
                };

                if (args.Kind == ActivationKind.StartupTask)
                {
                    var startupArgs = args as StartupTaskActivatedEventArgs;
                    payload = ActivationKind.StartupTask.ToString();
                }

                rootFrame.Navigate(typeof(MainPage), passer);
                Window.Current.Activate();
            }
        }

        // Check if it's the first launch of the app
        public async Task<bool> CheckFirstLaunchAsync()
        {
            try
            {
                var settingsFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Params.json") as StorageFile;

                if (settingsFile == null)
                {
                    var settings = new AppSettings { IsFirstLaunch = true, IsConnected = false };
                    var settingsJson = JsonConvert.SerializeObject(settings);

                    settingsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Params.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(settingsFile, settingsJson);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred: " + ex.Message);
            }

            return false;
        }



        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            bool canEnablePrelaunch = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;

            Frame rootFrame = Window.Current.Content as Frame ?? new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                Debug.WriteLine("App Terminated", e.Arguments);
            }

            Window.Current.Content = rootFrame;

            if (!e.PrelaunchActivated)
            {
                AppLaunchPasser passer = new AppLaunchPasser()
                {
                    LaunchType = AppLaunchType.LaunchBasic,
                    LaunchData = e.Arguments
                };

                if (canEnablePrelaunch)
                {
                    TryEnablePrelaunch();
                }

                bool isFirstLaunch = await CheckFirstLaunchAsync();

                if (isFirstLaunch)
                {
                    rootFrame.Navigate(typeof(Setup));
                }
                else if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), passer);
                }

                Window.Current.Activate();
            }
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
       throw new Exception("Failed to load Page " + e.SourcePageType.FullName);

        private void OnSuspending(object sender, SuspendingEventArgs e) =>
            e.SuspendingOperation.GetDeferral().Complete();
    }
}
