using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SAC_Web_Application.Data.ClubMigrations
{
    public partial class UpdatedModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QualName",
                table: "Qualifications",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Coaches",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Coaches",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "Coaches",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QualName",
                table: "Qualifications",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Coaches",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Coaches",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "Coaches",
                nullable: true);
        }
    }
}
