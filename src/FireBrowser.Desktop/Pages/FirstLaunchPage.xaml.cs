using FireBrowser.Core;
using FireBrowser.Pages.FirstLaunch;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    
    //To-Do: OnNavigatedTo with int as parameter for the page that's supposed to be shown

    public sealed partial class FirstLaunchPage : Page
    {
        public FirstLaunchPage()
        {
            this.InitializeComponent();
            var examplePage = ExamplePage.PageData;
            FirstLaunchPages.Add(examplePage);
            FirstLaunchPages.Add(DataSwitchPage.PageData);
            ViewModel = new AppFirstLaunchViewModel()
            {
                Title = "Welcome",
                MainButtonText = "Continue",
                LeftSideVisibility = Visibility.Collapsed,
                NavigationButtonsAlignment = HorizontalAlignment.Stretch,
                NavigationButtonsOrientation = Orientation.Vertical
            };
        }
        public AppFirstLaunchViewModel ViewModel { get; set; }
        // C# - Scale can be applied to any UIElement. In this case it is an image called ToolkitLogo.

        public partial class AppFirstLaunchViewModel : ObservableObject
        {
            [ObservableProperty]
            private string title;
            [ObservableProperty]
            private string description;
            [ObservableProperty]
            private bool canContinue;
            [ObservableProperty]
            private string mainButtonText;
            [ObservableProperty]
            private Visibility leftSideVisibility;
            [ObservableProperty]
            private string lottiePath;
            [ObservableProperty]
            private HorizontalAlignment navigationButtonsAlignment;
            [ObservableProperty]
            private Orientation navigationButtonsOrientation;
        }
        List<FirstLaunchPageBase> FirstLaunchPages = new();
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("DirectConnectedAnimation");
            if (anim != null)
            {
                anim.TryStart(TemporaryPageName);
            }

            /*User UserData = (User)e.Parameter;
            //to-do: we only ask for this permission once in the customize page
            var name = await UserData.GetPropertyAsync(KnownUserProperties.FirstName);
            if (name.ToString().Length != 0)
                AppFirstLaunchWelcomeText.Text = String.Format(Core.Resources.resourceLoader.GetString("FirstLaunchWelcomeText"), name);
            else
                AppFirstLaunchWelcomeText.Text = Core.Resources.resourceLoader.GetString("FirstLaunchWelcomeTextFallback");*/
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            NavigatePage(NavigateType.Forward);
        }
        enum NavigateType
        {
            Load,
            Backward,
            Forward,
            Skip,
        }
        private async void NavigatePage(NavigateType type)
        {
            var settings = await Core.Settings.GetSettingsDataAsync();

            var currentPageIndex = 0;
            var currentPage = FirstLaunchPages[currentPageIndex];
            //var correctIndex = currentPageIndex += 1;
            if (currentPageIndex > 0)
            { 
                ViewModel.LeftSideVisibility = Visibility.Visible;
                ViewModel.NavigationButtonsAlignment = HorizontalAlignment.Right;
                ViewModel.NavigationButtonsOrientation = Orientation.Horizontal;
            }
            //to-do: use a switch
            if(type == NavigateType.Skip)
            {
                //Temporary weird behavior
                settings.AccountData.FinishedFirstLaunch = true;
                AppData.SaveData(settings, AppData.DataType.AppSettings);
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                AppLaunchPasser passer = new()
                {
                    LaunchType = AppLaunchType.FirstLaunch
                };
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", TemporaryPageName);
                Frame.Navigate(typeof(MainPage), passer, new SuppressNavigationTransitionInfo());

                if (currentPageIndex < FirstLaunchPages.Count && currentPage.CanContinue)
                {
                    currentPageIndex++;
                    ContentFrame.Navigate(currentPage.PageContent);
                }
                else if (currentPageIndex == FirstLaunchPages.Count)
                {
                    ViewModel.MainButtonText = "Finish";
                    currentPageIndex++;
                    ContentFrame.Navigate(currentPage.PageContent);
                }
            }
            //Dots.SDK.UWP.Log.WriteLog(currentPageIndex.ToString(), "Current index");
            //Dots.SDK.UWP.Log.WriteLog(correctIndex.ToString(), "Correct index amount");
            //Dots.SDK.UWP.Log.WriteLog(FirstLaunchPages.Count.ToString(), "Amount of pages");
        }
        private async void FirstLaunchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationBuilder.Create()
                .Opacity(from: 0, to: 1, duration: TimeSpan.FromSeconds(2))
                .Scale(from: 0.4, to: 1, duration: TimeSpan.FromSeconds(2), layer: FrameworkLayer.Composition).Start(FirstLaunchWindow);
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            NavigatePage(NavigateType.Skip);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
