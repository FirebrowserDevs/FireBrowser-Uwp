﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Storage;
using System.Collections.Generic;
using FireBrowser.Pages;

namespace FireBrowser.Core;

public class Globals
{
    // History items list
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

    // Global variable to pass random data to other pages
    public static string launchurl { get; set; }
}