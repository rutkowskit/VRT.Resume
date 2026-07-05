using Microsoft.JSInterop;

namespace VRT.Resume.Pwa.Services;

public sealed class ResumePrintTemplateStorage(IJSRuntime jsRuntime)
{
    public static string KeyFor(int resumeId) => $"VRT.Resume.PrintTemplate.{resumeId}";

    public async Task<string?> GetAsync(int resumeId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                cancellationToken,
                KeyFor(resumeId));
        }
        catch (JSException)
        {
            return null;
        }
    }

    public async Task SetAsync(int resumeId, string templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync(
                "localStorage.setItem",
                cancellationToken,
                KeyFor(resumeId),
                templateId);
        }
        catch (JSException)
        {
            // Ignore when localStorage is unavailable (e.g. private mode restrictions).
        }
    }
}