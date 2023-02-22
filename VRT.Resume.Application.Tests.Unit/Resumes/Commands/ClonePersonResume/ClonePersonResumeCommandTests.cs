using FluentAssertions;
using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Resumes.Commands.ClonePersonResume;

public class ClonePersonResumeCommandTests : CommandTestBase<ClonePersonResumeCommand>
{
    public ClonePersonResumeCommandTests(ApplicationFixture fixture) : base(fixture)
    {
    }

    [Fact()]
    public async Task Send_CommandWithoutResumeId_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.ResumeId = 0;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithResumeIdThatDoesNotExists_ShouldFail()
    {
        var sut = CreateSut();
        sut.ResumeId = 43434343;

        var result = await Send(sut);

        result.AssertFail();
        Assert.Equal(Errors.RecordNotFound, result.Error);
    }

    [Fact()]
    public async Task Send_CommandWithProvidedIdOfExistingResume_ShouldCloneResume()
    {
        var sut = CreateSut();
        await GetDbContext().SeedPersonResume();

        var result = await Send(sut);

        result.AssertSuccess();

        var db = GetDbContext();
        var clonedResume = db.PersonResume.FirstOrDefault(r => r.ResumeId != sut.ResumeId);
        clonedResume.Should().NotBeNull();
        var baseResume = db.PersonResume.FirstOrDefault(r => r.ResumeId == sut.ResumeId);
        baseResume.Should().NotBeNull();
        AssertClonedResume(baseResume!, clonedResume!);
    }

    private static void AssertClonedResume(PersonResume template, PersonResume clone)
    {
        var t = template;
        var c = clone;
        Assert.Equal(t.Description, c.Description);
        Assert.Equal(t.Permission, c.Permission);
        Assert.Equal(t.PersonId, c.PersonId);
        Assert.Equal(t.Position, c.Position);
        Assert.Equal(t.ShowProfilePhoto, c.ShowProfilePhoto);
        Assert.Equal(t.Summary, c.Summary);
        Assert.Equal(t.ResumePersonSkill.Count, c.ResumePersonSkill.Count);

        foreach (var s in t.ResumePersonSkill)
        {
            var cSkill = c.ResumePersonSkill
                   .FirstOrDefault(i => i.SkillId == s.SkillId);
            Assert.NotNull(cSkill);
            Assert.Equal(s.IsHidden, cSkill.IsHidden);
            Assert.Equal(s.IsRelevant, cSkill.IsRelevant);
            Assert.Equal(s.Position, cSkill.Position);
        }
    }

    protected override ClonePersonResumeCommand CreateSut()
    {
        return new ClonePersonResumeCommand()
        {
            ResumeId = 1
        };
    }
}