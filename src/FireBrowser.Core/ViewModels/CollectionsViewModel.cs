using CommunityToolkit.Mvvm.ComponentModel;
using Dots.SDK.CollectionItems;
using Dots.SDK.Collections;
using Dots.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Core.ViewModels
{
    public partial class CollectionsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string collectionPath;
        [ObservableProperty]
        private ObservableCollection<BaseCollection> collections = new();
        [ObservableProperty]
        private ObservableCollection<ICollectionItem> collectionItems = new();
        [ObservableProperty]
        private BaseCollection selectedCollection;

       

        [ObservableProperty]
        private ICollectionItem selectedItem;

    }
}
