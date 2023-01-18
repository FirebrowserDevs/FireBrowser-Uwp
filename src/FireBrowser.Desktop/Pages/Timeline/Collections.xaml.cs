using FireBrowser.Core;
using Dots.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage;
using Dots.SDK.Collections;
using Dots.SDK.CollectionItems;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Timeline
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Collections : Page
    {
        public Collections()
        {
            this.InitializeComponent();
            ViewModel.CollectionPath = Core.Resources.collectionPath;

            foreach (var item in CollectionHelpers.GetCollectionsList(Core.Resources.collectionPath).Collections)
            { ViewModel.Collections.Add(item); }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await CreateCollectionDialog.ShowWithAnimationAsync();
        }

        private void CreateCollectionDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Collection collection = new()
            {
                Name = NewListName.Text,
                Content = new List<ICollectionItem>()
            };
            BaseCollection baseCollection = new()
            {
                Name = NewListName.Text
            };
            collection = collection.WriteCollection(ViewModel.CollectionPath);
            baseCollection.ID = collection.ID;
            ViewModel.Collections.Add(baseCollection);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            ICollectionItem item = null;
            switch (btn.Tag)
            {
                case "Website":
                    item = new WebsiteItem()
                    {
                        Title = "Hello World",
                        URL = new Uri("https://example.com")
                    };
                    break;
                case "Text":
                    item = new TextItem()
                    {
                        Text = "hello"
                    };
                    break;
            }
            ViewModel.CollectionItems.Add(item);
            var col = ViewModel.SelectedCollection;
            Collection collection = new()
            {
                Name = col.Name,
                ID = col.ID,
                Content = ViewModel.CollectionItems.ToList()
            };
            collection.WriteCollection(ViewModel.CollectionPath);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var col = ViewModel.SelectedCollection;
            Collection collection = new()
            {
                Name = col.Name,
                ID = col.ID,
                Content = ViewModel.CollectionItems.ToList()
            };
            collection.WriteCollection(ViewModel.CollectionPath);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var collection = new Collection()
            {
                Name = ViewModel.SelectedCollection.Name,
                ID = ViewModel.SelectedCollection.ID
            };
            collection.DeleteCollection(ViewModel.CollectionPath);
            ViewModel.Collections.Remove(ViewModel.SelectedCollection);
        }
    }
}
