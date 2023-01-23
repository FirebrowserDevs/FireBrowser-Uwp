using FireBrowser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages.SettingPages;

public sealed partial class Privacy : Page
{
    public Privacy()
    {
        this.InitializeComponent();
        UpdateText();
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
            trueCount++;
        }
        else
        {
            trueCount--;
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

    int trueCount = 0;
    public async void UpdateText()
    {
        if(trueCount == 0)
        {
            TextLevel.Text = "Default";
        }
        if(trueCount == 1)
        {
            TextLevel.Text = "Low";
        }
        if(trueCount == 2)
        {
            TextLevel.Text = "Medium";
        }
        if(trueCount > 3)
        {
            TextLevel.Text = "High";
        }
    }
    private void DisableGenaralAutoFillToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (DisableGenaralAutoFillToggle.IsOn)
        {
            SettingsHelper.SetSetting("GenAutoFill", "true");
            trueCount++;
        }
        else
        {
            SettingsHelper.SetSetting("GenAutoFill", "false");
            trueCount--;
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
            trueCount++;
        }
        else
        {
            SettingsHelper.SetSetting("WebMess", "false");
            trueCount--;
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
            trueCount++;
            SettingsHelper.SetSetting("PassSave", "true");
        }
        else
        {
            trueCount--;
            SettingsHelper.SetSetting("PassSave", "false");
        }
        UpdateText();
    }

}
