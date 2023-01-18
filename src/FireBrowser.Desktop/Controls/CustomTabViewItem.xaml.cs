using FireBrowser.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI;
using System;


namespace FireBrowser.Controls
{
    public sealed partial class CustomTabViewItem : TabViewItem
    {
        public CustomTabViewItem()
        {
        }

        /// <summary>
        /// Only used in compact mode which has a special template with a textbox
        /// </summary>
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(CustomTabViewItem), null);
    }
}
