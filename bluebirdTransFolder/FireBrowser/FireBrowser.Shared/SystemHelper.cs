using System;

namespace FireBrowser.Shared
{
    public class SystemHelper
    {

        public static string GetSystemArchitecture()
        {
            string architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return architecture;
        }
    }
}
