using System;

namespace Bluebird.Shared
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
