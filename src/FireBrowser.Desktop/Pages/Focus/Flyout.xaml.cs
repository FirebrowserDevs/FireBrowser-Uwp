// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Focus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Flyout : Page
    {
        public Flyout()
        {
            this.InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Core.Focusing.selectedList = (sender as ListView).SelectedIndex;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Core.Focusing.isFocusing = true;
        }
    }
}
