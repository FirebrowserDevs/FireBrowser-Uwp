using FireBrowser.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages;

public sealed partial class WebViewPage : Page
{
    public WebViewPage()
    {
        this.InitializeComponent();
        WebViewControl.EnsureCoreWebView2Async();
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
        if (SettingsHelper.GetSetting("DisableJavaScript") == "true")
        {
            WebViewControl.CoreWebView2.Settings.IsScriptEnabled = false;
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
        // Add item to history file
        HistoryHelper.AddHistoryItem(sender.DocumentTitle, sender.Source);
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
            if (menuList[index].Name == "openLinkInNewWindow" || menuList[index].Name == "print" || menuList[index].Name == "emoji" || menuList[index].Name == "webCapture" || menuList[index].Name == "openLinkInNewWindow")
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
