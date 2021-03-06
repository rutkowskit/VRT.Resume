﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace VRT.Resume.Domain.Entities
{
    /// <summary>
    /// Persons table
    /// </summary>
    public partial class Person
    {
        public Person()
        {
            PersonContact = new HashSet<PersonContact>();
            PersonEducation = new HashSet<PersonEducation>();
            PersonExperience = new HashSet<PersonExperience>();
            PersonImage = new HashSet<PersonImage>();
            PersonResume = new HashSet<PersonResume>();
            PersonSkill = new HashSet<PersonSkill>();
            UserPerson = new HashSet<UserPerson>();
        }

        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<PersonContact> PersonContact { get; set; }
        public virtual ICollection<PersonEducation> PersonEducation { get; set; }
        public virtual ICollection<PersonExperience> PersonExperience { get; set; }
        public virtual ICollection<PersonImage> PersonImage { get; set; }
        public virtual ICollection<PersonResume> PersonResume { get; set; }
        public virtual ICollection<PersonSkill> PersonSkill { get; set; }
        public virtual ICollection<UserPerson> UserPerson { get; set; }
    }
}