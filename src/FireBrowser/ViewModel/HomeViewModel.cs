using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowser.Core;

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
