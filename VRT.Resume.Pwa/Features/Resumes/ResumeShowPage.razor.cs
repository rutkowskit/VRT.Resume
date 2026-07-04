using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VRT.Resume.Application.Persons.Queries.GetProfileImage;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person;

namespace VRT.Resume.Pwa.Features.Resumes;

[Route(Routes.Resumes.Show)]
public partial class ResumeShowPage
{
    [Parameter] public int ResumeId { get; set; }

    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    private ResumeFullVM? _resume;
    private string _profileImageUrl = ProfileImageUrl.DefaultImagePath;
    private string _pageTitle = "Resume";
    private string? _loadError;
    private bool _loading = true;

    protected override async Task OnParametersSetAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _loadError = null;

        var outcome = await Mediator.SendAsync(
            new GetFullResumeQuery { ResumeId = ResumeId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            _loadError = outcome.Result.Error;
            _resume = null;
            _loading = false;
            return;
        }

        _resume = outcome.Result.Value;
        _pageTitle = string.IsNullOrWhiteSpace(_resume.Position)
            ? "Resume"
            : _resume.Position;

        if (_resume.ShowProfilePhoto)
        {
            var image = await Mediator.SendQueryAsync(new GetProfileImageQuery());
            _profileImageUrl = ProfileImageUrl.ToDataUrl(image) ?? ProfileImageUrl.DefaultImagePath;
        }

        _loading = false;
    }

    private async Task PrintAsync() => await Js.InvokeVoidAsync("print");
}