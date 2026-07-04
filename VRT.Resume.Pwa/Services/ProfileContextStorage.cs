using Microsoft.JSInterop;

namespace VRT.Resume.Pwa.Services;

public sealed class ProfileContextStorage(IJSRuntime jsRuntime)
{
    public const string StorageKey = "VRT.Resume.ActiveProfileUserId";

    public async Task<string?> GetActiveUserIdAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
        }
        catch (JSException)
        {
            return null;
        }
    }

    public async Task SetActiveUserIdAsync(string? userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId))
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, StorageKey);
            return;
        }

        await jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, StorageKey, userId);
    }
}