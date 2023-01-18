using Windows.UI;

namespace FireBrowser.Core
{
    public static class ColorConverter
    {
        public static Color GetColorFromHex(string hexaColor)
        {
            return
                Windows.UI.Color.FromArgb(
                    255,
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16));
        }
    }
}
