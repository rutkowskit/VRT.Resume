using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperience.Tests;

public class DeletePersonExperienceCommandTests
    : DeleteCommandTestBase<DeletePersonExperienceCommand, PersonExperience>
{
    public DeletePersonExperienceCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    protected override DeletePersonExperienceCommand CreateSut(int entityId)
    {
        return new DeletePersonExperienceCommand(entityId);
    }

    protected override DeletePersonExperienceCommand CreateSut(PersonExperience entity)
    {
        return CreateSut(entity.ExperienceId);
    }

    protected override Task<PersonExperience> SeedEntity()
    {
        return GetDbContext().SeedExperience();
    }
}