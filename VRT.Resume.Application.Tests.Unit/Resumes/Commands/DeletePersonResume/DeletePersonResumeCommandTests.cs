using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Resumes.Commands.DeletePersonResume.Tests;

public class DeletePersonResumeCommandTests
    : DeleteCommandTestBase<DeletePersonResumeCommand, PersonResume>
{
    public DeletePersonResumeCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    protected override DeletePersonResumeCommand CreateSut(int entityId)
    {
        return new DeletePersonResumeCommand(entityId);
    }

    protected override DeletePersonResumeCommand CreateSut(PersonResume entity)
    {
        return CreateSut(entity.ResumeId);
    }

    protected override Task<PersonResume> SeedEntity()
    {
        return GetDbContext().SeedPersonResume();
    }
}