using FireBrowser.Pages.SettingsPages.Dialog;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

public class WebPermissionSystem
{
    private readonly string _permissionsFileName = "permissions.json";
    private readonly string _permissionsFolderPath;
    private readonly string _permissionsFilePath;

    private Dictionary<string, Dictionary<CoreWebView2PermissionKind, CoreWebView2PermissionState>> _permissions;

    public WebPermissionSystem()
    {
        _permissionsFolderPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "WebPermission");
        if (!Directory.Exists(_permissionsFolderPath))
        {
            Directory.CreateDirectory(_permissionsFolderPath);
        }
        _permissionsFilePath = Path.Combine(_permissionsFolderPath, _permissionsFileName);
        LoadPermissions();
    }

    private void LoadPermissions()
    {
        try
        {
            if (File.Exists(_permissionsFilePath))
            {
                var json = File.ReadAllText(_permissionsFilePath);
                _permissions = JsonSerializer.Deserialize<Dictionary<string, Dictionary<CoreWebView2PermissionKind, CoreWebView2PermissionState>>>(json);
            }
            else
            {
                _permissions = new Dictionary<string, Dictionary<CoreWebView2PermissionKind, CoreWebView2PermissionState>>();
            }
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public CoreWebView2PermissionState CheckPermission(string website, CoreWebView2PermissionKind permissionKind)
    {
        if (_permissions.ContainsKey(website) && _permissions[website].ContainsKey(permissionKind))
        {
            return _permissions[website][permissionKind];
        }
        else
        {
            return CoreWebView2PermissionState.Default;
        }
    }




    public void SetPermission(string website, CoreWebView2PermissionKind permissionKind, CoreWebView2PermissionState value)
    {
        if (!_permissions.ContainsKey(website))
        {
            _permissions[website] = new Dictionary<CoreWebView2PermissionKind, CoreWebView2PermissionState>();
        }

        _permissions[website][permissionKind] = value;
        SavePermissions();
    }

    private void SavePermissions()
    {
        try
        {
            var json = JsonSerializer.Serialize(_permissions);
            File.WriteAllText(_permissionsFilePath, json);
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    public async Task HandlePermissionRequested(CoreWebView2PermissionRequestedEventArgs args, string url)
    {

        var website = GetWebsiteFromUrl(new Uri(url));

        if (string.IsNullOrEmpty(website))
        {
            args.State = CoreWebView2PermissionState.Default;
            return;
        }

        var permissionKind = args.PermissionKind;
        var permissionState = CheckPermission(website, permissionKind);


        if (permissionState == CoreWebView2PermissionState.Allow)
        {
            args.State = CoreWebView2PermissionState.Allow;
        }
        else if (permissionState == CoreWebView2PermissionState.Deny)
        {
            args.State = CoreWebView2PermissionState.Deny;
        }
        else // Default
        {
            // Prompt the user for permission
            var result = await PromptUserForPermission(permissionKind, website);

            if (result == CoreWebView2PermissionState.Allow)
            {
                args.State = CoreWebView2PermissionState.Allow;
            }
            else if (result == CoreWebView2PermissionState.Deny) // Deny or Default
            {
                args.State = CoreWebView2PermissionState.Deny;
            }
            else if (result == CoreWebView2PermissionState.Default)
            {
                args.State = CoreWebView2PermissionState.Default;
            }

            SetPermission(website, permissionKind, args.State);
        }
    }


    private string GetWebsiteFromUrl(Uri uri)
    {
        if (uri != null && uri.Host != null)
        {
            var host = uri.Host;

            if (host.StartsWith("www."))
            {
                host = host.Substring(4);
            }

            return host;
        }

        return null;
    }


    private async Task<CoreWebView2PermissionState> PromptUserForPermission(CoreWebView2PermissionKind permissionKind, string website)
    {
        var dialog = new PermissionWindow
        {
            Title = $"Allow {website} to access {permissionKind}?",
        };

        await dialog.ShowAsync();

        switch (dialog.Result)
        {
            case PermissionWindow.ProtectResult.Allow:
                return CoreWebView2PermissionState.Allow;
            case PermissionWindow.ProtectResult.Deny:
                return CoreWebView2PermissionState.Deny;
            default:
                return CoreWebView2PermissionState.Default;
        }

    }
}

