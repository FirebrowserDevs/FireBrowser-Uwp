using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowser.Core;

public class FileHelper
{
    public static async Task DeleteLocalFile(string fileName)
    {
        var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
        if (file != null)
        {
            await file.DeleteAsync();
        }
    }
}