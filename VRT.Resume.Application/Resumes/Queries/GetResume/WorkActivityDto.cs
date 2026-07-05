using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class WorkActivityDto: ISkillable
    {
        public required string Description { get; set; }
        public required SkillDto[] Skills { get; set; }
    }
}