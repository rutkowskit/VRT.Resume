using System;
using System.Collections.Generic;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{
    public sealed class PersonExperienceInListVM : ITimeRange
    {
        public int ExperienceId { get; internal set; }
        public required string Position { get; set; }
        public required string CompanyName { get; set; }
        public string? Location { get; internal set; }
        public DateTime FromDate { get; internal set; }
        public DateTime? ToDate { get; internal set; }
        public required IEnumerable<PersonExperienceDutyInListDto> Duties { get; set; }
    }
}