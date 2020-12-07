namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class SkillDto
    {
        public int SkillId { get; set; }
        public SkillTypes Type { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public bool IsRelevent { get; set; }
        public bool IsHidden { get; set; }
        public int Position { get; set; }
    }
}