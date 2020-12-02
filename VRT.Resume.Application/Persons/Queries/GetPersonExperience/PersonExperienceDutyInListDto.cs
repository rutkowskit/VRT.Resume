using System.Collections.Generic;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{    
    public sealed class PersonExperienceDutyInListDto
    {
        public int DutyId { get; internal set; }
        public string Name { get; internal set; }
        public IEnumerable<string> Skills { get; internal set; }
    }
}
