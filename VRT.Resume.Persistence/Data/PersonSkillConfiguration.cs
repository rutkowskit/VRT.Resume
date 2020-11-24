﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using VRT.Resume.Domain.Entities;



namespace VRT.Resume.Persistence.Data
{
    public class PersonSkillConfiguration : IEntityTypeConfiguration<PersonSkill>
    {
        public void Configure(EntityTypeBuilder<PersonSkill> entity)
        {
            entity.HasKey(e => e.SkillId)
                .HasName("PK_PersonSkill_1");

            entity.ToTable("PersonSkill", "Persons");

            entity.HasComment("Table contains full list of person skills");

            entity.Property(e => e.Level)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Knowledge level of the skill");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Default name of the skill");

            entity.Property(e => e.SkillTypeId).HasComment("Skill type id");

            entity.HasOne(d => d.Person)
                .WithMany(p => p.PersonSkill)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonSkill_Person");

            entity.HasOne(d => d.SkillType)
                .WithMany(p => p.PersonSkill)
                .HasForeignKey(d => d.SkillTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonSkill_SkillType");
        }
    }
}
