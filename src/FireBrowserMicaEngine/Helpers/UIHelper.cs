using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace FireBrowserMicaEngine.Helpers
{
    internal static class UIHelper
    {
        public static bool HasTitleBar => !CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
    }
}
