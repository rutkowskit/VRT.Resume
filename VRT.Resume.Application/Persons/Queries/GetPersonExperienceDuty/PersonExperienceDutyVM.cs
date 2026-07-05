namespace VRT.Resume.Application.Persons.Queries.GetPersonExperienceDuty
{
    public sealed class PersonExperienceDutyVM
    {        
        public int DutyId { get; internal set; }
        public int ExperienceId { get; internal set; }
        public required string Name { get; set; }
    }
}