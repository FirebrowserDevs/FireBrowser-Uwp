using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowserUrlHelper
{
    public class UrlHelper
    {
        public static string GetInputType(string input)
        {
            string type;
            string tld = TLD.GetTLDfromURL(input);
            if (input.Contains("http://") || input.Contains("https://"))
            {
                type = "url";
            }
            else if (input.Contains(".") && TLD.KnownDomains.Any(tld.Contains))
            {
                type = "urlNOProtocol";
            }
            else
            {
                type = "searchquery";
            }
            return type;
        }
    }
}
