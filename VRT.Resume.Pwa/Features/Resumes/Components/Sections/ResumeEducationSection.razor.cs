using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Pwa.Features.Resumes.Components.Sections;

public partial class ResumeEducationSection
{
    [Parameter] public IEnumerable<EducationDto> Education { get; set; } = [];
}