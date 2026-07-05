using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty;
using VRT.Resume.Application.Persons.Queries.GetPersonExperienceDuty;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Person.Editors;

public partial class ExperienceDutyEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int ExperienceId { get; set; }
    [Parameter] public int DutyId { get; set; }

    private MudForm? _form;
    private bool _isValid;
    private bool _loading = true;
    private bool _saving;
    private bool _isNew => DutyId == 0;

    private string _name = string.Empty;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();
    private readonly FormValiditySync _formValidity = new();

    private string _originalName = string.Empty;

    private bool CanSave => FormSaveGate.CanSave(_isValid, _loading, _saving, _isNew, IsDirty);

    private bool IsDirty => _name.Trim() != _originalName;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await _formValidity.OnAfterRenderAsync(_form, _loading);
    }

    private Task OnFieldChangedAsync() => _formValidity.OnFieldChangedAsync(_form);

    protected override async Task OnInitializedAsync()
    {
        if (_isNew)
        {
            CaptureSnapshot();
            _loading = false;
            _formValidity.RequestSync();
            return;
        }

        var outcome = await Mediator.SendAsync(
            new GetPersonExperienceDutyQuery { DutyId = DutyId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var item = outcome.Result.Value;
        _name = item.Name;
        ExperienceId = item.ExperienceId;
        CaptureSnapshot();
        _loading = false;
        _formValidity.RequestSync();
    }

    private void CaptureSnapshot() => _originalName = _name.Trim();

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
            new UpsertPersonExperienceDutyCommand
            {
                ExperienceId = ExperienceId,
                DutyId = DutyId,
                Name = _name.Trim(),
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _fieldErrors = outcome.FieldErrors;
        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }
}