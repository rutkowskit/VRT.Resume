﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

#nullable disable

namespace VRT.Resume.Persistence.Data.Configurations
{
    public class PersonExperienceConfiguration : IEntityTypeConfiguration<PersonExperience>
    {
        public void Configure(EntityTypeBuilder<PersonExperience> entity)
        {
            entity.HasKey(e => e.ExperienceId);

            entity.ToTable("PersonExperience", "Persons");

            entity.HasComment("Contains information about person's work experience");

            entity.HasIndex(e => e.PersonId, "IX_PersonExperience_PersonId");

            entity.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.FromDate).HasColumnType("date");

            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ModifyDate).HasColumnType("datetime");

            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ToDate).HasColumnType("date");

            entity.HasOne(d => d.Person)
                .WithMany(p => p.PersonExperience)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonExperience_Person");
        }
    }
}
