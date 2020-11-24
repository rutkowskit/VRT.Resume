﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace VRT.Resume.Domain.Entities
{
    /// <summary>
    /// Person resume data
    /// </summary>
    public partial class PersonResume
    {
        public PersonResume()
        {
            ResumeContact = new HashSet<ResumeContact>();
            ResumePersonSkill = new HashSet<ResumePersonSkill>();
        }

        public int ResumeId { get; set; }
        /// <summary>
        /// Internal person id
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// Job position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// Profile summary text
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// Flag indicates if profile photo should be displayed on resume
        /// </summary>
        public bool? ShowProfilePhoto { get; set; }
        /// <summary>
        /// Permission to process personal data by the receiver of the resume.
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// Last modification date and time
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        public virtual Person Person { get; set; }
        public virtual ICollection<ResumeContact> ResumeContact { get; set; }
        public virtual ICollection<ResumePersonSkill> ResumePersonSkill { get; set; }
    }
}