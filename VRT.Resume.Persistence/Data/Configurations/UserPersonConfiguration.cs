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
    public class UserPersonConfiguration : IEntityTypeConfiguration<UserPerson>
    {
        public void Configure(EntityTypeBuilder<UserPerson> entity)
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserPerson", "Auth");

            entity.HasComment("Connects person with extelnal user identifiers (from different sources)");

            entity.HasIndex(e => e.PersonId, "IX_UserPerson_PersonId");

            entity.Property(e => e.UserId).HasMaxLength(50);

            entity.HasOne(d => d.Person)
                .WithMany(p => p.UserPerson)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPerson_Person");
        }
    }
}
