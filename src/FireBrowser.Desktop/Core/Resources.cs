using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace FireBrowser.Core
{
    public class Resources
    {
        public static ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        public static string collectionPath = $"{ApplicationData.Current.LocalFolder.Path}\\FireBrowserData\\{AppData.CurrentProfileCore.FriendlyID}\\Collections";
        public static float GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return float.Parse(string.Format("{0}.{1}", version.Major, version.Minor));
        }

        public static string GetTextResource(string resourceName)
        {
            try { return resourceLoader.GetString(resourceName); }
            catch { return resourceName; }
        }
    }
}
