using System;
using System.ComponentModel;
using Windows.UI.ViewManagement;

namespace FireBrowser.Core
{
    public class FullSys
    {
        #region fullscreensys
        private bool fullScreen = false;

        [DefaultValue(false)]
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                ApplicationView view = ApplicationView.GetForCurrentView();
                if (value)
                {
                    try
                    {
                        if (!view.IsFullScreenMode)
                        {
                            view.TryEnterFullScreenMode();
                            Core.UseContent.MainPageContent.HideToolbar(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        FireExceptions.ExceptionsHelper.LogException(ex);
                    }
                }
                else
                {
                    try
                    {
                        if (view.IsFullScreenMode)
                        {
                            view.ExitFullScreenMode();
                            Core.UseContent.MainPageContent.HideToolbar(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        FireExceptions.ExceptionsHelper.LogException(ex);
                    }
                }
                fullScreen = value;
            }
        }

        #endregion
    }
}
