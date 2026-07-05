using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Person.Components;

public partial class ConfirmDeleteDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public string Title { get; set; } = LabelNames.DeleteEntry.GetLabelText();
    [Parameter] public string Message { get; set; } = MessageKeys.DeleteConfirmDefault.GetMessageText();
    [Parameter] public string ConfirmButtonText { get; set; } = LabelNames.ButtonDelete.GetLabelText();
    [Parameter] public Color ConfirmButtonColor { get; set; } = Color.Error;

    private void Cancel() => MudDialog.Cancel();

    private void Confirm() => MudDialog.Close(DialogResult.Ok(true));
}
