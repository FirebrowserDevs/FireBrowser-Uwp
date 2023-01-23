using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Bluebird.Core
{
    internal class TLD
    {
        public static string[] KnownDomains { get; set; }

        public static async void LoadKnownDomains()
        {
            // Top level domain list
            StorageFolder appInstalledFolder = Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            var file = await assets.GetFileAsync("tldlist.json");
            string filecontent = await FileIO.ReadTextAsync(file);
            KnownDomains = JsonConvert.DeserializeObject<string[]>(filecontent);
        }

        public static string GetTLDfromURL(string url)
        {
            int pos = url.LastIndexOf(".") + 1;
            string tld = url.Substring(pos, url.Length - pos);
            return tld;
        }
    }
}
