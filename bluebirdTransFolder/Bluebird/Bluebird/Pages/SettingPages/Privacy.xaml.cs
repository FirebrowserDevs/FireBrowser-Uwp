﻿using Bluebird.Core;
using Windows.UI.Xaml.Controls;

namespace Bluebird.Pages.SettingPages;

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
        string selection = SettingsHelper.GetSetting("DisableGenAutoFill");
        if (selection == "true")
        {
            DisableGenaralAutoFillToggle.IsOn = true;
        }
    }

    int trueCount = 0;
    public async void UpdateText()
    {
        TextLevel.Text = trueCount switch
        {
            0 => "Default",
            1 => "Low",
            2 => "Medium",
            3 => "High",
            4 => "Extreme"
        };
    }

    private void DisableGenaralAutoFillToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (DisableGenaralAutoFillToggle.IsOn)
        {
            SettingsHelper.SetSetting("DisableGenAutoFill", "true");
            trueCount++;
        }
        else
        {
            SettingsHelper.SetSetting("DisableGenAutoFill", "false");
            trueCount--;
        }
        UpdateText();
    }

    private void DisablWebMessFillToggle_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("DisableWebMess");
        if (selection == "true")
        {
            DisablWebMessFillToggle.IsOn = true;
        }
    }

    private void DisablWebMessFillToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (DisablWebMessFillToggle.IsOn)
        {
            SettingsHelper.SetSetting("DisableWebMess", "true");
            trueCount++;
        }
        else
        {
            SettingsHelper.SetSetting("DisableWebMess", "false");
            trueCount--;
        }
        UpdateText();
    }

    private void PasswordWebMessFillToggle_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("DisablePassSave");
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
            SettingsHelper.SetSetting("DisablePassSave", "true");
        }
        else
        {
            trueCount--;
            SettingsHelper.SetSetting("DisablePassSave", "false");
        }
        UpdateText();
    }

}
