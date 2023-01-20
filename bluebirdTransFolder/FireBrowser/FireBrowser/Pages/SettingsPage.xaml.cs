using System;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
    }

    private void NavigationView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        NavView.SelectedItem = NavView.MenuItems[0];
        object pageTag = "General";
        NavigateToPage(pageTag);
    }

    private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
    {
        // Get invoked item
        object pageTag = args.InvokedItemContainer.Tag;
        // Navigate to selected page
        NavigateToPage(pageTag);
    }

    private void NavigateToPage(object page)
    {
        // Navigate to selected page
        contentFrame.Navigate(Type.GetType($"FireBrowser.Pages.SettingPages.{page}"));
        string pageString = page.ToString();
        NavView.Header = page;
    }
}
