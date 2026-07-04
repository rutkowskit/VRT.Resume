using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VRT.Resume.Pwa.Features.Person.Components;

public partial class ConfirmDeleteDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public string Title { get; set; } = "Delete";
    [Parameter] public string Message { get; set; } = "Are you sure you want to delete this item?";

    private void Cancel() => MudDialog.Cancel();

    private void Confirm() => MudDialog.Close(DialogResult.Ok(true));
}