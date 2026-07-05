using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Resumes.Commands.ClonePersonResume;
using VRT.Resume.Application.Resumes.Commands.DeletePersonResume;
using VRT.Resume.Application.Resumes.Queries.GetResumeList;
using VRT.Resume.Pwa.Components;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person.Components;
using VRT.Resume.Pwa.Features.Resumes.Editors;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Pages;

[Route(Routes.Home)]
public partial class Home
{
    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private readonly List<ResumeInListVM> _resumes = [];
    private bool _loading = true;

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        try
        {
            var items = await Mediator.SendQueryAsync(new GetResumeListQuery());
            _resumes.Clear();
            _resumes.AddRange(items.OrderByDescending(r => r.ModifiedDate));
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task AddAsync() => await ShowEditorAsync(0);

    private async Task EditAsync(int resumeId) => await ShowEditorAsync(resumeId);

    private async Task ShowEditorAsync(int resumeId)
    {
        var parameters = new DialogParameters<ResumeEditorDialog>
        {
            { x => x.ResumeId, resumeId },
        };
        var dialog = await DialogService.ShowAsync<ResumeEditorDialog>(null, parameters, PwaDialogOptions.ResumeForm);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
            await LoadAsync();
    }

    private async Task EditSkillsAsync(int resumeId)
    {
        var parameters = new DialogParameters<ResumeSkillsEditorDialog>
        {
            { x => x.ResumeId, resumeId },
        };
        var dialog = await DialogService.ShowAsync<ResumeSkillsEditorDialog>(null, parameters, PwaDialogOptions.LargeForm);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
            await LoadAsync();
    }

    private async Task CloneAsync(int resumeId, string description)
    {
        var outcome = await Mediator.SendAsync(
            new ClonePersonResumeCommand { ResumeId = resumeId },
            new MediatorSendOptions
            {
                NotifyOnSuccess = true,
                SuccessMessage = MessageKeys.ResumeClonedSuccess.GetMessageText(description),
            });

        if (outcome.Result.IsSuccess)
            await LoadAsync();
    }

    private async Task DeleteAsync(int resumeId, string description)
    {
        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.Message, MessageKeys.DeleteResumeConfirm.GetMessageText(description) },
        };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(LabelNames.DeleteEntry.GetLabelText(), parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
            return;

        var outcome = await Mediator.SendAsync(
            new DeletePersonResumeCommand(resumeId),
            new MediatorSendOptions { NotifyOnSuccess = true });

        if (outcome.Result.IsSuccess)
            await LoadAsync();
    }
}
