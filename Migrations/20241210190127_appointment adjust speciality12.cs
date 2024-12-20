﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentDoctor.Migrations
{
    /// <inheritdoc />
    public partial class appointmentadjustspeciality12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Specialities_SpecialtyId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SpecialtyId",
                table: "AspNetUsers",
                newName: "SpecialityId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_SpecialtyId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_SpecialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Specialities_SpecialityId",
                table: "AspNetUsers",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Specialities_SpecialityId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SpecialityId",
                table: "AspNetUsers",
                newName: "SpecialtyId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_SpecialityId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Specialities_SpecialtyId",
                table: "AspNetUsers",
                column: "SpecialtyId",
                principalTable: "Specialities",
                principalColumn: "Id");
        }
    }
}
