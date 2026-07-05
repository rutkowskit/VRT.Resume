using System;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{
    public sealed class PersonExperienceVM
    {
        public int ExperienceId { get; internal set; }
        public required string Position { get; set; }
        public required string CompanyName { get; set; }
        public string? Location { get; internal set; }
        public DateTime FromDate { get; internal set; }
        public DateTime? ToDate { get; internal set; }        
    }
}