using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Resumes.Commands.UpsertPersonResume;

public class UpsertPersonResumeCommandTests : CommandTestBase<UpsertPersonResumeCommand>
{
    public UpsertPersonResumeCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    [Fact()]
    public async Task Send_CommandWithoutDescription_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.Description = null!;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }
    [Fact()]
    public async Task Send_CommandWithoutPosition_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.Position = null!;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithRequiredParametersWrongId_ShouldAddNewRecordToDbContext()
    {
        var sut = CreateSut();

        var result = await Send(sut);

        result.AssertSuccess();

        var resume = Assert.Single(GetDbContext().PersonResume);
        Assert.Equal(sut.Description, resume.Description);
        Assert.Equal(sut.Position, resume.Position);        
    }

    [Fact()]
    public async Task Send_CommandWithProvidedIdOfExistingResume_ShouldUpdateExistingRecord()
    {        
        var existingReusme = await GetDbContext().SeedPersonResume();
        var sut = CreateSut(existingReusme);

        var result = await Send(sut);

        result.AssertSuccess();

        var resume = Assert.Single(GetDbContext().PersonResume);
        Assert.Equal(sut.Description, resume.Description);
        Assert.Equal(sut.Position, resume.Position);
        Assert.Equal(existingReusme.PersonId, resume.PersonId);
        Assert.Equal(sut.ResumeId, resume.ResumeId);
        Assert.Equal(sut.Summary, resume.Summary);
        Assert.Equal(sut.DataProcessingPermission, resume.Permission);
        Assert.Equal(sut.ShowProfilePhoto, resume.ShowProfilePhoto);
    }

    private UpsertPersonResumeCommand CreateSut(PersonResume seededEntity)
    {
        var result = CreateSut();
        result.ResumeId = seededEntity.ResumeId;
        return result;
    }
    protected override UpsertPersonResumeCommand CreateSut()
    {
        return new UpsertPersonResumeCommand()
        {
            ResumeId = int.MaxValue,
            Description = "Description",
            Position = "Position",
            DataProcessingPermission = "DataProcessingPermission",
            ShowProfilePhoto = true,
            Summary = "Summary"
        };
    }
}