﻿using System;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;

namespace FireBrowserInterop
{
    public static class SystemHelper
    {
        public static async void RestartApp()
        {
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync("restart");

            if (result == AppRestartFailureReason.NotInForeground
                || result == AppRestartFailureReason.Other)
            {
               
            }
        }

        public static string GetSystemArchitecture()
        {
            string architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return architecture;
        }

        public static void ShowShareUIURL(string title, string url)
        {
            var dt = DataTransferManager.GetForCurrentView();
            dt.DataRequested += (sender, args) =>
            {
                DataRequest request = args.Request;
                request.Data.SetWebLink(new Uri(url));
                request.Data.Properties.Title = title;
            };
            DataTransferManager.ShowShareUI();
        }
    }
}
