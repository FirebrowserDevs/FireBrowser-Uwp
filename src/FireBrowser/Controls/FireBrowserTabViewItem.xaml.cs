using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace FireBrowser.Controls;
public partial class FireBrowserTabViewItem : TabViewItem
{
    public FireBrowserTabViewItem() => InitializeComponent();

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static DependencyProperty ValueProperty = DependencyProperty.Register(
    nameof(Value),
    typeof(string),
    typeof(FireBrowserTabViewItem),
    null);
}
