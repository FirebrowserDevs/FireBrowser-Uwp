using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace FireBrowserCore.Models
{
    public static class UI
    {
        public static async Task<ContentDialogResult> ShowWithAnimationAsync(this ContentDialog contentDialog)
        {
            contentDialog.Style ??= (Style)Application.Current.Resources["DefaultContentDialogStyle"];

            return await contentDialog.ShowAsync();
        }

        public static async Task ShowDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel"
            };

            await dialog.ShowWithAnimationAsync();
        }
    }
}
