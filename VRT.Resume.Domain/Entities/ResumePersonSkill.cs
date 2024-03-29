﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace VRT.Resume.Domain.Entities;

/// <summary>
/// Person skill for resume
/// </summary>
public partial class ResumePersonSkill
{
    /// <summary>
    /// Skill id from Person Skills
    /// </summary>
    public int SkillId { get; set; }

    public int ResumeId { get; set; }

    /// <summary>
    /// The flag indicates wheter the skill is significant for potencial employer
    /// </summary>
    public bool IsRelevant { get; set; }

    /// <summary>
    /// The flag indicates wheter the skill should be hidden in profile skills section (main)
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Position on the skills list (skills with higher values goes first - they are more important)
    /// </summary>
    public int Position { get; set; }

    public virtual PersonResume Resume { get; set; }

    public virtual PersonSkill Skill { get; set; }
}