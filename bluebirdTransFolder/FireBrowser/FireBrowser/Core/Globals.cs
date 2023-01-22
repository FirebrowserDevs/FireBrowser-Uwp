﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Storage;
using System.Collections.Generic;
using FireBrowser.Pages;
using static FireBrowser.Core.DataAccess;

namespace FireBrowser.Core;

public class Globals
{
    // History items list
    public static List<HistoryDetails> HistoryList { get; set; }
    public class JsonItems
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    // Access public elements/methods from MainPage
    public static MainPage MainPageContent
    {
        get { return (Window.Current.Content as Frame)?.Content as MainPage; }
    }
    public static WebViewPage WebViewPageContent
    {
        get { return (Window.Current.Content as Frame)?.Content as WebViewPage; }
    }
    // Global variables
    public static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

    public static string HomepageUrl { get; set; }
    public static string SearchUrl { get; set; }

    public static List<JsonItems> JsonItemsList;

    // Varible which defines which url should be launched when a new tab is created
    public static string launchurl { get; set; }
}
