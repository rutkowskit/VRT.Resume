
using FluentAssertions;
using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonContact;

public class UpsertPersonContactCommandTests : CommandTestBase<UpsertPersonContactCommand>
{
    public UpsertPersonContactCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    [Fact()]
    public async Task Send_CommandWithoutName_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.Name = null!;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }
    [Fact()]
    public async Task Send_CommandWithoutValue_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.Value = "";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithRequiredParametersWrongId_ShouldAddNewRecordToDbContext()
    {
        var sut = CreateSut();

        var result = await Send(sut);

        result.AssertSuccess();

        var resume = Assert.Single(GetDbContext().PersonContact);
        Assert.Equal(sut.Name, resume.Name);
        Assert.Equal(sut.Value, resume.Value);        
    }

    [Fact()]
    public async Task Send_CommandWithProvidedIdOfExistingResume_ShouldUpdateExistingRecord()
    {        
        var existingContact = await GetDbContext().SeedContact();
        var sut = CreateSut();
        sut.ContactId = existingContact.ContactId;

        var result = await Send(sut);

        result.AssertSuccess();
        var resume = Assert.Single(GetDbContext().PersonContact);
        Assert.Equal(sut.Url, resume.Url);
        Assert.Equal(sut.Value, resume.Value);        
        Assert.Equal(sut.Name, resume.Name);
        Assert.Equal(sut.Icon, resume.Icon);
        Assert.Equal(sut.ContactId, resume.ContactId);        
    }

    [Fact()]
    public async Task Send_CommandWithUnsupportedIcon_ShouldThrowValidationException()
    {
        var sut = CreateSut();
        sut.Icon = "<script>doSomethingNasty();</script>";
        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithCorrectImgIcon_ShouldSetImage()
    {
        const string ExpectedImg = "<img src=\"test\">";
        var sut = CreateSut();
        sut.Icon = ExpectedImg;

        var result = await Send(sut);

        result.AssertSuccess();
        
        var resume = Assert.Single(GetDbContext().PersonContact);
        Assert.Equal(ExpectedImg, resume.Icon);
            
    }

    [Fact]
    public async Task Send_CommandWithCorrectSvgIcon_ShouldSetImage()
    {
        const string ExpectedImg = "<svg>Test</svg>";
        var sut = CreateSut();
        sut.Icon = ExpectedImg;

        var result = await Send(sut);

        result.AssertSuccess();
        
        var resume = Assert.Single(GetDbContext().PersonContact);
        Assert.Equal(ExpectedImg, resume.Icon);
        
    }

    protected override UpsertPersonContactCommand CreateSut()
    {
        return new UpsertPersonContactCommand()
        {            
            Name = "Name",
            Value = "Value",
            Url = "",
            Icon = null
        };
    }
}