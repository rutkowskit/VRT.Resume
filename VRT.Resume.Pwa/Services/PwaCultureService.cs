using System.Globalization;
using Microsoft.JSInterop;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Pwa.Services;

public sealed class PwaCultureService(IServiceScopeFactory scopeFactory) : ICultureService
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

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await WithJsAsync(async js =>
        {
            try
            {
                var stored = await js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
                _currentCulture = NormalizeCulture(stored);
            }
            catch (JSException)
            {
                _currentCulture = DefaultLanguage;
            }

            ApplyCulture(_currentCulture);
            CultureChanged?.Invoke();
        });
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
        _currentCulture = NormalizeCulture(culture);
        ApplyCulture(_currentCulture);
        _ = PersistCultureAsync(_currentCulture);
        CultureChanged?.Invoke();
    }

    private async Task PersistCultureAsync(string culture)
    {
        await WithJsAsync(async js =>
        {
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, culture);
        });
    }

    private async Task WithJsAsync(Func<IJSRuntime, Task> action)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var js = scope.ServiceProvider.GetRequiredService<IJSRuntime>();
        await action(js);
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