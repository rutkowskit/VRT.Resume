﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace VRT.Resume.Domain.Entities
{
    /// <summary>
    /// Dictionary table with available degrees
    /// </summary>
    public partial class Degree
    {
        public Degree()
        {
            PersonEducation = new HashSet<PersonEducation>();
        }

        public int DegreeId { get; set; }
        public string Name { get; set; }
        public string Abreviation { get; set; }

        public virtual ICollection<PersonEducation> PersonEducation { get; set; }
    }
}