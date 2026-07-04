using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.DeletePersonsSkill;
using VRT.Resume.Application.Persons.Queries.GetPersonSkills;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person.Components;
using VRT.Resume.Pwa.Features.Person.Editors;

namespace VRT.Resume.Pwa.Features.Person.Tabs;

public partial class PersonSkillsTab
{
    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private bool _loading = true;
    private string? _loadError;
    private Dictionary<string, PersonSkillInListVM[]> _groups = [];

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _loadError = null;

        var outcome = await Mediator.SendAsync(
            new GetPersonSkillListQuery(),
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            _loadError = outcome.Result.Error;
            _groups = [];
        }
        else
        {
            _groups = outcome.Result.Value
                .OrderBy(i => i.Type)
                .GroupBy(i => i.Type)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.SkillId).ToArray());
        }

        _loading = false;
    }

    private async Task AddAsync() => await ShowEditorAsync(0);

    private async Task EditAsync(int id) => await ShowEditorAsync(id);

    private async Task ShowEditorAsync(int skillId)
    {
        var parameters = new DialogParameters<SkillEditorDialog>
        {
            { x => x.SkillId, skillId },
        };
        var dialog = await DialogService.ShowAsync<SkillEditorDialog>(null, parameters);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
            await LoadAsync();
    }

    private async Task DeleteAsync(int id, string name)
    {
        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.Message, $"Delete skill \"{name}\"?" },
        };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>("Delete", parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
            return;

        var outcome = await Mediator.SendAsync(
            new DeletePersonSkillCommand(id),
            new MediatorSendOptions { NotifyOnSuccess = true });

        if (outcome.Result.IsSuccess)
            await LoadAsync();
    }
}