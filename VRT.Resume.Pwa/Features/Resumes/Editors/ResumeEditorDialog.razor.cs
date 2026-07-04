using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Resumes.Commands.UpsertPersonResume;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Pwa.Features.Mediator;

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
    private bool _showProfilePhoto;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();

    protected override async Task OnInitializedAsync()
    {
        if (_isNew)
        {
            _loading = false;
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
