using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragoTactical.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Services",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 1,
                columns: new[] { "Description", "Title" },
                values: new object[] { "We identify your vulnerabilities before a threat does. Our experts conduct thorough assessments of your property and procedures to pinpoint risks and provide a clear, actionable plan to strengthen your defenses.", "Risk Analysis & Security Audits" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 2,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Professional, licensed and highly-trained officers whose presence acts as a deterrent and allows for immediate response to security incidents. Includes site security and specialized close-protection for high-profile individuals.", "On-Site Security Personnel / VIP Protection" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 3,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Design, installation, and maintenance of CCTV and video analytics solutions that enable continuous monitoring, evidence recording, real-time observation, and post-incident investigations.", "Surveillance Systems" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 4,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Electronic keycards, biometrics, and tailored controls to secure entry points, manage visitor flow, and protect sensitive areas across your facilities.", "Access Control Solutions" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 5,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Integrated intrusion detection that triggers immediate alerts for breaches, fires, or emergencies, with options for linkage to rapid response teams or local authorities.", "Alarm and Emergency Response Systems" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 6,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Robust first-line defenses including fencing, bollards, gate systems, and patrols to protect property boundaries and vehicle assets from unauthorized access.", "Vehicle & Perimeter Security" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 7,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Strategic guidance from industry experts to analyze your unique challenges and develop comprehensive, long-term strategies that protect people, assets, and reputation.", "Security Consulting" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 8,
                columns: new[] { "Description", "Title" },
                values: new object[] { "End-to-end delivery of security installations—from concept to completion—ensuring on-time, on-budget projects seamlessly integrated with your operations.", "Project Management" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 9,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Comprehensive identification of digital risks across networks, applications, and cloud assets with prioritized remediation plans to reduce exploitability.", "Risk Assessment and Vulnerability Testing" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 10,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Segmented architectures, next‑gen firewalls, secure remote access, and continuous monitoring to prevent, detect, and contain network-borne threats.", "Network Security" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 11,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Encryption at rest and in transit, key management, data loss prevention, and governance controls to safeguard sensitive information throughout its lifecycle.", "Data Protection & Encryption" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 12,
                columns: new[] { "Description", "Title" },
                values: new object[] { "24/7 response playbooks, triage, containment, forensics, and recovery to minimize impact and accelerate safe restoration after cyber incidents.", "Incident Response and Threat Mitigation" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 13,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Realistic adversary simulations that uncover exploitable weaknesses in web, infrastructure, and cloud, with actionable fixes mapped to risk.", "Penetration Testing & Ethical Hacking" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 14,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Role-based training and phishing simulations to reduce human-factor risk and build a sustained security culture across the organization.", "Employee Training and Awareness" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 15,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Secure cloud architectures, CSPM, IAM hardening, and workload protections to prevent misconfigurations and enforce least privilege in multi-cloud.", "Cloud Security" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 16,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Ongoing monitoring, threat detection, and executive‑level security leadership as-a-service to continuously improve posture and meet compliance.", "Managed Security Services (MSSP) - Virtual Cyber Assistant / VCISO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Services");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 1,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 2,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 3,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 4,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 5,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 6,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 7,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 8,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 9,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 10,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 11,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 12,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 13,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 14,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 15,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 16,
                column: "Description",
                value: null);
        }
    }
}
