using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TomasosPizzeria.Migrations.Tomasos
{
    public partial class DeleteOnCascadeBestallningMatratt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BestallningMatratt_Bestallning",
                table: "BestallningMatratt");

            migrationBuilder.AddForeignKey(
                name: "FK_BestallningMatratt_Bestallning",
                table: "BestallningMatratt",
                column: "BestallningID",
                principalTable: "Bestallning",
                principalColumn: "BestallningID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BestallningMatratt_Bestallning",
                table: "BestallningMatratt");

            migrationBuilder.AddForeignKey(
                name: "FK_BestallningMatratt_Bestallning",
                table: "BestallningMatratt",
                column: "BestallningID",
                principalTable: "Bestallning",
                principalColumn: "BestallningID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
