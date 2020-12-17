﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using VRT.Resume.Domain.Entities;



namespace VRT.Resume.Persistence.Data
{
    public class PersonResumeConfiguration : IEntityTypeConfiguration<PersonResume>
    {
        public void Configure(EntityTypeBuilder<PersonResume> entity)
        {
            entity.HasKey(e => e.ResumeId);

            entity.ToTable("PersonResume", "Resumes");

            entity.HasComment("Person resume data");

            entity.HasIndex(e => e.PersonId);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("(N'Resume')")
                .HasComment("Description of the resume (e.g. .net develper to asseco)");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasComment("Last modification date and time");

            entity.Property(e => e.Permission).HasComment("Permission to process personal data by the receiver of the resume.");

            entity.Property(e => e.PersonId).HasComment("Internal person id");

            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Job position");

            entity.Property(e => e.ShowProfilePhoto)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasComment("Flag indicates if profile photo should be displayed on resume");

            entity.Property(e => e.Summary).HasComment("Profile summary text");

            entity.HasOne(d => d.Person)
                .WithMany(p => p.PersonResume)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonResume_Person");
        }
    }
}
