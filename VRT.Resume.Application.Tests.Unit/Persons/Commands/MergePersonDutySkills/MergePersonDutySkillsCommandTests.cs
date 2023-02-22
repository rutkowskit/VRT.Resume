using FluentAssertions;
using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.MergePersonDutySkills;

public sealed class MergePersonDutySkillsCommandTests : CommandTestBase<MergePersonDutySkillsCommand>
{
    public MergePersonDutySkillsCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    [Fact()]
    public async Task Send_CommandWithInvalidEntityId_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.DutyId = 0;
        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithEntityIdThatIsNotInDb_ShouldFail()
    {
        var sut = CreateSut();
        sut.DutyId = 434343434;
        await Send(sut).AssertFail();
    }

    [Fact()]
    public async Task Send_CommandWithNullDutySkillList_ShouldDoNoChanges()
    {
        var db = GetDbContext();
        var exp = await db.SeedExperience();
        var duty = exp.PersonExperienceDuty.First();

        var sut = CreateSut();        
        sut.DutyId = duty.DutyId;
        sut.DutySkills = null!;

        var result = await Send(sut);
        result.AssertSuccess();
        GetDbContext()
            .Set<PersonExperienceDutySkill>()
            .ToArray()
            .Should().HaveCount(1);
    }

    [Fact()]
    public async Task Send_CommandWithRelevantDutySkills_AddedPersonExpDutySkill()
    {
        var db = GetDbContext();
        var exp = await db.SeedExperience();
        var duty = exp.PersonExperienceDuty.First();
        var newSkill = await db.SeedSkill();

        var sut = CreateSut();
        sut.DutyId = duty.DutyId;
        
        sut.DutySkills = new[]
        {
            new PersonExpDutySkillDto()
            {
                IsRelevant = true,
                SkillId = newSkill.SkillId //should be added 
            }
        };
        
        await Send(sut).AssertSuccess();

        db = GetDbContext();
        var dutySkills = db.Set<PersonExperienceDutySkill>().ToList();
        dutySkills.Should().HaveCount(2); //one default + one added

        dutySkills
            .FirstOrDefault(s => s.SkillId == newSkill.SkillId)
            .Should()
            .NotBeNull();
    }

    [Fact()]
    public async Task Send_CommandWithIrrelevantDutySkills_UnchangedDutySkills()
    {
        var db = GetDbContext();
        var exp = await db.SeedExperience();
        var newSkill = await db.SeedSkill();
        var duty = exp.PersonExperienceDuty.First();

        var sut = CreateSut();
        sut.DutyId = duty.DutyId;
        
        sut.DutySkills = new[]
        {
            new PersonExpDutySkillDto()
            {
                IsRelevant = false,
                SkillId = newSkill.SkillId
            }
        };

        var result = await Send(sut);
        
        result.AssertSuccess();

        GetDbContext()
            .Set<PersonExperienceDutySkill>()
            .ToArray()
            .Should().HaveCount(1);
    }

    [Fact()]
    public async Task Send_CommandWithIrrelevantExistingDutySkills_RemovedDutySkill()
    {
        var db = GetDbContext();
        var exp = await db.SeedExperience();
        var duty = exp.PersonExperienceDuty.First();
        var skill = duty.PersonExperienceDutySkill.First();
        
        var sut = CreateSut();
        sut.DutyId = duty.DutyId;
        sut.DutySkills = new[]
        {
            new PersonExpDutySkillDto()
            {
                IsRelevant = false,
                SkillId = skill.SkillId
            }
        };
        
        var result = await Send(sut);

        result.AssertSuccess();
        GetDbContext().Set<PersonExperienceDutySkill>()
            .ToArray()
            .Should().HaveCount(0);
    }
    
    protected override MergePersonDutySkillsCommand CreateSut()
    {
        return new MergePersonDutySkillsCommand()
        {
            DutyId = 444
        };
    }
}
