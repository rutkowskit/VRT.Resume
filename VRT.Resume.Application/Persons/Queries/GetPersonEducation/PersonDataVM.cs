using System;

namespace VRT.Resume.Application.Persons.Queries.GetPersonEducation
{
    public sealed class PersonDataVM
    {
        public int PersonId { get; set; }        
        public string FirstName { get; set; }        
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
