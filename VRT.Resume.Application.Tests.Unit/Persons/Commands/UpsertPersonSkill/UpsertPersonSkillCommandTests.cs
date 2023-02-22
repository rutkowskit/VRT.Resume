using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Common;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonSkill;

public class UpsertPersonSkillCommandTests : CommandTestBase<UpsertPersonSkillCommand>
{
    public UpsertPersonSkillCommandTests(ApplicationFixture fixture) : base(fixture)
    {
    }

    [Fact()]
    public async Task Send_CommandWithTooShortSkillName_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.SkillName = "";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithTooShortSkillLevel_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.SkillLevel = "";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithWrongId_ShouldAddNewRecordToDbContext()
    {
        var sut = CreateSut();
        var entity = GetDbContext().PersonSkill.FirstOrDefault();
        Assert.Null(entity);

        var result = await Send(sut);
        result.AssertSuccess();

        entity = Assert.Single(GetDbContext().PersonSkill);
        AssertPersonSkill(entity, sut);        
    }

    [Fact()]
    public async Task Send_CommandWithIdOfExistingSkill_ShouldUpdateExistingRecord()
    {
        var skillSeed = await GetDbContext().SeedSkill();
        var sut = CreateSut();
        sut.SkillId = skillSeed.SkillId;

        var result = await Send(sut);
        
        result.AssertSuccess();
        var exp = Assert.Single(GetDbContext().PersonSkill);
        AssertPersonSkill(exp, sut);        
    }

    private static void AssertPersonSkill(PersonSkill skill,
           UpsertPersonSkillCommand sut)
    {
        Assert.NotNull(skill);        
        Assert.Equal(sut.SkillLevel, skill.Level);
        Assert.Equal(sut.SkillName, skill.Name);
        Assert.Equal(sut.SkillType, (SkillTypes)skill.SkillTypeId);        
    }

    protected override UpsertPersonSkillCommand CreateSut()
    {
        return new UpsertPersonSkillCommand()
        {
            SkillId = int.MaxValue,
            SkillLevel = "0",
            SkillName = "screwdriver",
            SkillType = SkillTypes.Tool
        };
    }
}