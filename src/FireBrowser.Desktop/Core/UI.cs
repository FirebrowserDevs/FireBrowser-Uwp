using System.Threading.Tasks;
using Windows.Foundation;

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
