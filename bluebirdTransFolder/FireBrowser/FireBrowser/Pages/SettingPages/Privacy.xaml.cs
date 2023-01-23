using FireBrowser.Core;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages.SettingPages;

public sealed partial class Privacy : Page
{
    public Privacy()
    {
        this.InitializeComponent();
    }

    private void DisableJavaScriptToggle_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("DisableJavaScript");
        if (selection == "true")
        {
            DisableJavaScriptToggle.IsOn = true;
        }
        
    }

    private void DisableJavaScriptToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (DisableJavaScriptToggle.IsOn)
        {
            SettingsHelper.SetSetting("DisableJavaScript", "true");
        }
        else
        {
            SettingsHelper.SetSetting("DisableJavaScript", "false");
        }
        UpdateText();
    }

    private void DisableGenaralAutoFillToggle_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("GenAutoFill");
        if (selection == "true")
        {
            DisableGenaralAutoFillToggle.IsOn = true;
        }
    }

    public void UpdateText()
    {
        #region lowlevel
        if (DisableJavaScriptToggle.IsOn == true)
        {
            TextLevel.Text = "Low";
        }
        else if(DisableJavaScriptToggle.IsOn == false) 
        {
            TextLevel.Text = "Default";
        }
        if(DisablWebMessFillToggle.IsOn == true)
        {
            TextLevel.Text = "Low";
        }
        else if(DisablWebMessFillToggle.IsOn == false)
        {
            TextLevel.Text = "Default";
        }
        if (DisableGenaralAutoFillToggle.IsOn == true)
        {
            TextLevel.Text = "Low";
        }
        else if (DisableGenaralAutoFillToggle.IsOn == false)
        {
            TextLevel.Text = "Default";
        }
        if (PasswordWebMessFillToggle.IsOn == true)
        {
            TextLevel.Text = "Low";
        }
        else if (PasswordWebMessFillToggle.IsOn == false)
        {
            TextLevel.Text = "Default";
        }
        #endregion
    }
    private void DisableGenaralAutoFillToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (DisableGenaralAutoFillToggle.IsOn)
        {
            SettingsHelper.SetSetting("GenAutoFill", "true");
        }
        else
        {
            SettingsHelper.SetSetting("GenAutoFill", "false");
        }
        UpdateText();
    }

    private void DisablWebMessFillToggle_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("WebMess");
        if (selection == "true")
        {
            DisablWebMessFillToggle.IsOn = true;
        }
    }

    private void DisablWebMessFillToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (DisablWebMessFillToggle.IsOn)
        {
            SettingsHelper.SetSetting("WebMess", "true");
        }
        else
        {
            SettingsHelper.SetSetting("WebMess", "false");
        }
        UpdateText();
    }

    private void PasswordWebMessFillToggle_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("PassSave");
        if (selection == "true")
        {
            PasswordWebMessFillToggle.IsOn = true;
        }
    }

    private void PasswordWebMessFillToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (PasswordWebMessFillToggle.IsOn)
        {
            SettingsHelper.SetSetting("PassSave", "true");
        }
        else
        {
            SettingsHelper.SetSetting("PassSave", "false");
        }
        UpdateText();
    }

}
