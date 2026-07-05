using FluentAssertions;
using VRT.Resume.Application;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Domain.Common;
using VRT.Resume.Pwa.Features.Resumes.Components;
using VRT.Resume.Pwa.Tests.Fixtures;

namespace VRT.Resume.Pwa.Tests.Components;

public sealed class ResumeDocumentLinearTests
{
    [Fact]
    public void RendersSeededResumeFullVM()
    {
        using var ctx = new PwaTestContext();
        var model = CreateSampleResume();

        var cut = ctx.RenderWithMudProviders<ResumeDocumentLinear>(parameters =>
            parameters.Add(component => component.Model, model));

        cut.Markup.Should().Contain("resume-template-linear");
        cut.Markup.Should().Contain("Jan Kowalski");
        cut.Markup.Should().Contain("Senior Developer");
        cut.Markup.Should().Contain("jan@example.com");
        cut.Markup.Should().Contain("Acme Corp");
        cut.Markup.Should().Contain("C#");
        cut.Markup.Should().Contain("resume-skills-inline");
        cut.Markup.Should().Contain("resume-linear-contact-line");
    }

    private static ResumeFullVM CreateSampleResume() => new()
    {
        ResumeId = 1,
        PersonId = 1,
        FirstName = "Jan",
        LastName = "Kowalski",
        Position = "Senior Developer",
        Summary = "Experienced .NET developer.",
        ShowProfilePhoto = false,
        Contact =
        [
            new ContactItemDto
            {
                Name = "Email",
                Value = "jan@example.com",
                Type = ContactItemTypes.PlainText,
            },
        ],
        WorkExperience =
        [
            new WorkExperienceDto
            {
                Position = "Developer",
                CompanyName = "Acme Corp",
                Location = "Warsaw",
                FromDate = new DateTime(2020, 1, 1),
                ToDate = new DateTime(2024, 6, 1),
                WorkActivities =
                [
                    new WorkActivityDto
                    {
                        Description = "Built web APIs",
                        Skills = [],
                    },
                ],
            },
        ],
        Education =
        [
            new EducationDto
            {
                SchoolName = "Warsaw University",
                Degree = "MSc",
                Field = "Computer Science",
                FromDate = new DateTime(2015, 10, 1),
                ToDate = new DateTime(2020, 6, 30),
            },
        ],
        Skills =
        [
            new SkillDto
            {
                SkillId = 1,
                Name = "C#",
                Type = SkillTypes.Technical,
                IsRelevant = true,
                Position = 1,
            },
            new SkillDto
            {
                SkillId = 2,
                Name = "English",
                Type = SkillTypes.HumanLanguage,
                Level = "C1",
                Position = 1,
            },
        ],
    };
}