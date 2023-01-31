using FireBrowser.Controls.BrowserEngines;
namespace FireBrowser.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebView : Edge
    {
        public WebView()
        {
            this.InitializeComponent();
            InitializeWebView2();
        }

        async void InitializeWebView2()
        {
            await EnsureCoreWebView2Async();
            //WebView2.CoreWebView2CompositionController.SendMouseInput()
        }
    }
}
