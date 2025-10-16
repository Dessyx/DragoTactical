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
            // Connection string is configured in Program.cs via dependency injection
            // No need to configure here when using DI
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

                entity.HasOne(d => d.Service)
                      .WithMany()
                      .HasForeignKey(d => d.ServiceId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_FormSubmission_Services");
            });

            // === Service Table ===
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.ServiceId)
                      .HasName("PK_Services");

                entity.ToTable("Services");

                entity.Property(e => e.ServiceName)
                      .HasMaxLength(255);

                entity.Property(e => e.Description)
                      .HasMaxLength(1000);

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Services)
                      .HasForeignKey(d => d.CategoryId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Services_Category");
            });


            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Physical Service" },
                new Category { CategoryId = 2, CategoryName = "Cybersecurity Service" }
            );

            // Physical Services (CategoryId = 1)
            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceId = 1, CategoryId = 1, ServiceName = "Risk Analysis & Security Audits" },
                new Service { ServiceId = 2, CategoryId = 1, ServiceName = "On-Site Security Personnel / VIP Protection" },
                new Service { ServiceId = 3, CategoryId = 1, ServiceName = "Surveillance Systems" },
                new Service { ServiceId = 4, CategoryId = 1, ServiceName = "Access Control Solutions" },
                new Service { ServiceId = 5, CategoryId = 1, ServiceName = "Alarm & Emergency Response Systems" },
                new Service { ServiceId = 6, CategoryId = 1, ServiceName = "Vehicle & Perimeter Security" },
                new Service { ServiceId = 7, CategoryId = 1, ServiceName = "Security Consulting" },
                new Service { ServiceId = 8, CategoryId = 1, ServiceName = "Project Management" },

                // Cybersecurity Services (CategoryId = 2)
                new Service { ServiceId = 9, CategoryId = 2, ServiceName = "Risk Assessment and Vulnerability Testing" },
                new Service { ServiceId = 10, CategoryId = 2, ServiceName = "Network Security" },
                new Service { ServiceId = 11, CategoryId = 2, ServiceName = "Data Protection & Encryption" },
                new Service { ServiceId = 12, CategoryId = 2, ServiceName = "Incident Response and Threat Mitigation" },
                new Service { ServiceId = 13, CategoryId = 2, ServiceName = "Penetration Testing & Ethical Hacking" },
                new Service { ServiceId = 14, CategoryId = 2, ServiceName = "Employee Training and Awareness" },
                new Service { ServiceId = 15, CategoryId = 2, ServiceName = "Cloud Security" },
                new Service { ServiceId = 16, CategoryId = 2, ServiceName = "Managed Security Services (MSSP) - Virtual Cyber Assistant / VCISO" }
            );


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
