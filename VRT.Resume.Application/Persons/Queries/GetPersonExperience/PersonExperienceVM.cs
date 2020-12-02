using System;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{
    public sealed class PersonExperienceVM
    {
        public int ExperienceId { get; internal set; }
        public string Position { get; internal set; }
        public string CompanyName { get; internal set; }
        public string Location { get; internal set; }
        public DateTime FromDate { get; internal set; }
        public DateTime? ToDate { get; internal set; }        
    }
}
