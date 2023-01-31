using Microsoft.Toolkit.Uwp.UI;
using System;

namespace FireBrowser.Core
{
    public class Icons
    {
        public class FluentIcon : FontIconExtension
        {
            FluentIcon()
            {
                FontFamily = new Windows.UI.Xaml.Media.FontFamily("FluentIcons");
            }
        }
    }
}
