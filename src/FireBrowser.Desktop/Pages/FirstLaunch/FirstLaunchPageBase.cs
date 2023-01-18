
namespace FireBrowser.Pages.FirstLaunch
{
    public class FirstLaunchPageBase
    {
        public string PageTitleResource { get; set; }
        public string PageID { get; set; }
        public Type PageContent { get; set; }
        public bool CanContinue { get; set; }
        public Type PageSkip { get; set; }
        //Path to the Lottie animation displayed on the left side
        public string AnimationPath { get; set; }
    }
}