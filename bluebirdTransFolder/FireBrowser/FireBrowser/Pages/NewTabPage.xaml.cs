
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FireBrowser.Pages;

public sealed partial class NewTabPage : Page
{
    public NewTabPage()
    {
        this.InitializeComponent();
    }

    private void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        var textbox = sender as TextBox;
        textbox.Focus(FocusState.Programmatic);
    }

    private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        var textbox = sender as TextBox;
        if (e.Key == VirtualKey.Enter)
        {
            string searchurl;
            if (SearchUrl == null) searchurl = "https://lite.qwant.com/?q=";
            else
            {
                searchurl = SearchUrl;
            }
            string query = searchurl + textbox.Text;
            MainPageContent.NavigateToUrl(query);
        }
    }
}
