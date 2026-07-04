using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.UpdatePersonData;
using VRT.Resume.Application.Persons.Queries.GetPersonData;
using VRT.Resume.Application.Persons.Queries.GetProfileImage;
using VRT.Resume.Pwa.Features.Mediator;
using ProfileImageUrl = VRT.Resume.Pwa.Features.Person.ProfileImageUrl;

namespace VRT.Resume.Pwa.Features.Person.Tabs;

public partial class PersonProfileTab
{
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    private MudForm? _form;
    private bool _isValid;
    private bool _loading = true;
    private bool _saving;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private DateTime? _dateOfBirth;
    private string? _loadError;
    private string _imageUrl = ProfileImageUrl.DefaultImagePath;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _loadError = null;

        var outcome = await Mediator.SendAsync(
            new GetPersonDataQuery(),
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            _loadError = outcome.Result.Error;
            _loading = false;
            return;
        }

        _firstName = outcome.Result.Value.FirstName;
        _lastName = outcome.Result.Value.LastName;
        _dateOfBirth = outcome.Result.Value.DateOfBirth;

        var image = await Mediator.SendQueryAsync(new GetProfileImageQuery());
        _imageUrl = ProfileImageUrl.ToDataUrl(image) ?? ProfileImageUrl.DefaultImagePath;

        _loading = false;
    }

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
            new UpdatePersonDataCommand
            {
                FirstName = _firstName.Trim(),
                LastName = _lastName.Trim(),
                DateOfBirth = _dateOfBirth,
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _fieldErrors = outcome.FieldErrors;
        _saving = false;
    }
}
