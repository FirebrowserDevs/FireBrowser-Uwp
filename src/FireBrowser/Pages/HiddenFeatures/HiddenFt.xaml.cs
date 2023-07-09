using ColorCode.Compilation.Languages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using muxc = Microsoft.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.HiddenFeatures
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HiddenFt : Page
    {
        public HiddenFt()
        {
            this.InitializeComponent();
            Stack();
        }

        public async void Stack()
        {
            HiddenTb1.IsOn = test1 == "0x1";
        }

        string test1 = FireBrowserInterop.SettingsHelper.GetSetting("DragOutSideExperiment");

        private Passer passer;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as Passer;
            passer.Tab.Header = "FireBrowser Experimental";
            passer.Tab.IconSource = new muxc.FontIconSource()
            {
                Glyph = "\uEC7A"
            };
        }

        private bool restartConfirmationShown = true;

        private void HiddenTb1_Toggled(object sender, RoutedEventArgs e)
        {
           if(HiddenTb1.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DragOutSideExperiment", "0x1");
            }
            else
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DragOutSideExperiment", "0x0");
            }
        }
    }
}
