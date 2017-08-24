using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TomasosPizzeria.Migrations.Tomasos
{
    public partial class AddedIdentityToTOmasos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Kund",
                type: "varchar(50)",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Kund",
                type: "varchar(50)",
                nullable: true);
        }
    }
}
