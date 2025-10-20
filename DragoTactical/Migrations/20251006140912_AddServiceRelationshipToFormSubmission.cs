using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragoTactical.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceRelationshipToFormSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FormSubmission_ServiceId",
                table: "FormSubmission",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSubmission_Services",
                table: "FormSubmission",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSubmission_Services",
                table: "FormSubmission");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmission_ServiceId",
                table: "FormSubmission");
        }
    }
}
