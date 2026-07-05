using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.UpsertPersonExperience;
using VRT.Resume.Application.Persons.Queries.GetPersonExperience;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Person.Editors;

public partial class ExperienceEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int ExperienceId { get; set; }

    private MudForm? _form;
    private bool _isValid;
    private bool _loading = true;
    private bool _saving;
    private bool _isNew => ExperienceId == 0;

    private string _position = string.Empty;
    private string _companyName = string.Empty;
    private string _location = string.Empty;
    private DateTime? _fromDate = DateTime.Today;
    private DateTime? _toDate;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();
    private readonly FormValiditySync _formValidity = new();

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
            _loading = false;
            _formValidity.RequestSync();
            return;
        }

        var outcome = await Mediator.SendAsync(
            new GetPersonExperienceQuery { ExperienceId = ExperienceId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var item = outcome.Result.Value;
        _position = item.Position;
        _companyName = item.CompanyName;
        _location = item.Location ?? string.Empty;
        _fromDate = item.FromDate;
        _toDate = item.ToDate;
        _loading = false;
        _formValidity.RequestSync();
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
            new UpsertPersonExperienceCommand
            {
                ExperienceId = ExperienceId,
                Position = _position.Trim(),
                CompanyName = _companyName.Trim(),
                Location = _location.Trim(),
                FromDate = _fromDate ?? DateTime.Today,
                ToDate = _toDate,
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _fieldErrors = outcome.FieldErrors;
        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }
}
