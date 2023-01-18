using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowser.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowser.Core.ViewModels
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
        [ObservableProperty]
        private ObservableCollection<Dots.SDK.CollectionItems.WebsiteItem> pins;
        [ObservableProperty]
        private ObservableCollection<Dots.SDK.CollectionItems.WebsiteItem> searchSuggestions;
        [ObservableProperty]
        private bool showSearchSuggestions;
    }
}
