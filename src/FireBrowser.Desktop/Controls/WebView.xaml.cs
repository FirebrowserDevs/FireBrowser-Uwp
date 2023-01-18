using FireBrowser.Controls.BrowserEngines;

namespace FireBrowser.Controls
{
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
