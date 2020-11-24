namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class ResumeVM : ISkillable
    {
        public ResumeVM()
        {
            ShowProfilePhoto = true;
        }
        public int ResumeId { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Summary { get; set; }
        public ContactItemDto[] Contact { get; set; }
        public WorkExperienceDto[] WorkExperience { get; set; }
        public EducationDto[] Education { get; set; }
        public SkillDto[] Skills { get; set; }
        public string Permission { get; set; }
        public bool ShowProfilePhoto { get; set; }
    }
}