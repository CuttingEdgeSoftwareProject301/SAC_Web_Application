using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SAC_Web_Application.Data.ClubMigrations
{
    public partial class UpdateMemberModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "County",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Member",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "County",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Member",
                nullable: false);
        }
    }
}
