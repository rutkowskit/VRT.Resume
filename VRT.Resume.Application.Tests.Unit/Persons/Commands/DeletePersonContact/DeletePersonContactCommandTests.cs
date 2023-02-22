using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonContact;

public class DeletePersonContactCommandTests
    : DeleteCommandTestBase<DeletePersonContactCommand, PersonContact>
{
    public DeletePersonContactCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    protected override Task<PersonContact> SeedEntity()
        => GetDbContext().SeedContact();

    protected override DeletePersonContactCommand CreateSut(int contactId)
    {
        return new DeletePersonContactCommand(contactId);
    }

    protected override DeletePersonContactCommand CreateSut(PersonContact entity)
    {
        return CreateSut(entity.ContactId);
    }
}