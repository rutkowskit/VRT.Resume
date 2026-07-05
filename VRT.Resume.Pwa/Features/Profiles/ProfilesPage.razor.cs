using Microsoft.AspNetCore.Components;
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
    [Inject] private LocalProfileService ProfileService { get; set; } = null!;
    [Inject] private DummyCurrentUserService ProfileContext { get; set; } = null!;
    [Inject] private ICurrentUserService CurrentUser { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private UserNotificationService Notifications { get; set; } = null!;

    private readonly List<LocalProfileDto> _profiles = [];
    private bool _loading = true;

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
}
