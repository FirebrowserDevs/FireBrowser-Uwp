using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class PDFReader : Page
    {
        public class PageListVisibility : ObservableObject
        {
            private Visibility _state;

            public PageListVisibility()
            {
                this.PageVisibility = Visibility.Collapsed;
            }

            public Visibility PageVisibility
            {
                get { return _state; }
                set { SetProperty(ref _state, value); }
            }
        }


        public PDFReader()
        {
            this.InitializeComponent();
            FloatingToolbar.Translation += new Vector3(0, 0, 128);
            pdfViewer.IsThumbnailViewEnabled = false;
            pdfViewer.TextSelectionMenu.CopyButton.Visibility = Visibility.Collapsed;
            ViewModel = new PageListVisibility();
        }
        public PageListVisibility ViewModel { get; set; }

        async private void Open_Click(object sender, RoutedEventArgs e)
        {
            //Opens a file picker.
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;
            //Filters PDF files in the documents library.
            picker.FileTypeFilter.Add(".pdf");
            var file = await picker.PickSingleFileAsync();
            if (file == null) return;
            //Reads the stream of the loaded PDF document.
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            Stream fileStream = stream.AsStreamForRead();
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            //Loads the PDF document.
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(buffer);
            pdfViewer.LoadDocument(loadedDocument);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            pdfViewer.SearchText((sender as TextBox).Text);
        }
        public static string CurrentPage
        {
            get; set;
        }
        private void PdfViewer_PageChanged(object sender, PageChangedEventArgs e)
        {
            CurrentPage = "Page" + e.NewPageNumber.ToString() + "out of" + pdfViewer.PageCount.ToString();
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var passer = (e.Parameter as Passer);
            var parameter = passer?.Param;

            if (parameter is IStorageItem args)
            {
                //Loads the stream from the embedded resource.
                CancellationTokenSource cancellationTokenSource = new();
                await pdfViewer.LoadDocumentAsync(args as StorageFile, cancellationTokenSource.Token);
                passer.ViewModel.CurrentAddress = args.Path;
            }
            else if (parameter is Uri)
            {
                HttpClient httpClient = new();
                Byte[] contentBytes = await httpClient.GetByteArrayAsync(e.Parameter.ToString());
                PdfLoadedDocument loadedDocument = new(contentBytes);
                pdfViewer.LoadDocument(loadedDocument);
            }
            else
            {
                Dots.SDK.UWP.Log.WriteLog(parameter.GetType().ToString(), "PDFReaderOnNavigatedTo", Dots.SDK.Log.LogType.Info);
            }
        }
        private void PDFViewerListToggle_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as ToggleButton).IsChecked) { ViewModel.PageVisibility = Visibility.Visible; }
            else { ViewModel.PageVisibility = Visibility.Collapsed; }
        }
    }
}