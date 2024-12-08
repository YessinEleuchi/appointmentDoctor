using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentDoctor.Migrations
{
    /// <inheritdoc />
    public partial class adjustmedicalHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medicalHistories",
                columns: table => new
                {
                    MedicalHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateOfEntry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicalCondition = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Medications = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Surgeries = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FamilyMedicalHistory = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicalHistories", x => x.MedicalHistoryID);
                    table.ForeignKey(
                        name: "FK_medicalHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medicalHistories_UserId",
                table: "medicalHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medicalHistories");
        }
    }
}
