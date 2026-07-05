using Microsoft.JSInterop;
using VRT.Resume.Pwa.Layout;

namespace VRT.Resume.Pwa.Services;

public sealed class PwaThemeService(IJSRuntime js)
{
    public const string StorageKey = "VRT.Resume.DarkMode";

    private bool _isDarkMode;

    public event Action? ThemeChanged;

    public bool IsDarkMode => _isDarkMode;

    public async Task InitializeFromStorageAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var stored = await js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
            var enabled = string.Equals(stored, "true", StringComparison.Ordinal);
            if (_isDarkMode != enabled)
            {
                _isDarkMode = enabled;
                ThemeChanged?.Invoke();
            }
        }
        catch (JSException)
        {
            // Keep default when storage is unavailable (e.g. test host).
        }

        await SyncBrowserChromeAsync();
    }

    public void SetDarkMode(bool enabled)
    {
        if (_isDarkMode == enabled)
            return;

        _isDarkMode = enabled;
        _ = PersistAsync(enabled);
        ThemeChanged?.Invoke();
        _ = SyncBrowserChromeAsync();
    }

    public void ToggleDarkMode() => SetDarkMode(!_isDarkMode);

    private async Task PersistAsync(bool enabled)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, enabled ? "true" : "false");
        }
        catch (JSException)
        {
            // Preference already applied in memory.
        }
    }

    private async Task SyncBrowserChromeAsync()
    {
        try
        {
            await js.InvokeVoidAsync(
                "__pwaSetThemeColor",
                _isDarkMode ? AppMudTheme.DarkChromeHex : AppMudTheme.LightChromeHex);
        }
        catch (JSException)
        {
            // Optional polish when boot script is unavailable.
        }
    }
}