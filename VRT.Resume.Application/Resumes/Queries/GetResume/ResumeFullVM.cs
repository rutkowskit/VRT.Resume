namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class ResumeFullVM : ISkillable
    {
        public ResumeFullVM()
        {
            ShowProfilePhoto = true;
            Contact = [];
            WorkExperience = [];
            Education = [];
            Skills = [];
        }
        public int ResumeId { get; set; }
        public int PersonId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Position { get; set; }
        public string? Summary { get; set; }
        public required ContactItemDto[] Contact { get; set; }
        public required WorkExperienceDto[] WorkExperience { get; set; }
        public required EducationDto[] Education { get; set; }
        public required SkillDto[] Skills { get; set; }
        public string? Permission { get; set; }
        public bool ShowProfilePhoto { get; set; }
    }
}