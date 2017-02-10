using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SAC_Web_Application.Data.ClubMigrations
{
    public partial class Update2MemberModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TeamName",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Province",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Address1",
                table: "Member",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TeamName",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Province",
                table: "Member",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address1",
                table: "Member",
                nullable: false);
        }
    }
}
