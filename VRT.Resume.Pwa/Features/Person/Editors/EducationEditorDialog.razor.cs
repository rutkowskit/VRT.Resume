using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.UpsertPersonEducation;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Person.Editors;

public partial class EducationEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int EducationId { get; set; }

    private MudForm? _form;
    private bool _isValid;
    private bool _loading = true;
    private bool _saving;
    private bool _isNew => EducationId == 0;

    private string _schoolName = string.Empty;
    private string _field = string.Empty;
    private string _degree = string.Empty;
    private string _specialization = string.Empty;
    private string _thesisTitle = string.Empty;
    private string _grade = string.Empty;
    private DateTime? _fromDate = DateTime.Today;
    private DateTime? _toDate = DateTime.Today;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();
    private readonly FormValiditySync _formValidity = new();

    private string _originalSchoolName = string.Empty;
    private string _originalField = string.Empty;
    private string _originalDegree = string.Empty;
    private string _originalSpecialization = string.Empty;
    private string _originalThesisTitle = string.Empty;
    private string _originalGrade = string.Empty;
    private DateTime? _originalFromDate = DateTime.Today;
    private DateTime? _originalToDate = DateTime.Today;

    private bool CanSave => FormSaveGate.CanSave(_isValid, _loading, _saving, _isNew, IsDirty);

    private bool IsDirty =>
        _schoolName.Trim() != _originalSchoolName
        || _field.Trim() != _originalField
        || _degree.Trim() != _originalDegree
        || _specialization.Trim() != _originalSpecialization
        || _thesisTitle.Trim() != _originalThesisTitle
        || _grade.Trim() != _originalGrade
        || !FormSaveGate.DatesEqual(_fromDate, _originalFromDate)
        || !FormSaveGate.DatesEqual(_toDate, _originalToDate);

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
            new GetPersonEducationQuery(EducationId),
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var item = outcome.Result.Value;
        _schoolName = item.SchoolName;
        _field = item.Field ?? string.Empty;
        _degree = item.Degree ?? string.Empty;
        _specialization = item.Specialization ?? string.Empty;
        _thesisTitle = item.ThesisTitle ?? string.Empty;
        _grade = item.Grade ?? string.Empty;
        _fromDate = item.FromDate;
        _toDate = item.ToDate ?? item.FromDate;
        CaptureSnapshot();
        _loading = false;
        _formValidity.RequestSync();
    }

    private void CaptureSnapshot()
    {
        _originalSchoolName = _schoolName.Trim();
        _originalField = _field.Trim();
        _originalDegree = _degree.Trim();
        _originalSpecialization = _specialization.Trim();
        _originalThesisTitle = _thesisTitle.Trim();
        _originalGrade = _grade.Trim();
        _originalFromDate = _fromDate;
        _originalToDate = _toDate;
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
            new UpsertPersonEducationCommand
            {
                EducationId = EducationId,
                SchoolName = _schoolName.Trim(),
                Field = _field.Trim(),
                Degree = _degree.Trim(),
                Specialization = _specialization.Trim(),
                ThesisTitle = _thesisTitle.Trim(),
                Grade = _grade.Trim(),
                FromDate = _fromDate ?? DateTime.Today,
                ToDate = _toDate ?? _fromDate ?? DateTime.Today,
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _fieldErrors = outcome.FieldErrors;
        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }
}