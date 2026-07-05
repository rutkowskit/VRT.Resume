using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Pwa.Components;
using VRT.Resume.Pwa.Features.Person;

namespace VRT.Resume.Pwa.Features.Resumes.Components;

public abstract class ResumeDocumentBase : CultureAwareComponentBase
{
    [Parameter, EditorRequired] public ResumeFullVM Model { get; set; } = null!;
    [Parameter] public string ProfileImageUrl { get; set; } = Person.ProfileImageUrl.DefaultImagePath;

    protected IEnumerable<WorkExperienceDto> OrderedExperience =>
        (Model.WorkExperience ?? [])
            .OrderByDescending(e => e.FromDate)
            .ThenByDescending(e => e.ToDate ?? DateTime.MaxValue);

    protected IEnumerable<EducationDto> OrderedEducation =>
        (Model.Education ?? [])
            .OrderByDescending(e => e.FromDate)
            .ThenByDescending(e => e.ToDate);
}