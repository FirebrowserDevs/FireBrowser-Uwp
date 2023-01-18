using Microsoft.UI.Xaml.Controls;
using System;

namespace FireBrowser.Core;

public static class FaviconHelper
{
    public static IconSource GetFavicon(string url)
    {
        Uri faviconUrl = new Uri("https://www.google.com/s2/favicons?domain=" + url);
        IconSource iconsource = new BitmapIconSource() { UriSource = faviconUrl, ShowAsMonochrome = false };
        return iconsource;
    }
}