using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowser.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowser.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private Settings.NewTabBackground backgroundType;
        [ObservableProperty]
        private string imageTitle;
        [ObservableProperty]
        private string imageCopyright;
        [ObservableProperty]
        private string imageCopyrightLink;
    }
}
