using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DragoTactical.Models
{
    public partial class DragoTacticalDbContext : DbContext
    {
        public DragoTacticalDbContext()
        {
        }

        public DragoTacticalDbContext(DbContextOptions<DragoTacticalDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<FormSubmission> FormSubmissions { get; set; }
        public virtual DbSet<Service> Services { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use SQLite instead of SQL Server
                // This will create a local file named DragoTactical.db in your project folder
                optionsBuilder.UseSqlite("Data Source=DragoTactical.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === Category Table ===
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                      .HasName("PK_Category");

                entity.ToTable("Category");

                entity.Property(e => e.CategoryName)
                      .HasMaxLength(100);
            });

            // === FormSubmission Table ===
            modelBuilder.Entity<FormSubmission>(entity =>
            {
                entity.HasKey(e => e.SubmissionId)
                      .HasName("PK_FormSubmission");

                entity.ToTable("FormSubmission");

                entity.Property(e => e.CompanyName).HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Location).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.SubmissionDate)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // === Service Table ===
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.ServiceId)
                      .HasName("PK_Services");

                entity.ToTable("Services");

                entity.Property(e => e.ServiceName)
                      .HasMaxLength(255);

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Services)
                      .HasForeignKey(d => d.CategoryId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Services_Category");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
