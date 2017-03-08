using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SAC_Web_Application.Data.ClubMigrations
{
    public partial class EventsValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Events",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "EventTitle",
                table: "Events",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Events",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventTitle",
                table: "Events",
                nullable: true);
        }
    }
}
