using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Resumes.Commands.UpsertPersonResume;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person;

namespace VRT.Resume.Pwa.Features.Resumes.Editors;

public partial class ResumeEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int ResumeId { get; set; }

    private MudForm? _form;
    private bool _isValid;
    private bool _loading = true;
    private bool _saving;
    private bool _isNew => ResumeId == 0;

    private string _description = string.Empty;
    private string _position = string.Empty;
    private string _summary = string.Empty;
    private string _dataProcessingPermission = string.Empty;
    private bool _showProfilePhoto = true;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();
    private readonly FormValiditySync _formValidity = new();

    private string _originalDescription = string.Empty;
    private string _originalPosition = string.Empty;
    private string _originalSummary = string.Empty;
    private string _originalDataProcessingPermission = string.Empty;
    private bool _originalShowProfilePhoto = true;

    private bool CanSave => FormSaveGate.CanSave(_isValid, _loading, _saving, _isNew, IsDirty);

    private bool IsDirty =>
        _description.Trim() != _originalDescription
        || _position.Trim() != _originalPosition
        || _summary.Trim() != _originalSummary
        || _dataProcessingPermission.Trim() != _originalDataProcessingPermission
        || _showProfilePhoto != _originalShowProfilePhoto;

    protected override async Task OnInitializedAsync()
    {
        if (_isNew)
        {
            _loading = false;
            CaptureSnapshot();
            _formValidity.RequestSync();
            return;
        }

        var outcome = await Mediator.SendAsync(
            new GetResumeQuery { ResumeId = ResumeId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var item = outcome.Result.Value;
        _description = item.Description;
        _position = item.Position;
        _summary = item.Summary ?? string.Empty;
        _dataProcessingPermission = item.DataProcessingPermission ?? string.Empty;
        _showProfilePhoto = item.ShowProfilePhoto;
        CaptureSnapshot();
        _loading = false;
        _formValidity.RequestSync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await _formValidity.OnAfterRenderAsync(_form, _loading);
    }

    private Task OnFieldChangedAsync() => _formValidity.OnFieldChangedAsync(_form);

    private void CaptureSnapshot()
    {
        _originalDescription = _description.Trim();
        _originalPosition = _position.Trim();
        _originalSummary = _summary.Trim();
        _originalDataProcessingPermission = _dataProcessingPermission.Trim();
        _originalShowProfilePhoto = _showProfilePhoto;
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
            new UpsertPersonResumeCommand
            {
                ResumeId = ResumeId,
                Description = _description.Trim(),
                Position = _position.Trim(),
                Summary = _summary.Trim(),
                DataProcessingPermission = _dataProcessingPermission.Trim(),
                ShowProfilePhoto = _showProfilePhoto,
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _fieldErrors = outcome.FieldErrors;
        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }
}