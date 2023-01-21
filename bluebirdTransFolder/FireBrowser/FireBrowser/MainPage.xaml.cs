using Microsoft.UI.Xaml.Controls;
using FireBrowser.Shared;
using System;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using FireBrowser.Pages;
using FireBrowser.Core;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using static FireBrowser.Core.DataAccess;
using System.Collections.Generic;

namespace FireBrowser;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        CustomTitleBar();
        Window.Current.VisibilityChanged += WindowVisibilityChangedEventHandler;
    }

    void WindowVisibilityChangedEventHandler(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
    {
        // Perform operations that should take place when the application becomes visible rather than
        // when it is prelaunched, such as building a what's new feed
        if (Tabs.TabItems.Count == 0) CreateHomeTab();
    }
    private void CustomTitleBar()
    {
        // Hide default title bar.
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.ExtendViewIntoTitleBar = true;
        // Set custom XAML element as titlebar
        Window.Current.SetTitleBar(CustomDragRegion);
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        // Set colors
        titleBar.ButtonBackgroundColor = Colors.Transparent;
    }

    private async void SidebarButton_Click(object sender, RoutedEventArgs e)
    {
        try {
            switch ((sender as Button).Tag)
            {
                case "Back":
                    TabWebView.GoBack();
                    break;
                case "Refresh":
                    TabWebView.Reload();
                    break;
                case "Forward":
                    TabWebView.GoForward();
                    break;
                case "Search":
                    UrlBox.Text = TabWebView.CoreWebView2.Source;
                    UrlBox.Focus(FocusState.Programmatic);
                    break;
                case "ReadingMode":
                    string jscript = await ReadingModeHelper.GetReadingModeJScriptAsync();
                    await TabWebView.CoreWebView2.ExecuteScriptAsync(jscript);
                    break;
                case "Translate":
                    string url = TabWebView.CoreWebView2.Source;
                    TabWebView.CoreWebView2.Navigate("https://translate.google.com/translate?hl&u=" + url);
                    break;
                case "AddFavoriteFlyout":
                    FavoriteTitle.Text = TabWebView.CoreWebView2.DocumentTitle;
                    FavoriteUrl.Text = TabWebView.CoreWebView2.Source;
                    break;
                case "AddFavorite":
                    FavoritesHelper.AddFavoritesItem(FavoriteTitle.Text, FavoriteUrl.Text);
                    AddFavoriteFlyout.Hide();
                    break;
                case "Favorites":
                    LoadListFromJson("Favorites.json");
                    break;
                case "History":
                    ShowHistory();
                    break;
            }
        }
        catch
        {
            
        }
    }

    private async void MoreFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        switch ((sender as MenuFlyoutItem).Tag)
        {
            case "Fullscreen":
                var view = ApplicationView.GetForCurrentView();
                if (!view.IsFullScreenMode)
                {
                    WindowManager.EnterFullScreen(true);
                }
                else
                {
                    WindowManager.EnterFullScreen(false);
                }
                break;
            case "DevTools":
                if (TabContent.Content is WebViewPage)
                {
                    TabWebView.CoreWebView2.OpenDevToolsWindow();
                }
                else
                {
                    await UI.ShowDialog("Error", "Only webpage source can be inspected");
                }
                break;
            case "ShowSource":
                if (TabContent.Content is WebViewPage)
                {
                    launchurl = "view-source:" + TabWebView.Source.ToString();
                    CreateWebTab();
                }
                else
                {
                    await UI.ShowDialog("Error", "Only webpage source can be inspected");
                }
                break;
            case "Settings":
                CreateTab("Settings", Symbol.Setting, typeof(SettingsPage));
                break;
            case "About":
                break;
        }
    }

    #region UrlBox
    private void UrlBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            if (UrlBox.Text.Contains("."))
            {
                if (UrlBox.Text.Contains("http://") || UrlBox.Text.Contains("https://"))
                {
                    NavigateToUrl(UrlBox.Text.Trim());
                }
                else
                {
                    UrlBox.Text = "https://" + UrlBox.Text;
                    NavigateToUrl(UrlBox.Text.Trim());
                }
            }
            else
            {
                string searchurl;
                if (SearchUrl == null) searchurl = "https://lite.qwant.com/?q=";
                else
                {
                    searchurl = SearchUrl;
                }
                string query = searchurl + UrlBox.Text;
                NavigateToUrl(query);
            }
            SearchFlyout.Hide();
        }
    }

    private void UrlBox_GotFocus(object sender, RoutedEventArgs e)
    {
        UrlBox.SelectAll();
    }
    #endregion

    public void NavigateToUrl(string uri)
    {
        if (TabWebView != null)
        {
            TabWebView.CoreWebView2.Navigate(uri);
        }
        else {
            launchurl = uri;
            TabContent.Navigate(typeof(WebViewPage));
        }
    }

    public TabViewItem SelectedTab
    {
        get
        {
            TabViewItem selectedItem = (TabViewItem)Tabs.SelectedItem;
            if (selectedItem != null)
            {
                return selectedItem;
            }
            return null;
        }
    }

    Frame TabContent
    {
        get
        {
            TabViewItem selectedItem = (TabViewItem)Tabs.SelectedItem;
            if (selectedItem != null)
            {
                return (Frame)selectedItem.Content;
            }
            return null;
        }
    }

    WebView2 TabWebView
    {
        get
        {
            if (TabContent.Content is WebViewPage)
            {
                return (TabContent.Content as WebViewPage).WebViewControl;
            }
            return null;
        }
    }

    private void Tabs_Loaded(object sender, RoutedEventArgs e)
    {
        if (launchurl != null) CreateWebTab();
        else
        {
            CreateHomeTab();
        }
    }

    private void Tabs_AddTabButtonClick(TabView sender, object args)
    {
        CreateHomeTab();
    }

    public void CreateHomeTab()
    {
        CreateTab("New tab", Symbol.Document, typeof(NewTabPage));
    }

    public void CreateWebTab()
    {
        CreateTab("New tab", Symbol.Document, typeof(WebViewPage));
    }

    public void CreateTab(string header, Symbol symbol , Type page)
    {
        Frame frame = new();
        TabViewItem newItem = new()
        {
            Header = header,
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = symbol },
            Content = frame,
        };
        frame.Navigate(page);
        Tabs.TabItems.Add(newItem);
        Tabs.SelectedItem = newItem;
    }

    private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        TabViewItem selectedItem = args.Tab;
        var tabcontent = (Frame)selectedItem.Content;
        if (tabcontent.Content is WebViewPage) (tabcontent.Content as WebViewPage).WebViewControl.Close();
        sender.TabItems.Remove(args.Tab);
    }

    private async void LoadListFromJson(string file)
    {
        JsonItemsList = await Json.GetListFromJsonAsync(file);
        if (JsonItemsList != null) JsonListView.ItemsSource = JsonItemsList;
        else
        {
            JsonListView.ItemsSource = null;
        }
    }

    private void JsonListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Get listview sender
        ListView listView = sender as ListView;
        if (listView.ItemsSource != null)
        {
            // Get selected item
            JsonItems item = (JsonItems)listView.SelectedItem;
            string url = item.Url;
            NavigateToUrl(url);
            listView.ItemsSource = null;
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textbox = sender as TextBox;
        // Get all ListView items with the submitted search query
        var SearchResults = from s in JsonItemsList where s.Title.Contains(textbox.Text, StringComparison.OrdinalIgnoreCase) select s;
        // Set SearchResults as ItemSource for HistoryListView
        JsonListView.ItemsSource = SearchResults;
    }

    private async void ShowHistory()
    {
        var historyList = await DataAccess.GetHistoryDetails();
        if (historyList != null) SmallHistoryMenu.ItemsSource = historyList;
        else
        {
            SmallHistoryMenu.ItemsSource = null;
        }
    }

    private async void SmallHistoryMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListView listView = sender as ListView;
        if (listView.ItemsSource != null)
        {
            // Get selected item
            HistoryDetails item = (HistoryDetails)listView.SelectedItem;
            string url = item.Url;
            NavigateToUrl(url);
            listView.ItemsSource = null;
        }
    }

    private async void SmallHistoryMenu_SearchBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        // very inefficient
        var historyList = await GetHistoryDetails();
        TextBox textbox = sender as TextBox;
        // Get all ListView items with the submitted search query
        var SearchResults = from s in historyList where s.Title.Contains(textbox.Text, StringComparison.OrdinalIgnoreCase) select s;
        // Set SearchResults as ItemSource for HistoryListView
        SmallHistoryMenu.ItemsSource = SearchResults;
    }
}