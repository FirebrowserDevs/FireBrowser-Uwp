using FireBrowser.SetupForm;
using FireTabs;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace FireBrowser
{
    public partial class BrowserWindow : Form
    {
        public BrowserWindow()
        {
            InitializeComponent();       
        }

        protected FireTitle ParentTabs
        {
            get
            {
                return (ParentForm as FireTitle);
            }
        }

        string url = FireBrowser.Properties.Settings.Default.EngineDefault.ToString();
        public async void Browser_Load(object sender, EventArgs e)
        {
            webHost.CoreWebView2.Navigate(url);
            initweb();
           
            if (FireBrowser.Properties.Settings.Default.SetupDone == true)
            {
                
            }
            else
            {

               
            }
            
        }

        public bool incognito = FireBrowser.Properties.Settings.Default.incognito;
        public string userProfile = FireBrowser.Properties.Settings.Default.userProfile;
        public async void initweb()
        {
            try
            {
                await webHost.EnsureCoreWebView2Async(null);
                webHost.Source = new Uri("https://www.google.nl");
            }
            catch
            {

            }
        }

        private void webHost_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
           
        }

        private void Browser_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            
        }

        private void Browser_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            
        }

        private void Browser_changedDoc(object sender, object e)
        {
            
        }
    }
}
