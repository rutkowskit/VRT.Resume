using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Application.Persons.Commands.DeletePersonContact;
using VRT.Resume.Application.Persons.Queries.GetPersonContacts;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Features.Person.Components;
using VRT.Resume.Pwa.Features.Person.Editors;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Person.Tabs;

public partial class PersonContactsTab
{
    [Inject] private MediatorSender Mediator { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private bool _loading = true;
    private string? _loadError;
    private PersonContactInListVM[] _items = [];

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _loading = true;
        _loadError = null;

        var outcome = await Mediator.SendAsync(
            new GetPersonContactListQuery(),
            new MediatorSendOptions { NotifyOnFailure = true });

        if (outcome.Result.IsFailure)
        {
            _loadError = outcome.Result.Error;
            _items = [];
        }
        else
        {
            _items = outcome.Result.Value;
        }

        _loading = false;
    }

    private async Task AddAsync() => await ShowEditorAsync(0);

    private async Task EditAsync(int id) => await ShowEditorAsync(id);

    private async Task ShowEditorAsync(int contactId)
    {
        var parameters = new DialogParameters<ContactEditorDialog>
        {
            { x => x.ContactId, contactId },
        };
        var dialog = await DialogService.ShowAsync<ContactEditorDialog>(null, parameters);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
            await LoadAsync();
    }

    private async Task DeleteAsync(int id, string name)
    {
        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.Message, MessageKeys.DeleteContactConfirm.GetMessageText(name) },
        };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(LabelNames.DeleteEntry.GetLabelText(), parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
            return;

        var outcome = await Mediator.SendAsync(
            new DeletePersonContactCommand(id),
            new MediatorSendOptions { NotifyOnSuccess = true });

        if (outcome.Result.IsSuccess)
            await LoadAsync();
    }
}