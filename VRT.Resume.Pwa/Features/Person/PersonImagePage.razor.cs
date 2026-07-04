using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using VRT.Resume.Application.Persons.Commands.UpsertProfileImage;
using VRT.Resume.Application.Persons.Queries.GetProfileImage;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Person;

[Route(Routes.Persons.Image)]
public partial class PersonImagePage
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private UserNotificationService Notifications { get; set; } = null!;

    private bool _loading = true;
    private bool _saving;
    private string _displayUrl = ProfileImageUrl.DefaultImagePath;
    private string? _fileName;
    private byte[]? _pendingImageData;
    private string? _pendingImageType;

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;

        var image = await Mediator.SendQueryAsync(new GetProfileImageQuery());
        _displayUrl = ProfileImageUrl.ToDataUrl(image) ?? ProfileImageUrl.DefaultImagePath;
        _pendingImageData = null;
        _pendingImageType = null;
        _fileName = null;

        _loading = false;
    }

    private async Task OnFileSelectedAsync(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file.Size > MaxFileSize)
        {
            Notifications.ShowError("Image must be 5 MB or smaller.");
            return;
        }

        try
        {
            await using var stream = file.OpenReadStream(MaxFileSize);
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);

            _pendingImageData = memory.ToArray();
            _pendingImageType = string.IsNullOrWhiteSpace(file.ContentType) ? "image/png" : file.ContentType;
            _fileName = file.Name;
            _displayUrl = $"data:{_pendingImageType};base64,{Convert.ToBase64String(_pendingImageData)}";
        }
        catch (Exception)
        {
            Notifications.ShowError("Could not read the selected image.");
        }
    }

    private async Task SaveAsync()
    {
        if (_pendingImageData is null || _pendingImageType is null)
            return;

        _saving = true;

        var outcome = await Mediator.SendAsync(
            new UpsertProfileImageCommand
            {
                ImageData = _pendingImageData,
                ImageType = _pendingImageType,
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _saving = false;

        if (outcome.Result.IsSuccess)
            await LoadAsync();
    }
}