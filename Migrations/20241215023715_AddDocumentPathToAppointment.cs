using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentDoctor.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentPathToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_medicalHistories_AspNetUsers_UserId",
                table: "medicalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_medicalHistories",
                table: "medicalHistories");

            migrationBuilder.RenameTable(
                name: "medicalHistories",
                newName: "MedicalHistories");

            migrationBuilder.RenameIndex(
                name: "IX_medicalHistories_UserId",
                table: "MedicalHistories",
                newName: "IX_MedicalHistories_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfEntry",
                table: "MedicalHistories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories",
                column: "MedicalHistoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistories_AspNetUsers_UserId",
                table: "MedicalHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistories_AspNetUsers_UserId",
                table: "MedicalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "MedicalHistories",
                newName: "medicalHistories");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalHistories_UserId",
                table: "medicalHistories",
                newName: "IX_medicalHistories_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfEntry",
                table: "medicalHistories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_medicalHistories",
                table: "medicalHistories",
                column: "MedicalHistoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_medicalHistories_AspNetUsers_UserId",
                table: "medicalHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
