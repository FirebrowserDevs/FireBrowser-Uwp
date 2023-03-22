using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;

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
        public Core.Settings.UILayout Mode
        {
            get => (Core.Settings.UILayout)GetValue(ModeProperty);
            set
            {
                switch (value)
                {
                    case Core.Settings.UILayout.Classic:
                        ViewModel.Style = (Style)Application.Current.Resources["DefaultTabViewStyle"];
                        break;
                    case Core.Settings.UILayout.Compact:
                        ViewModel.Style = (Style)Application.Current.Resources["CompactTabViewStyle"];
                        break;
                    case Core.Settings.UILayout.Vertical:
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
            DependencyProperty.Register(nameof(Mode), typeof(Core.Settings.UILayout), typeof(CustomTabView), null);
    }
}
