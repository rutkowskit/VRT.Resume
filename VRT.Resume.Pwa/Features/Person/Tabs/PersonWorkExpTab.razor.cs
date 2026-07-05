using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.DeletePersonExperience;
using VRT.Resume.Application.Persons.Commands.DeletePersonExperienceDuty;
using VRT.Resume.Application.Persons.Queries.GetPersonExperience;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person.Components;
using VRT.Resume.Pwa.Features.Person.Editors;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Person.Tabs;

public partial class PersonWorkExpTab
{
    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private bool _loading = true;
    private string? _loadError;
    private PersonExperienceInListVM[] _items = [];

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _loadError = null;

        var outcome = await Mediator.SendAsync(
            new GetPersonExperienceListQuery(),
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            _loadError = outcome.Result.Error;
            _items = [];
        }
        else
        {
            _items = outcome.Result.Value
                .OrderByDescending(i => i.FromDate)
                .ToArray();
        }

        _loading = false;
    }

    private async Task AddExperienceAsync() => await ShowExperienceEditorAsync(0);

    private async Task EditExperienceAsync(int id) => await ShowExperienceEditorAsync(id);

    private async Task ShowExperienceEditorAsync(int experienceId)
    {
        var parameters = new DialogParameters<ExperienceEditorDialog>
        {
            { x => x.ExperienceId, experienceId },
        };
        var dialog = await DialogService.ShowAsync<ExperienceEditorDialog>(null, parameters);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
            await LoadAsync();
    }

    private async Task DeleteExperienceAsync(int id, string name)
    {
        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.Message, MessageKeys.DeleteWorkExpConfirm.GetMessageText(name) },
        };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(LabelNames.DeleteEntry.GetLabelText(), parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
            return;

        var outcome = await Mediator.SendAsync(
            new DeletePersonExperienceCommand(id),
            new MediatorSendOptions { NotifyOnSuccess = true });

        if (outcome.Result.IsSuccess)
            await LoadAsync();
    }

    private async Task AddDutyAsync(int experienceId) => await ShowDutyEditorAsync(experienceId, 0);

    private async Task EditDutyAsync(int experienceId, int dutyId) => await ShowDutyEditorAsync(experienceId, dutyId);

    private async Task ShowDutyEditorAsync(int experienceId, int dutyId)
    {
        var parameters = new DialogParameters<ExperienceDutyEditorDialog>
        {
            { x => x.ExperienceId, experienceId },
            { x => x.DutyId, dutyId },
        };
        var dialog = await DialogService.ShowAsync<ExperienceDutyEditorDialog>(null, parameters);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
            await LoadAsync();
    }

    private async Task DeleteDutyAsync(int id, string name)
    {
        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.Message, MessageKeys.DeleteDutyConfirm.GetMessageText(name) },
        };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(LabelNames.DeleteEntry.GetLabelText(), parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
            return;

        var outcome = await Mediator.SendAsync(
            new DeletePersonExperienceDutyCommand(id),
            new MediatorSendOptions { NotifyOnSuccess = true });

        if (outcome.Result.IsSuccess)
            await LoadAsync();
    }

    private async Task EditDutySkillsAsync(int dutyId)
    {
        var parameters = new DialogParameters<DutySkillsEditorDialog>
        {
            { x => x.DutyId, dutyId },
        };
        var dialog = await DialogService.ShowAsync<DutySkillsEditorDialog>(null, parameters);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
            await LoadAsync();
    }
}
