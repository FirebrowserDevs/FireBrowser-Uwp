using static FireBrowser.Core.Globals;
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
using Windows.ApplicationModel.VoiceCommands;

namespace FireBrowser;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        CustomTitleBar();
        Window.Current.VisibilityChanged += WindowVisibilityChangedEventHandler;
    }

    void WindowVisibilityChangedEventHandler(System.Object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
    {
        // Perform operations that should take place when the application becomes visible rather than
        // when it is prelaunched, such as building a what's new feed
        if (Tabs.TabItems.Count == 0) CreateNewTab();
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
            case "Home":
                NavigateToUrl(HomepageUrl);
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
                LoadListFromJson("History.json");
                break;
        }
    }

    private async void MoreFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        switch ((sender as MenuFlyoutItem).Tag)
        {
            case "DownloadFlyout":
                TabWebView.CoreWebView2.OpenDefaultDownloadDialog();
                break;
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
                TabWebView.CoreWebView2.OpenDevToolsWindow();
                break;
            case "ShowSource":
                launchurl = "view-source:" + TabWebView.Source.ToString();
                CreateNewTab();
                break;
            case "TaskManager":
                TabWebView.CoreWebView2.OpenTaskManagerWindow();
                break;
            case "Settings":
                OpenExtendedSidebar("Settings");
                break;
            case "About":
                OpenExtendedSidebar("About");
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

    private void NavigateToUrl(string uri)
    {
        TabWebView.CoreWebView2.Navigate(uri);
    }

    private void OpenExtendedSidebar(object page)
    {
        ExtendedSidebar.Visibility = Visibility.Visible;
        contentFrame.Navigate(Type.GetType($"FireBrowser.Pages.{page}Page"));
        ExtendedSidebarTitle.Text = page.ToString();
    }

    private void CloseExtendedSidebar_Click(object sender, RoutedEventArgs e)
    {
        ExtendedSidebar.Visibility = Visibility.Collapsed;
        contentFrame.Content = null;
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
        CreateNewTab();
    }

    private void Tabs_AddTabButtonClick(TabView sender, object args)
    {
        CreateNewTab();
    }

    public void CreateNewTab()
    {
        Frame frame = new();
        TabViewItem newItem = new()
        {
            Header = "New tab",
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document },
            Content = frame,
        };
        frame.Navigate(typeof(WebViewPage));
        Tabs.TabItems.Add(newItem);
        Tabs.SelectedItem = newItem;
    }

    private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        TabViewItem selectedItem = args.Tab;
        var tabcontent = (Frame)selectedItem.Content;
        (tabcontent.Content as WebViewPage).WebViewControl.Close();
        sender.TabItems.Remove(args.Tab);
    }

    private async void LoadListFromJson(string file)
    {
        Core.Globals.JsonItemsList = await Json.GetListFromJsonAsync(file);
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
            MainPageContent.TabWebView.CoreWebView2.Navigate(url);
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
}