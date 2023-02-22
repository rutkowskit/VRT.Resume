using FluentAssertions;
using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Commands.MergeResumeSkills;

public sealed class MergeResumeSkillsCommandTests : CommandTestBase<MergeResumeSkillsCommand>
{
    public MergeResumeSkillsCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    [Fact()]
    public async Task Send_CommandWithInvalidResumeId_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.ResumeId = 0;
        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithResumeIdThatIsNotInDb_ShouldFail()
    {
        var sut = CreateSut();
        sut.ResumeId = 434343434;
        await Send(sut).AssertFail();
    }

    [Fact()]
    public async Task Send_CommandWithNullResumeSkillList_ShouldDoNoChanges()
    {
        var db = GetDbContext();
        var resumeSeed = await db.SeedPersonResume();

        var sut = CreateSut(resumeSeed);
        sut.ResumeSkills = null!;
        
        var result = await Send(sut);

        result.AssertSuccess();
        GetDbContext().Set<ResumePersonSkill>().ToArray().Should().HaveCount(1);
    }

    [Fact()]
    public async Task Send_CommandWithRelevantDutySkills_AddedResumePersonSkill()
    {
        var db = GetDbContext();
        var skillSeed = await db.SeedSkill();
        var resumeSeed = await db.SeedPersonResume();
        var sut = CreateSut(resumeSeed);
        
        sut.ResumeSkills = new[]
        {
            new ResumePersonSkillDto()
            {
                IsRelevant = true,
                SkillId = skillSeed.SkillId//should be added 
            }
        };


        var result = await Send(sut);

        result.AssertSuccess();
        db = GetDbContext();
        var skillSet = db.Set<ResumePersonSkill>().ToList();
        skillSet.Should().HaveCount(2);//one default + one added

        skillSet
            .FirstOrDefault(s => s.SkillId == skillSeed.SkillId && s.ResumeId == resumeSeed.ResumeId)
            .Should()
            .NotBeNull("Relevant skill should be added to the resume");
    }

    [Fact()]
    public async Task Send_CommandWithIrrelevantDutySkills_UnchangedDutySkills()
    {        
        var db = GetDbContext();
        var skillSeed = await db.SeedSkill();
        var resumeSeed = await db.SeedPersonResume();
        var sut = CreateSut(resumeSeed);
        sut.ResumeSkills = new[]
        {
            new ResumePersonSkillDto()
            {
                IsRelevant = false,
                SkillId = skillSeed.SkillId
            }
        };        

        var result = await Send(sut);

        result.AssertSuccess();
        GetDbContext().Set<ResumePersonSkill>().Count().Should().Be(1);
    }

    [Fact()]
    public async Task Send_CommandWithRelevantExistingResumePersonSkill_UpdatedResumePersonSkill()
    {
        var db = GetDbContext();
        var skillSeed = await db.SeedSkill();
        var resumeSeed = await db.SeedPersonResume();

        var sut = CreateSut(resumeSeed);
        
        sut.ResumeSkills = new[]
        {
            new ResumePersonSkillDto()
            {
                IsRelevant = true,
                IsHidden = true,
                SkillId = skillSeed.SkillId
            }
        };

        var result = await Send(sut);

        result.AssertSuccess();
        
        var rpSkill = GetDbContext()
            .Set<ResumePersonSkill>()
            .FirstOrDefault(s => s.SkillId == skillSeed.SkillId);

        rpSkill.Should().NotBeNull();
        rpSkill!.IsRelevant.Should().BeTrue();
        rpSkill.IsHidden.Should().BeTrue();
    }

    private MergeResumeSkillsCommand CreateSut(PersonResume seedResume)
    {
        var sut = CreateSut();
        sut.ResumeId = seedResume.ResumeId;
        return sut;
    }
    protected override MergeResumeSkillsCommand CreateSut()
    {
        return new MergeResumeSkillsCommand()
        {
            ResumeId = int.MaxValue
        };
    }
}
