using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Core
{
    public static class UI
    {
        public static IAsyncOperation<ContentDialogResult> ShowWithAnimationAsync(this ContentDialog contentDialog)
        {
            if (contentDialog.Style == null)
            {
                contentDialog.Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"];
            }

            return contentDialog.ShowAsync();
        }

        public async static Task ShowDialog(string title, string message)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "OK"
            };
            await dialog.ShowWithAnimationAsync();
        }
    }
}
