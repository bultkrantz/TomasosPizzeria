using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TomasosPizzeria.Migrations.Tomasos
{
    public partial class BonusSystemNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GratisPizza",
                table: "Kund");

            migrationBuilder.AddColumn<int>(
                name: "Bonus",
                table: "Kund",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bonus",
                table: "Kund");

            migrationBuilder.AddColumn<int>(
                name: "GratisPizza",
                table: "Kund",
                nullable: false,
                defaultValue: 0);
        }
    }
}
