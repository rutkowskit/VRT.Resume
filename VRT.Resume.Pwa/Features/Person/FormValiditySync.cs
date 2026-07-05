using MudBlazor;

namespace VRT.Resume.Pwa.Features.Person;

/// <summary>
/// MudForm.IsValid defaults to false and does not always refresh when bound fields change.
/// Request sync after load, then validate again on each field change and after first render.
/// </summary>
internal sealed class FormValiditySync
{
    private bool _pending;

    public void RequestSync() => _pending = true;

    public async Task OnAfterRenderAsync(MudForm? form, bool loading)
    {
        if (!_pending || loading || form is null)
            return;

        _pending = false;
        await form.ValidateAsync();
    }

    public Task OnFieldChangedAsync(MudForm? form) =>
        form is null ? Task.CompletedTask : form.ValidateAsync();
}

internal static class FormSaveGate
{
    public static bool CanSave(bool isValid, bool loading, bool saving, bool isNew, bool isDirty) =>
        isValid && !loading && !saving && (isNew || isDirty);

    public static bool DatesEqual(DateTime? left, DateTime? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Value.Date == right.Value.Date;
    }
}