namespace VRT.Resume.Application.Resumes.Commands.MergeResumeSkills
{
    public sealed class ResumePersonSkillDto
    {
        public int SkillId { get; set; }
        public bool IsRelevent { get; set; }
        public bool IsHidden { get; set; }
        public int Position { get; set; }
    }
}
