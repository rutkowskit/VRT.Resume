﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace VRT.Resume.Domain.Entities
{
    /// <summary>
    /// Connects person with extelnal user identifiers (from different sources)
    /// </summary>
    public partial class UserPerson
    {
        public string UserId { get; set; }
        public int PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}