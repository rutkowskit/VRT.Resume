using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Persons.Queries.GetPersonData;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private IActiveProfileContext ActiveProfile { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    private string? _activeProfileName;

    protected override void OnInitialized() => ActiveProfile.ContextChanged += OnContextChanged;

    protected override async Task OnInitializedAsync() => await LoadActiveProfileNameAsync();

    private void OnContextChanged()
    {
        _ = InvokeAsync(async () =>
        {
            await LoadActiveProfileNameAsync();
            StateHasChanged();
        });
    }

    private async Task LoadActiveProfileNameAsync()
    {
        if (!ActiveProfile.HasActiveContext)
        {
            _activeProfileName = null;
            return;
        }

        var outcome = await Mediator.SendAsync(
            new GetPersonDataQuery(),
            new MediatorSendOptions { NotifyOnFailure = false });

        _activeProfileName = outcome.Result.IsSuccess
            ? $"{outcome.Result.Value.FirstName} {outcome.Result.Value.LastName}".Trim()
            : null;
    }

    public void Dispose() => ActiveProfile.ContextChanged -= OnContextChanged;
}