using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.CreatePersonAccount;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Features.Profiles;

[Route(Routes.Profiles.Create)]
public partial class ProfilesCreatePage : IProfileExemptPage
{
    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private DummyCurrentUserService ProfileContext { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private MudForm? _form;
    private bool _isValid;
    private bool _submitting;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string? _email;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();

    private async Task CreateProfileAsync()
    {
        if (_form is not null)
        {
            await _form.ValidateAsync();
            if (!_form.IsValid)
                return;
        }

        _submitting = true;
        _fieldErrors = new Dictionary<string, string[]>();

        var userId = $"local:{Guid.NewGuid():N}";
        var outcome = await Mediator.SendAsync(new CreatePersonAccountCommand
        {
            UserId = userId,
            FirstName = _firstName.Trim(),
            LastName = _lastName.Trim(),
            Email = string.IsNullOrWhiteSpace(_email) ? null : _email.Trim(),
        });

        _fieldErrors = outcome.FieldErrors;

        if (outcome.Result.IsFailure)
        {
            _submitting = false;
            return;
        }

        await ProfileContext.SetContextAsync(userId);
        Navigation.NavigateTo(Routes.Home);
    }
}