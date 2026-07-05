using VRT.Resume.Application;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Domain.Common;

namespace VRT.Resume.Pwa.Tests.Fixtures;

internal static class ResumeTestData
{
    public static ResumeFullVM CreateSampleResume() => new()
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