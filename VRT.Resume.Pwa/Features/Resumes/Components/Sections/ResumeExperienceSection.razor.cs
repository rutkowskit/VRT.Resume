using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Pwa.Features.Resumes.Components.Sections;

public partial class ResumeExperienceSection
{
    [Parameter] public IEnumerable<WorkExperienceDto> Experience { get; set; } = [];
}