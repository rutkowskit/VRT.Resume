using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person.Components;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Profiles;

[Route(Routes.Profiles.List)]
public partial class ProfilesPage : IProfileExemptPage
{
    private const long MaxImportFileSize = 50 * 1024 * 1024;

    [Inject] private LocalProfileService ProfileService { get; set; } = null!;
    [Inject] private PwaDatabaseBackupService DatabaseBackup { get; set; } = null!;
    [Inject] private DummyCurrentUserService ProfileContext { get; set; } = null!;
    [Inject] private ICurrentUserService CurrentUser { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private UserNotificationService Notifications { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    private readonly List<LocalProfileDto> _profiles = [];
    private bool _loading = true;
    private bool _backupBusy;

    protected override async Task OnInitializedAsync() => await LoadProfilesAsync();

    private async Task LoadProfilesAsync()
    {
        _loading = true;
        try
        {
            _profiles.Clear();
            _profiles.AddRange(await ProfileService.GetAllAsync());
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task SelectProfileAsync(string userId)
    {
        await ProfileContext.SetContextAsync(userId);
        Navigation.NavigateTo(Routes.Home);
    }

    private async Task DeleteProfileAsync(LocalProfileDto profile)
    {
        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.Message, MessageKeys.DeleteProfileConfirm.GetMessageText(profile.DisplayName) },
        };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(LabelNames.DeleteEntry.GetLabelText(), parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
            return;

        var deleted = await ProfileService.DeleteAsync(profile.UserId);
        if (!deleted)
        {
            Notifications.ShowError(MessageKeys.ErrorOccured.GetMessageText());
            return;
        }

        if (string.Equals(CurrentUser.UserId, profile.UserId, StringComparison.Ordinal))
            await ProfileContext.ClearContextAsync();

        Notifications.ShowSuccess(MessageKeys.ProfileDeletedSuccess.GetMessageText(profile.DisplayName));
        await LoadProfilesAsync();
    }

    private async Task ExportDatabaseAsync()
    {
        _backupBusy = true;
        try
        {
            await DatabaseBackup.ExportAsync();
            Notifications.ShowSuccess(MessageKeys.DatabaseExportSuccess.GetMessageText());
        }
        catch (Exception)
        {
            Notifications.ShowError(MessageKeys.DatabaseBackupFailed.GetMessageText());
        }
        finally
        {
            _backupBusy = false;
        }
    }

    private async Task TriggerImportPickerAsync()
        => await Js.InvokeVoidAsync("pwaDbBackup.openFilePicker", "db-import-input");

    private async Task OnImportFileSelectedAsync(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file.Size > MaxImportFileSize)
        {
            Notifications.ShowError(MessageKeys.DatabaseImportTooLarge.GetMessageText());
            return;
        }

        byte[] data;
        try
        {
            await using var stream = file.OpenReadStream(MaxImportFileSize);
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            data = memory.ToArray();
        }
        catch (Exception)
        {
            Notifications.ShowError(MessageKeys.DatabaseBackupFailed.GetMessageText());
            return;
        }

        if (!PwaDatabaseBackupService.IsValidSqliteFile(data))
        {
            Notifications.ShowError(MessageKeys.DatabaseImportInvalid.GetMessageText());
            return;
        }

        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.Message, MessageKeys.ImportDatabaseConfirm.GetMessageText() },
            { x => x.ConfirmButtonText, LabelNames.ButtonImportDatabase.GetLabelText() },
            { x => x.ConfirmButtonColor, Color.Warning },
        };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(
            LabelNames.ButtonImportDatabase.GetLabelText(),
            parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
            return;

        _backupBusy = true;
        try
        {
            await DatabaseBackup.ImportAsync(data);
            await ProfileContext.ClearContextAsync();
            Navigation.NavigateTo(Routes.Profiles.List, forceLoad: true);
        }
        catch (InvalidDataException)
        {
            Notifications.ShowError(MessageKeys.DatabaseImportInvalid.GetMessageText());
        }
        catch (Exception)
        {
            Notifications.ShowError(MessageKeys.DatabaseBackupFailed.GetMessageText());
        }
        finally
        {
            _backupBusy = false;
        }
    }
}
