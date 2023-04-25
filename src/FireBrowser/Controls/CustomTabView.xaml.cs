using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using FireBrowserCore.Models;

// Szablon elementu Kontrolka użytkownika jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234236

namespace FireBrowser.Controls
{
    public sealed partial class CustomTabView : TabView
    {
        public CustomTabView()
        {
            this.InitializeComponent();
            ViewModel = new CustomTabViewViewModel()
            {
                Style = (Style)Application.Current.Resources["DefaultTabViewStyle"]
            };
        }
        public CustomTabViewViewModel ViewModel { get; set; }
        public partial class CustomTabViewViewModel : ObservableObject
        {
            [ObservableProperty]
            private Style style;
        }
        public Settings.UILayout Mode
        {
            get => (Settings.UILayout)GetValue(ModeProperty);
            set
            {
                switch (value)
                {
                    case Settings.UILayout.Classic:
                        ViewModel.Style = (Style)Application.Current.Resources["DefaultTabViewStyle"];
                        break;
                    case Settings.UILayout.Compact:
                        ViewModel.Style = (Style)Application.Current.Resources["CompactTabViewStyle"];
                        break;
                    case Settings.UILayout.Vertical:
                        ViewModel.Style = (Style)Application.Current.Resources["VerticalTabViewStyle"];
                        break;
                    default:
                        ViewModel.Style = (Style)Application.Current.Resources["DefaultTabViewStyle"];
                        break;
                }
                SetValue(ModeProperty, value);
            }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(Settings.UILayout), typeof(CustomTabView), null);
    }
}
