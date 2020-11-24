using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class WorkActivityDto: ISkillable
    {
        public string Description { get; set; }
        public SkillDto[] Skills { get; set; }
    }
}