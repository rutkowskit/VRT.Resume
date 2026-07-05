
namespace VRT.Resume.Application.Resumes.Queries.GetResumeSkillList
{
    public sealed class ResumeSkillListVM
    {
        public int ResumeId { get; internal set; }
        public required ResumeSkillInListDto[] ResumeSkills { get; set; }
    }
}