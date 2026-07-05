using System;

namespace VRT.Resume.Application.Persons.Queries.GetPersonData
{
    public sealed class PersonDataVM
    {
        public int PersonId { get; set; }        
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
    }
}