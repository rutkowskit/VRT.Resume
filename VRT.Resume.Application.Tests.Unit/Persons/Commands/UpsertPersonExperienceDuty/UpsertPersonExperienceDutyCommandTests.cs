
using FluentAssertions;
using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty;

public class UpsertPersonExperienceDutyCommandTests : CommandTestBase<UpsertPersonExperienceDutyCommand>
{
    public UpsertPersonExperienceDutyCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    [Fact()]
    public async Task Send_CommandWithoutExperienceId_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.ExperienceId = 0;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithTooShortDutyName_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.Name = "";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithExperienceThatDoesNotExists_ShouldFail()
    {
        var sut = CreateSut();
        sut.ExperienceId = 63533535;

        var result = await Send(sut);

        result.AssertFail();
        Assert.Equal(Errors.PersonExperienceNotExists, result.Error);
    }

    [Fact()]
    public async Task Send_CommandWithWrongDutyId_ShouldAddNewRecordToDbContext()
    {
        var db = GetDbContext();
        var expSeed = await db.SeedExperience(seedDuty: false);
        var sut = CreateSut(expSeed);
        sut.ExperienceId = expSeed.ExperienceId;        
        db.PersonExperienceDuty.FirstOrDefault().Should().BeNull();

        var result = await Send(sut);

        result.AssertSuccess();
        db = GetDbContext();
        var duty = Assert.Single(db.PersonExperienceDuty);
        AssertPersonExperienceDuty(duty, sut);
    }

    [Fact()]
    public async Task Send_CommandWithIdOfExistingExpDuty_ShouldUpdateExistingRecord()
    {
        var expSeed = await GetDbContext().SeedExperience();
        var duty = expSeed.PersonExperienceDuty.First();
        var sut = CreateSut(expSeed);
        sut.DutyId = duty.DutyId;

        var result = await Send(sut);

        result.AssertSuccess();
        duty = Assert.Single(GetDbContext().PersonExperienceDuty);
        AssertPersonExperienceDuty(duty, sut);
        Assert.Equal(sut.DutyId, duty.DutyId);
    }

    private static void AssertPersonExperienceDuty(PersonExperienceDuty exp,
           UpsertPersonExperienceDutyCommand sut)
    {
        Assert.NotNull(exp);        
        Assert.Equal(sut.Name, exp.Name);
    }

    private UpsertPersonExperienceDutyCommand CreateSut(PersonExperience experience)
    {
        var result = CreateSut();
        result.ExperienceId = experience.ExperienceId;
        return result;
    }
    protected override UpsertPersonExperienceDutyCommand CreateSut()
    {
        return new UpsertPersonExperienceDutyCommand()
        {
            ExperienceId = int.MaxValue,
            Name = "Creating Web Services"
        };
    }
}