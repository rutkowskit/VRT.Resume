using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.UpsertPersonContact;
using VRT.Resume.Application.Persons.Queries.GetPersonContacts;
using VRT.Resume.Pwa.Features.Mediator;

namespace VRT.Resume.Pwa.Features.Person.Editors;

public partial class ContactEditorDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private MediatorSender Mediator { get; set; } = null!;

    [Parameter] public int ContactId { get; set; }

    private MudForm? _form;
    private bool _isValid;
    private bool _loading = true;
    private bool _saving;
    private bool _isNew => ContactId == 0;

    private string _name = string.Empty;
    private string _value = string.Empty;
    private string _url = string.Empty;
    private string _icon = string.Empty;
    private IReadOnlyDictionary<string, string[]> _fieldErrors = new Dictionary<string, string[]>();

    protected override async Task OnInitializedAsync()
    {
        if (_isNew)
        {
            _loading = false;
            return;
        }

        var outcome = await Mediator.SendAsync(
            new GetPersonContactQuery { ContactId = ContactId },
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            MudDialog.Cancel();
            return;
        }

        var item = outcome.Result.Value;
        _name = item.Name;
        _value = item.Value;
        _url = item.Url ?? string.Empty;
        _icon = item.Icon ?? string.Empty;
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
            new UpsertPersonContactCommand
            {
                ContactId = ContactId,
                Name = _name.Trim(),
                Value = _value.Trim(),
                Url = _url.Trim(),
                Icon = _icon.Trim(),
            },
            new MediatorSendOptions { NotifyOnSuccess = true });

        _fieldErrors = outcome.FieldErrors;
        _saving = false;

        if (outcome.Result.IsSuccess)
            MudDialog.Close(DialogResult.Ok(true));
    }
}