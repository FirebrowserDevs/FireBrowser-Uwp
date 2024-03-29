﻿using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace FireBrowser.Launch;

public sealed partial class Setup : Page
{
    public Setup()
    {
        this.InitializeComponent();

        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.ExtendViewIntoTitleBar = true;
        coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

        var formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
        formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
        formattableTitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
        formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        formattableTitleBar.InactiveBackgroundColor = Colors.Transparent;
        formattableTitleBar.ButtonPressedBackgroundColor = Colors.Transparent;

        Window.Current.SetTitleBar(TitleBar);

        DataContext = this;
    }

 

    private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
        TitleBar.MinWidth = (FlowDirection == FlowDirection.LeftToRight) ? sender.SystemOverlayRightInset : sender.SystemOverlayLeftInset;
        TitleBar.Height = sender.Height;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Frame.Visibility = Visibility.Visible;
        Frame.Navigate(typeof(SetupSettings), null, new DrillInNavigationTransitionInfo());
    }

    private string _introMessage = @"
• Seamless browsing experience.

• One-click access to favorite websites and a built-in favorites organizer.

• Immersive full-screen mode.

• Prioritizes user convenience.

• Caters to users seeking a user-friendly web browser with advanced features.
";
}