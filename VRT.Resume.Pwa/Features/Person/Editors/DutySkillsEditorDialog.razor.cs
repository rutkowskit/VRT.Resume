using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.MergePersonDutySkills;
using VRT.Resume.Application.Persons.Queries.GetPersonExperienceDutySkillList;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Person.Editors;

public partial class DutySkillsEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int DutyId { get; set; }

    private readonly List<DutySkillRow> _skills = [];
    private bool _loading = true;
    private bool _saving;

    protected override async Task OnInitializedAsync()
    {
        var outcome = await Mediator.SendAsync(
            new GetPersonExperienceDutySkillListQuery { DutyId = DutyId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var list = outcome.Result.Value.DutySkills ?? [];
        _skills.AddRange(list.Select(s => new DutySkillRow
        {
            SkillId = s.SkillId,
            Name = s.Name,
            Type = s.Type,
            IsRelevant = s.IsRelevant,
        }));
        _loading = false;
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        _saving = true;

        var outcome = await Mediator.SendAsync(
            new MergePersonDutySkillsCommand
            {
                DutyId = DutyId,
                DutySkills = _skills.Select(s => new PersonExpDutySkillDto
                {
                    SkillId = s.SkillId,
                    IsRelevant = s.IsRelevant,
                }).ToArray(),
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }

    private sealed class DutySkillRow
    {
        public int SkillId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public bool IsRelevant { get; set; }
    }
}
