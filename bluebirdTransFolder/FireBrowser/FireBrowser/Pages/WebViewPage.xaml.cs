using FireBrowser.Core;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using System.ServiceModel.Channels;
using System.Security.Authentication.ExtendedProtection;
using Windows.ApplicationModel.Background;
using Windows.Services.Maps;
using Windows.UI.Xaml.Documents;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Shapes;

namespace FireBrowser.Pages;

public sealed partial class WebViewPage : Page
{
    public static event System.EventHandler<EnteredBackgroundEventArgs> EnteredBackground;
    public static event System.EventHandler<LeavingBackgroundEventArgs> ExitBackground;

    public WebViewPage()
    {
        this.InitializeComponent();
        WebViewControl.EnsureCoreWebView2Async();

        EnteredBackground += WebViewPage_EnteredBackground;
        ExitBackground += WebViewPage_ExitBackground;
    }

    private void WebViewPage_ExitBackground(object sender, LeavingBackgroundEventArgs e)
    {
        
    }

    private void WebViewPage_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
    {
       
    }

    #region WebViewEvents
    private void WebViewControl_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
    {
        // WebViewEvents
        sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
        sender.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        sender.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
        sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
        // Apply WebView2 settings
        
        ApplyWebView2Settings();
        if (launchurl != null)
        {
            WebViewControl.Source = new Uri(launchurl);
            launchurl = null;
        }
    }

    private void ApplyWebView2Settings()
    {
        if (SettingsHelper.GetSetting("DisableJavaScript") is "true")
        {
            WebViewControl.CoreWebView2.Settings.IsScriptEnabled = false;
        }
        if (SettingsHelper.GetSetting("SwipeNav") is "true")
        {
            WebViewControl.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
        }
        if (SettingsHelper.GetSetting("GenAutoFill") is "true")
        {
            WebViewControl.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
        }
        if (SettingsHelper.GetSetting("WebMess") is "true")
        {
            WebViewControl.CoreWebView2.Settings.IsWebMessageEnabled = false;
        }
        if (SettingsHelper.GetSetting("PassSave") is "true")
        {
            WebViewControl.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
        }
    }

    private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
    {
        MainPageContent.LoadingRing.IsActive = true;
    }
    private void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        MainPageContent.LoadingRing.IsActive = false;
        MainPageContent.SelectedTab.IconSource = FaviconHelper.GetFavicon(sender.Source);
    }
    private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        launchurl = args.Uri;
        MainPageContent.CreateWebTab();
        args.Handled = true;
    }

    private void CoreWebView2_ContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
    {
        IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;
        //MainPageContent.JsonListView.ItemsSource = menuList;
        for (int index = 0; index < menuList.Count; index++)
        {
            if (menuList[index].Name == "openLinkInNewWindow" || menuList[index].Name == "print" || menuList[index].Name == "emoji" || menuList[index].Name == "webCapture")
            {
                menuList.RemoveAt(index);
            }
        }
    }

    private void CoreWebView2_DocumentTitleChanged(CoreWebView2 sender, object args)
    {
        MainPageContent.SelectedTab.Header = sender.DocumentTitle;
    }
    #endregion
}
