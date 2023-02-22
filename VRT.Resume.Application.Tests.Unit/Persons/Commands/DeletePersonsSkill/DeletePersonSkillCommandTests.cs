using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonsSkill;

public class DeletePersonSkillCommandTests
    : DeleteCommandTestBase<DeletePersonSkillCommand, PersonSkill>
{
    public DeletePersonSkillCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    protected override DeletePersonSkillCommand CreateSut(int entityId)
    {
        return new DeletePersonSkillCommand(entityId);
    }

    protected override DeletePersonSkillCommand CreateSut(PersonSkill entity)
    {
        return CreateSut(entity.SkillId);
    }

    protected override async Task<PersonSkill> SeedEntity()
    {
        var exp = await GetDbContext().SeedExperience();
        return exp.PersonExperienceDuty.First().PersonExperienceDutySkill.First().Skill;
    }
}