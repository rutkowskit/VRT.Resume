using System.Globalization;
using Microsoft.JSInterop;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Pwa.Services;

public sealed class PwaCultureService(IJSRuntime js) : ICultureService
{
    public const string StorageKey = "VRT.Resume.Culture";

    private const string DefaultLanguage = "pl";

    private static readonly Dictionary<string, (string key, string caption)> SupportedLanguages =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["pl"] = ("pl", "Polski"),
            ["en"] = ("en", "English"),
        };

    private string _currentCulture = DefaultLanguage;

    public event Action? CultureChanged;

    /// <summary>Apply the in-memory culture to thread defaults (safe before JS interop).</summary>
    public void ApplyCurrentCulture() => ApplyCulture(_currentCulture);

    /// <summary>Read persisted culture once Blazor JS interop is available.</summary>
    public async Task InitializeFromStorageAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var stored = await js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
            var normalized = NormalizeCulture(stored);
            if (!string.Equals(_currentCulture, normalized, StringComparison.OrdinalIgnoreCase))
            {
                _currentCulture = normalized;
                CultureChanged?.Invoke();
            }
        }
        catch (JSException)
        {
            // Keep default culture when storage is unavailable (e.g. prerender/test host).
        }

        ApplyCulture(_currentCulture);
    }

    public IReadOnlyDictionary<string, (string key, string caption)> GetSupportedLanguages() => SupportedLanguages;

    public (string key, string name) GetCurrentLanguage()
    {
        var key = GetCultureKey(_currentCulture);
        return SupportedLanguages.TryGetValue(key, out var culture)
            ? culture
            : SupportedLanguages[DefaultLanguage];
    }

    public string GetCurrentCulture() => _currentCulture;

    public void SetCurrentCulture(string culture)
    {
        var normalized = NormalizeCulture(culture);
        if (string.Equals(_currentCulture, normalized, StringComparison.OrdinalIgnoreCase))
            return;

        _currentCulture = normalized;
        ApplyCulture(_currentCulture);
        _ = PersistCultureAsync(_currentCulture);
        CultureChanged?.Invoke();
    }

    private async Task PersistCultureAsync(string culture)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, culture);
        }
        catch (JSException)
        {
            // UI culture already applied; persistence can fail on test hosts.
        }
    }

    private static string NormalizeCulture(string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
            return DefaultLanguage;

        var key = GetCultureKey(culture);
        return SupportedLanguages.ContainsKey(key) ? key : DefaultLanguage;
    }

    private static string GetCultureKey(string culture)
    {
        return culture.Length >= 2 ? culture[..2] : DefaultLanguage;
    }

    private static void ApplyCulture(string culture)
    {
        var cultureInfo = CultureInfo.GetCultureInfo(culture);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}