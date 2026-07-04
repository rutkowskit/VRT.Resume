using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Pwa.Features.Person;

namespace VRT.Resume.Pwa.Features.Resumes.Components;

public partial class ResumeDocument
{
    [Parameter, EditorRequired] public ResumeFullVM Model { get; set; } = null!;
    [Parameter] public string ProfileImageUrl { get; set; } = Person.ProfileImageUrl.DefaultImagePath;

    private IEnumerable<WorkExperienceDto> OrderedExperience =>
        (Model.WorkExperience ?? [])
            .OrderByDescending(e => e.FromDate)
            .ThenByDescending(e => e.ToDate ?? DateTime.MaxValue);

    private IEnumerable<EducationDto> OrderedEducation =>
        (Model.Education ?? [])
            .OrderByDescending(e => e.FromDate)
            .ThenByDescending(e => e.ToDate);
}
