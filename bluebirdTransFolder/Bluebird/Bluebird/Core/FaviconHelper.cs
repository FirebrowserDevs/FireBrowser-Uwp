using Microsoft.UI.Xaml.Controls;
using System;

namespace Bluebird.Core;

public static class FaviconHelper
{
    public static IconSource GetFavicon(string url)
    {
        Uri faviconUrl = new("https://www.google.com/s2/favicons?domain=" + url);
        BitmapIconSource iconsource = new() { UriSource = faviconUrl, ShowAsMonochrome = false };
        return iconsource;
    }
}