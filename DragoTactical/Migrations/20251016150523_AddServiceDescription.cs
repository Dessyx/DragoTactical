using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragoTactical.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Services",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 1,
                column: "Description",
                value: "We identify your vulnerabilities before a threat does. Our experts conduct thorough assessments of your property and procedures to pinpoint risks and provide a clear, actionable plan to strengthen your defenses.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 2,
                column: "Description",
                value: "Professional, licensed and highly-trained officers whose presence acts as a deterrent and allows for immediate response to security incidents. Includes site security and specialized close-protection for high-profile individuals.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 3,
                column: "Description",
                value: "Design, installation, and maintenance of CCTV and video analytics solutions that enable continuous monitoring, evidence recording, real-time observation, and post-incident investigations.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 4,
                column: "Description",
                value: "Electronic keycards, biometrics, and tailored controls to secure entry points, manage visitor flow, and protect sensitive areas across your facilities.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 5,
                column: "Description",
                value: "Integrated intrusion detection that triggers immediate alerts for breaches, fires, or emergencies, with options for linkage to rapid response teams or local authorities.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 6,
                column: "Description",
                value: "Robust first-line defenses including fencing, bollards, gate systems, and patrols to protect property boundaries and vehicle assets from unauthorized access.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 7,
                column: "Description",
                value: "Strategic guidance from industry experts to analyze your unique challenges and develop comprehensive, long-term strategies that protect people, assets, and reputation.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 8,
                column: "Description",
                value: "End-to-end delivery of security installations—from concept to completion—ensuring on-time, on-budget projects seamlessly integrated with your operations.");

            // Cybersecurity services
            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 9,
                column: "Description",
                value: "Comprehensive identification of digital risks across networks, applications, and cloud assets with prioritized remediation plans to reduce exploitability.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 10,
                column: "Description",
                value: "Segmented architectures, next‑gen firewalls, secure remote access, and continuous monitoring to prevent, detect, and contain network-borne threats.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 11,
                column: "Description",
                value: "Encryption at rest and in transit, key management, data loss prevention, and governance controls to safeguard sensitive information throughout its lifecycle.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 12,
                column: "Description",
                value: "24/7 response playbooks, triage, containment, forensics, and recovery to minimize impact and accelerate safe restoration after cyber incidents.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 13,
                column: "Description",
                value: "Realistic adversary simulations that uncover exploitable weaknesses in web, infrastructure, and cloud, with actionable fixes mapped to risk.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 14,
                column: "Description",
                value: "Role-based training and phishing simulations to reduce human-factor risk and build a sustained security culture across the organization.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 15,
                column: "Description",
                value: "Secure cloud architectures, CSPM, IAM hardening, and workload protections to prevent misconfigurations and enforce least privilege in multi-cloud.");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "ServiceId",
                keyValue: 16,
                column: "Description",
                value: "Ongoing monitoring, threat detection, and executive‑level security leadership as-a-service to continuously improve posture and meet compliance.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Services");
        }
    }
}



