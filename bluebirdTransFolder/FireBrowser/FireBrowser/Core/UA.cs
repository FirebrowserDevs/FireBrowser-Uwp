using Microsoft.Web.WebView2.Core;
using FireBrowser.Shared;

namespace FireBrowser.Core;

public class UA
{
    static string MozillaHeader = "Mozilla/5.0 ";
    // FireBrowser is a uwp app, wont run on <10, Win11 uses NT 10.0 UA as well, thus it does not matter
    // Need to add, ARM64 detection
    static string OS = "(Windows NT 10.0; Win64; x64) ";
    public static string wv2version = CoreWebView2Environment.GetAvailableBrowserVersionString() + " ";
    static string FireBrowserversion = "FireBrowser/" + AppVersion.GetAppVersion();
    public static string FireBrowserUA = MozillaHeader + OS + "Chrome/" + wv2version + FireBrowserversion;
}
