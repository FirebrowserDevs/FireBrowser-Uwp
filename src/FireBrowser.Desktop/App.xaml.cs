global using System;
global using System.IO;
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using FireBrowser.Core;
using System.Web;

namespace FireBrowser
{
    //To-Do: Prelaunch and optional splash screen
    public enum AppLaunchType
    {
        LaunchBasic,
        LaunchIncognito,
        LaunchStartup,
        FirstLaunch,
        FilePDF,
        FileHTML,
        URIHttp,
        URIAirplane,
    }
    public class AppLaunchPasser
    {
        public AppLaunchType LaunchType { get; set; }
        public object LaunchData { get; set; }
        public int ProfileID { get; set; }
    }
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            Dots.SDK.Core.projectID = "AirplaneDesktop";
        }
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            AppLaunchPasser passer = new()
            {
                //To-Do: temporary
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

        //Handles URI activation
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                Frame rootFrame = Window.Current.Content as Frame;

                if (rootFrame == null)
                {
                    var appData = await AppData.GetAppDataCore();

                    int profileID = appData.DefaultProfileID;
                    AppLaunchType kind = AppLaunchType.LaunchBasic;
                    if (eventArgs.Uri.Scheme == "airplane")
                    {
                        kind = AppLaunchType.URIAirplane;
                        //Part of the URL after airplane://
                        switch (eventArgs.Uri.Authority)
                        {
                            case "incognito":
                                kind = AppLaunchType.LaunchIncognito;
                                break;
                        }
                        var queries = HttpUtility.ParseQueryString(eventArgs.Uri.Query);
                        if (queries["profile"]?.Length > 0) profileID = int.Parse(queries["profile"]);
                    }
                    else
                    {
                        kind = AppLaunchType.URIHttp;

                        Dots.SDK.UWP.Log.WriteLog(eventArgs.Uri.Scheme);
                    }
                    AppLaunchPasser passer = new()
                    {
                        LaunchType = kind,
                        LaunchData = eventArgs.Uri,
                        ProfileID = profileID
                    };
                    AppData.currentProfileID = profileID;

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
                if (args.Kind == ActivationKind.StartupTask)
                {
                    var startupArgs = args as StartupTaskActivatedEventArgs;
                    payload = ActivationKind.StartupTask.ToString();
                }
                AppLaunchPasser passer = new()
                {
                    LaunchType = AppLaunchType.LaunchStartup,
                    LaunchData = payload
                };

                rootFrame.Navigate(typeof(MainPage), passer);
                Window.Current.Activate();
            }
        }
        /// <summary>
        /// Handles the file activation, so when the app is opened with a PDF or HTML file
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // CoreApplication.EnablePrelaunch was introduced in Windows 10 version 1607
            bool canEnablePrelaunch = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                var settings = await Core.Settings.GetSettingsDataAsync();

                // On Windows 10 version 1607 or later, this code signals that this app wants to participate in prelaunch
                if (canEnablePrelaunch)
                {
                    TryEnablePrelaunch();
                }
                AppLaunchPasser passer = new()
                {
                    LaunchType = AppLaunchType.LaunchBasic,
                    LaunchData = e.Arguments
                };
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    if (!settings.AccountData.FinishedFirstLaunch)
                        rootFrame.Navigate(typeof(Pages.FirstLaunchPage), e.User);
                    else
                        rootFrame.Navigate(typeof(MainPage), passer);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Wywoływane, gdy nawigacja do konkretnej strony nie powiedzie się
        /// </summary>
        /// <param name="sender">Ramka, do której nawigacja nie powiodła się</param>
        /// <param name="e">Szczegóły dotyczące niepowodzenia nawigacji</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Wywoływane, gdy wykonanie aplikacji jest wstrzymywane. Stan aplikacji jest zapisywany
        /// bez wiedzy o tym, czy aplikacja zostanie zakończona, czy wznowiona z niezmienioną zawartością
        /// pamięci.
        /// </summary>
        /// <param name="sender">Źródło żądania wstrzymania.</param>
        /// <param name="e">Szczegóły żądania wstrzymania.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Zapisz stan aplikacji i zatrzymaj wszelkie aktywności w tle
            deferral.Complete();
        }

        /// <summary>
        /// This method should be called only when the caller
        /// determines that we're running on a system that
        /// supports CoreApplication.EnablePrelaunch.
        /// </summary>
        private void TryEnablePrelaunch()
        {
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
        }
    }

}