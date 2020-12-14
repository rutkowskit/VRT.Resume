
namespace VRT.Resume.Application.Resumes.Queries.GetResumeSkillList
{
    public sealed class ResumeSkillInListDto
    {
        public int SkillId { get; internal set; }
        public bool IsRelevant { get; internal set; }
        public bool IsHidden { get; internal set; }
        public string Name { get; internal set; }
        public string Type { get; internal set; }
        public int Position { get; internal set; }
    }
}