using FireBrowser.Core;
using FireBrowser.Launch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.Core.UserData;
using static FireBrowser.MainPage;

namespace FireBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            LoadSettings();
        }

        public enum AppLaunchType
        {
            LaunchBasic,
            LaunchIncognito,
            LaunchStartup,
            FirstLaunch,
            FilePDF,
            FileHTML,
            URIHttp,
            URIFireBrowser,
        }
        public class AppLaunchPasser
        {
            public AppLaunchType LaunchType { get; set; }
            public object LaunchData { get; set; }
        }

        public static string IsFirstLaunch { get; set; }
        private void LoadSettings()
        {
            SearchUrl = SettingsHelper.GetSetting("SearchUrl");
            TLD.LoadKnownDomains();
        }

        private void TryEnablePrelaunch()
        {
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                Frame rootFrame = Window.Current.Content as Frame;

                if (rootFrame == null)
                {
                    var startup = await StartupTask.GetAsync("FireBrowserStartUp");

                    AppLaunchPasser passer = new()
                    {
                        LaunchData = eventArgs.Uri
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
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // CoreApplication.EnablePrelaunch was introduced in Windows 10 version 1607
            bool canEnablePrelaunch = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");

            // NOTE: Only enable this code if you are targeting a version of Windows 10 prior to version 1607,
            // and you want to opt out of prelaunch.
            // In Windows 10 version 1511, all UWP apps were candidates for prelaunch.
            // Starting in Windows 10 version 1607, the app must opt in to be prelaunched.
            //if ( !canEnablePrelaunch && e.PrelaunchActivated == true)
            //{
            //    return;
            //}

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
               
                // On Windows 10 version 1607 or later, this code signals that this app wants to participate in prelaunch
                if (canEnablePrelaunch)
                {
                    TryEnablePrelaunch();
                }

                IsFirstLaunch = SettingsHelper.GetSetting("LaunchFirst");
                if (IsFirstLaunch == "1")
                {
                   
                    if (rootFrame == null)
                    {
                        rootFrame = new Frame();
                        Window.Current.Content = rootFrame;
                    }

                    rootFrame.Navigate(typeof(Setup));
                    Window.Current.Activate();
                }
                else
                {
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
