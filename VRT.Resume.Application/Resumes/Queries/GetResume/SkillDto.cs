namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class SkillDto
    {
        public SkillTypes Type { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public bool IsRelevant { get; set; }
        public bool IsHidden { get; set; }
    }
}