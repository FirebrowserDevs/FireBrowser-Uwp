using static FireBrowser.Core.Globals;
using FireBrowser.Shared;
using System;
using Windows.Storage;

namespace FireBrowser.Core;

public class FileHelper
{
    public static async void DeleteLocalFile(string filename)
    {
        try
        {
            StorageFile historyfile = await localFolder.GetFileAsync(filename);
            await historyfile.DeleteAsync();
        }
        catch (Exception ex)
        {
            ExceptionHelper.ThrowFullError(ex);
        }
    }
}