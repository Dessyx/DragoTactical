using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DragoTactical.Services;

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

                // Configure value converters for field-level encryption of PII
                var encryptOptional = new ValueConverter<string?, string?>(
                    v => FieldEncryption.EncryptString(v),
                    v => FieldEncryption.DecryptString(v)
                );
                var encryptRequired = new ValueConverter<string, string>(
                    v => FieldEncryption.EncryptString(v)!,
                    v => FieldEncryption.DecryptString(v)!
                );

                entity.Property(e => e.CompanyName).HasMaxLength(255).HasConversion(encryptOptional);
                entity.Property(e => e.Email).HasMaxLength(255).HasConversion(encryptRequired);
                entity.Property(e => e.FirstName).HasMaxLength(100).HasConversion(encryptRequired);
                entity.Property(e => e.LastName).HasMaxLength(100).HasConversion(encryptRequired);
                entity.Property(e => e.Location).HasMaxLength(255).HasConversion(encryptOptional);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50).HasConversion(encryptOptional);
                entity.Property(e => e.Message).HasConversion(encryptOptional);
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

                entity.Property(e => e.Title)
                      .HasMaxLength(255);

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
                // Physical Services (CategoryId = 1) — Titles aligned with view card titles
                new Service { ServiceId = 1, CategoryId = 1, ServiceName = "Risk Analysis & Security Audits", Title = "Risk Analysis & Security Audits", Description = "We identify your vulnerabilities before a threat does. Our experts conduct thorough assessments of your property and procedures to pinpoint risks and provide a clear, actionable plan to strengthen your defenses." },
                new Service { ServiceId = 2, CategoryId = 1, ServiceName = "On-Site Security Personnel / VIP Protection", Title = "On-Site Security Personnel / VIP Protection", Description = "Professional, licensed and highly-trained officers whose presence acts as a deterrent and allows for immediate response to security incidents. Includes site security and specialized close-protection for high-profile individuals." },
                new Service { ServiceId = 3, CategoryId = 1, ServiceName = "Surveillance Systems", Title = "Surveillance Systems", Description = "Design, installation, and maintenance of CCTV and video analytics solutions that enable continuous monitoring, evidence recording, real-time observation, and post-incident investigations." },
                new Service { ServiceId = 4, CategoryId = 1, ServiceName = "Access Control Solutions", Title = "Access Control Solutions", Description = "Electronic keycards, biometrics, and tailored controls to secure entry points, manage visitor flow, and protect sensitive areas across your facilities." },
                new Service { ServiceId = 5, CategoryId = 1, ServiceName = "Alarm & Emergency Response Systems", Title = "Alarm and Emergency Response Systems", Description = "Integrated intrusion detection that triggers immediate alerts for breaches, fires, or emergencies, with options for linkage to rapid response teams or local authorities." },
                new Service { ServiceId = 6, CategoryId = 1, ServiceName = "Vehicle & Perimeter Security", Title = "Vehicle & Perimeter Security", Description = "Robust first-line defenses including fencing, bollards, gate systems, and patrols to protect property boundaries and vehicle assets from unauthorized access." },
                new Service { ServiceId = 7, CategoryId = 1, ServiceName = "Security Consulting", Title = "Security Consulting", Description = "Strategic guidance from industry experts to analyze your unique challenges and develop comprehensive, long-term strategies that protect people, assets, and reputation." },
                new Service { ServiceId = 8, CategoryId = 1, ServiceName = "Project Management", Title = "Project Management", Description = "End-to-end delivery of security installations—from concept to completion—ensuring on-time, on-budget projects seamlessly integrated with your operations." },

                // Cybersecurity Services (CategoryId = 2) — Use ServiceName as Title
                new Service { ServiceId = 9, CategoryId = 2, ServiceName = "Risk Assessment and Vulnerability Testing", Title = "Risk Assessment and Vulnerability Testing", Description = "Comprehensive identification of digital risks across networks, applications, and cloud assets with prioritized remediation plans to reduce exploitability." },
                new Service { ServiceId = 10, CategoryId = 2, ServiceName = "Network Security", Title = "Network Security", Description = "Segmented architectures, next‑gen firewalls, secure remote access, and continuous monitoring to prevent, detect, and contain network-borne threats." },
                new Service { ServiceId = 11, CategoryId = 2, ServiceName = "Data Protection & Encryption", Title = "Data Protection & Encryption", Description = "Encryption at rest and in transit, key management, data loss prevention, and governance controls to safeguard sensitive information throughout its lifecycle." },
                new Service { ServiceId = 12, CategoryId = 2, ServiceName = "Incident Response and Threat Mitigation", Title = "Incident Response and Threat Mitigation", Description = "24/7 response playbooks, triage, containment, forensics, and recovery to minimize impact and accelerate safe restoration after cyber incidents." },
                new Service { ServiceId = 13, CategoryId = 2, ServiceName = "Penetration Testing & Ethical Hacking", Title = "Penetration Testing & Ethical Hacking", Description = "Realistic adversary simulations that uncover exploitable weaknesses in web, infrastructure, and cloud, with actionable fixes mapped to risk." },
                new Service { ServiceId = 14, CategoryId = 2, ServiceName = "Employee Training and Awareness", Title = "Employee Training and Awareness", Description = "Role-based training and phishing simulations to reduce human-factor risk and build a sustained security culture across the organization." },
                new Service { ServiceId = 15, CategoryId = 2, ServiceName = "Cloud Security", Title = "Cloud Security", Description = "Secure cloud architectures, CSPM, IAM hardening, and workload protections to prevent misconfigurations and enforce least privilege in multi-cloud." },
                new Service { ServiceId = 16, CategoryId = 2, ServiceName = "Managed Security Services (MSSP) - Virtual Cyber Assistant / VCISO", Title = "Managed Security Services (MSSP) - Virtual Cyber Assistant / VCISO", Description = "Ongoing monitoring, threat detection, and executive‑level security leadership as-a-service to continuously improve posture and meet compliance." }
            );


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
