using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Resumes.Commands.MergeResumeSkills;
using VRT.Resume.Application.Resumes.Queries.GetResumeSkillList;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Resumes.Editors;

public partial class ResumeSkillsEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int ResumeId { get; set; }

    private readonly List<ResumeSkillRow> _skills = [];
    private bool _loading = true;
    private bool _saving;

    protected override async Task OnInitializedAsync()
    {
        var outcome = await Mediator.SendAsync(
            new GetResumeSkillListQuery { ResumeId = ResumeId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var list = outcome.Result.Value.ResumeSkills ?? [];
        _skills.AddRange(list.Select(s => new ResumeSkillRow
        {
            SkillId = s.SkillId,
            Name = s.Name,
            Type = s.Type,
            IsRelevant = s.IsRelevant,
            IsHidden = s.IsHidden,
            Position = s.Position,
        }));
        _loading = false;
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        _saving = true;

        var outcome = await Mediator.SendAsync(
            new MergeResumeSkillsCommand
            {
                ResumeId = ResumeId,
                ResumeSkills = _skills.Select(s => new ResumePersonSkillDto
                {
                    SkillId = s.SkillId,
                    IsRelevant = s.IsRelevant,
                    IsHidden = s.IsHidden,
                    Position = s.Position,
                }).ToArray(),
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }

    private sealed class ResumeSkillRow
    {
        public int SkillId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public bool IsRelevant { get; set; }
        public bool IsHidden { get; set; }
        public int Position { get; set; }
    }
}
