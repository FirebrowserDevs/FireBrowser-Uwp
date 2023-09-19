namespace FireBrowserCore.Overlay;

public class AppOverlay
{
    #region appdata

    public class AppSettings
    {
        public bool IsFirstLaunch { get; set; }

        public bool IsConnected { get; set; }
    }

    public class AppLaunchPasser
    {
        public AppLaunchType LaunchType { get; set; }
        public object LaunchData { get; set; }

        public string UserName { get; set; }
    }

    public enum AppLaunchType
    {
        LaunchBasic,
        LaunchIncognito,
        LaunchStartup,
        FirstLaunch,
        FilePDF,
        URIHttp,
        URIFireBrowser,
        Reset,
    }

    #endregion
}