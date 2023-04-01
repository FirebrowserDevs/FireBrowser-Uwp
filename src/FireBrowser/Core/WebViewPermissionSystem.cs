using FireBrowser;
using FireBrowser.Pages;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public class WebViewPermissionSystem
{
    private readonly string _permissionsFileName = "permissions.json";
    private readonly string _permissionsFilePath;

    private Dictionary<string, Dictionary<CoreWebView2PermissionKind, CoreWebView2PermissionState>> _permissions;

    public WebViewPermissionSystem()
    {
        _permissionsFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, _permissionsFileName);
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
        // Show a dialog box asking the user for permission
        var dialog = new ContentDialog
        {
            Title = $"Allow {website} to access {permissionKind}?",
            PrimaryButtonText = "Allow",
            SecondaryButtonText = "Deny",
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = "Cancel"
        };

        var dialogResult = await dialog.ShowAsync();

        if (dialogResult == ContentDialogResult.Primary)
        {
            return CoreWebView2PermissionState.Allow;
        }
        else
        {
            return CoreWebView2PermissionState.Deny;
        }
    }
}

