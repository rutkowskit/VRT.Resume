using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperienceDuty;

public class DeletePersonExperienceDutyCommandTests
    : DeleteCommandTestBase<DeletePersonExperienceDutyCommand, PersonExperienceDuty>
{
    public DeletePersonExperienceDutyCommandTests(ApplicationFixture fixture) : base(fixture)
    {
    }

    protected override DeletePersonExperienceDutyCommand CreateSut(int entityId)
    {
        return new DeletePersonExperienceDutyCommand(entityId);
    }

    protected override DeletePersonExperienceDutyCommand CreateSut(PersonExperienceDuty entity)
    {
        return CreateSut(entity.DutyId);
    }    

    protected override async Task<PersonExperienceDuty> SeedEntity()
    {
        var result = await GetDbContext().SeedExperience();
        return result.PersonExperienceDuty.First();
    }    
}