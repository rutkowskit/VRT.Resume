using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VRT.Resume.Application.Persons.Queries.GetProfileImage;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Resumes;

[Route(Routes.Resumes.Show)]
public partial class ResumeShowPage : IDisposable
{
    [Parameter] public int ResumeId { get; set; }

    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;
    [Inject] private PwaCultureService CultureService { get; set; } = null!;

    private ResumeFullVM? _resume;
    private string _profileImageUrl = ProfileImageUrl.DefaultImagePath;
    private string? _loadError;
    private bool _loading = true;

    private string PageTitleText => string.IsNullOrWhiteSpace(_resume?.Position)
        ? LabelNames.PageResume.GetLabelText()
        : _resume.Position;

    protected override void OnInitialized() => CultureService.CultureChanged += OnCultureChanged;

    protected override async Task OnParametersSetAsync() => await LoadAsync();

    private void OnCultureChanged() => _ = InvokeAsync(StateHasChanged);

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

        if (_resume.ShowProfilePhoto)
        {
            var image = await Mediator.SendQueryAsync(new GetProfileImageQuery());
            _profileImageUrl = ProfileImageUrl.ToDataUrl(image) ?? ProfileImageUrl.DefaultImagePath;
        }

        _loading = false;
    }

    private async Task PrintAsync() => await Js.InvokeVoidAsync("print");

    public void Dispose() => CultureService.CultureChanged -= OnCultureChanged;
}