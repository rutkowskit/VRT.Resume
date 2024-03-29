﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Persistence.Data.Configurations
{
    public partial class PersonEducationConfiguration : IEntityTypeConfiguration<PersonEducation>
    {
        public void Configure(EntityTypeBuilder<PersonEducation> entity)
        {
            entity.HasKey(e => e.EducationId).HasName("PK_PersonEducation_1");

            entity.ToTable("PersonEducation", "Persons", tb => tb.HasComment("Persons education entries"));

            entity.HasIndex(e => e.DegreeId, "IX_PersonEducation_DegreeId");

            entity.HasIndex(e => e.EducationFieldId, "IX_PersonEducation_EducationFieldId");

            entity.HasIndex(e => e.PersonId, "IX_PersonEducation_PersonId");

            entity.HasIndex(e => e.SchoolId, "IX_PersonEducation_SchoolId");

            entity.Property(e => e.FromDate).HasColumnType("date");
            entity.Property(e => e.Grade).HasMaxLength(20);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Specialization).HasMaxLength(255);
            entity.Property(e => e.ToDate).HasColumnType("date");

            entity.HasOne(d => d.Degree).WithMany(p => p.PersonEducation)
            .HasForeignKey(d => d.DegreeId)
            .HasConstraintName("FK_PersonEducation_Degree");

            entity.HasOne(d => d.EducationField).WithMany(p => p.PersonEducation)
            .HasForeignKey(d => d.EducationFieldId)
            .HasConstraintName("FK_PersonEducation_EducationField");

            entity.HasOne(d => d.Person).WithMany(p => p.PersonEducation)
            .HasForeignKey(d => d.PersonId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonEducation_Person");

            entity.HasOne(d => d.School).WithMany(p => p.PersonEducation)
            .HasForeignKey(d => d.SchoolId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonEducation_School");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<PersonEducation> entity);
    }
}
