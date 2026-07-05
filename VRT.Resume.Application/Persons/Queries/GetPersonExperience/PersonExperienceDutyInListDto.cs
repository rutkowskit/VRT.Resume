using System.Collections.Generic;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{    
    public sealed class PersonExperienceDutyInListDto
    {
        public int DutyId { get; internal set; }
        public required string Name { get; set; }
        public required IEnumerable<string> Skills { get; set; }
    }
}