using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VRT.Resume.Application.Persons.Queries.GetProfileImage;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person;
using VRT.Resume.Pwa.Features.Resumes.Templates;
using VRT.Resume.Pwa.Layout;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Resumes;

[Route(Routes.Resumes.Show)]
[Layout(typeof(ResumeShowLayout))]
public partial class ResumeShowPage : IDisposable
{
    [Parameter] public int ResumeId { get; set; }

    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;
    [Inject] private PwaCultureService CultureService { get; set; } = null!;
    [Inject] private ResumePrintTemplateStorage TemplateStorage { get; set; } = null!;

    private ResumeFullVM? _resume;
    private string _profileImageUrl = ProfileImageUrl.DefaultImagePath;
    private string? _loadError;
    private bool _loading = true;

    private readonly IReadOnlyList<ResumeTemplateDescriptor> _templates = ResumeTemplateRegistry.All;
    private string _selectedTemplateId = ResumeTemplateIds.Classic;
    private ResumeTemplateDescriptor _activeTemplate = ResumeTemplateRegistry.GetOrDefault(null);

    private Dictionary<string, object> _documentParameters => new()
    {
        ["Model"] = _resume!,
        ["ProfileImageUrl"] = _profileImageUrl,
    };

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
        else
        {
            _profileImageUrl = ProfileImageUrl.DefaultImagePath;
        }

        var storedTemplateId = await TemplateStorage.GetAsync(ResumeId);
        _selectedTemplateId = ResumeTemplateRegistry.Normalize(storedTemplateId);
        _activeTemplate = ResumeTemplateRegistry.GetOrDefault(_selectedTemplateId);

        _loading = false;
    }

    private async Task OnTemplateChangedAsync(string templateId)
    {
        _selectedTemplateId = ResumeTemplateRegistry.Normalize(templateId);
        _activeTemplate = ResumeTemplateRegistry.GetOrDefault(_selectedTemplateId);
        await TemplateStorage.SetAsync(ResumeId, _selectedTemplateId);
    }

    private async Task PrintAsync() => await Js.InvokeVoidAsync("print");

    public void Dispose() => CultureService.CultureChanged -= OnCultureChanged;
}