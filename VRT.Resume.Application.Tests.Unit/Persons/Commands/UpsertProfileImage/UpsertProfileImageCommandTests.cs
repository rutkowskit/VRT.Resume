
using FluentAssertions;
using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.UpsertProfileImage;

public class UpsertProfileImageCommandTests : CommandTestBase<UpsertProfileImageCommand>
{
    public UpsertProfileImageCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    [Fact()]
    public async Task Send_CommandWithEmptyImageData_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.ImageData = Array.Empty<byte>();

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_WhenImageNotExistsIndDb_ShouldAddNewRecordToDbContext()
    {
        var sut = CreateSut();
        GetDbContext().PersonImage.FirstOrDefault().Should().BeNull();

        var result = await Send(sut);

        result.AssertSuccess();
        var entity = Assert.Single(GetDbContext().PersonImage);
        AssertPersonImage(entity, sut);

    }

    [Fact()]
    public async Task Send_CommandWithIdOfExistingSkill_ShouldUpdateExistingRecord()
    {
        var sut = CreateSut();
        var seedImg = await GetDbContext().SeedImage();

        var result = await Send(sut);

        var img = Assert.Single(GetDbContext().PersonImage);
        AssertPersonImage(img, sut);
        result.AssertSuccess();
    }

    private static void AssertPersonImage(PersonImage img, UpsertProfileImageCommand sut)
    {
        Assert.NotNull(img);        
        Assert.Equal(sut.ImageType, img.ImageType);
        Assert.Equal(sut.ImageData, img.ImageData);        
    }

    protected override UpsertProfileImageCommand CreateSut()
    {
        return new UpsertProfileImageCommand()
        {
            ImageData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0xD, 0xA, 0x1A, 0xA, 0x0, 0x0, 0x0, 0xD, 0x49, 0x48, 0x44, 0x52, 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x1, 0x8, 0x2, 0x0, 0x0, 0x0, 0x90, 0x77, 0x53, 0xDE, 0x0, 0x0, 0x0, 0x9, 0x70, 0x48, 0x59, 0x73, 0x0, 0x0, 0xE, 0xC4, 0x0, 0x0, 0xE, 0xC4, 0x1, 0x95, 0x2B, 0xE, 0x1B, 0x0, 0x0, 0x0, 0xC, 0x49, 0x44, 0x41, 0x54, 0x8, 0x99, 0x63, 0xF8, 0xCF, 0xA0, 0x6, 0x0, 0x3, 0x27, 0x1, 0x26, 0x2F, 0xC1, 0xAF, 0xE, 0x0, 0x0, 0x0, 0x0, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 },
            ImageType = "image/png"
        };
    }
}