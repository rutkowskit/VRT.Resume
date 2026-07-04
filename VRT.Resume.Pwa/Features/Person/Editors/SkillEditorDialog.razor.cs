using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.UpsertPersonSkill;
using VRT.Resume.Application.Persons.Queries.GetPersonSkills;
using VRT.Resume.Domain.Common;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Person.Editors;

public partial class SkillEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int SkillId { get; set; }

    private MudForm? _form;
    private bool _isValid;
    private bool _loading = true;
    private bool _saving;
    private bool _isNew => SkillId == 0;

    private SkillTypes _skillType = SkillTypes.Technical;
    private string _skillName = string.Empty;
    private string _skillLevel = string.Empty;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();

    protected override async Task OnInitializedAsync()
    {
        if (_isNew)
        {
            _loading = false;
            return;
        }

        var outcome = await Mediator.SendAsync(
            new GetPersonSkillQuery { SkillId = SkillId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var item = outcome.Result.Value;
        _skillType = item.Type;
        _skillName = item.Name;
        _skillLevel = item.Level;
        _loading = false;
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        if (_form is not null)
        {
            await _form.ValidateAsync();
            if (!_form.IsValid)
                return;
        }

        _saving = true;
        _fieldErrors = new Dictionary<string, string[]>();

        var outcome = await Mediator.SendAsync(
            new UpsertPersonSkillCommand
            {
                SkillId = SkillId,
                SkillType = _skillType,
                SkillName = _skillName.Trim(),
                SkillLevel = _skillLevel.Trim(),
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _fieldErrors = outcome.FieldErrors;
        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }
}
