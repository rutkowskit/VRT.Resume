
namespace VRT.Resume.Application.Resumes.Queries.GetResumeSkillList
{
    public sealed class ResumeSkillListVM
    {
        public int ResumeId { get; internal set; }
        public ResumeSkillInListDto[] ResumeSkills { get; internal set; }
    }
}