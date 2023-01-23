using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowser.Core;

public class ReadingModeHelper
{
    public static async Task<string> GetReadingModeJScriptAsync()
    {
        StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Extensions/ReadingMode/simplyread-firebrowser-min.js"));
        string jscript = await FileIO.ReadTextAsync(file);
        return jscript;
    }
}
