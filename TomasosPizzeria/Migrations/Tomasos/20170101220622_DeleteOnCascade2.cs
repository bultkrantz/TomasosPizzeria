using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TomasosPizzeria.Migrations.Tomasos
{
    public partial class DeleteOnCascade2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matratt_MatrattTyp_MatrattTyp",
                table: "Matratt");

            migrationBuilder.DropForeignKey(
                name: "FK_MatrattProdukt_Matratt",
                table: "MatrattProdukt");

            migrationBuilder.AddForeignKey(
                name: "FK_Matratt_MatrattTyp_MatrattTyp",
                table: "Matratt",
                column: "MatrattTyp",
                principalTable: "MatrattTyp",
                principalColumn: "MatrattTyp",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatrattProdukt_Matratt",
                table: "MatrattProdukt",
                column: "MatrattID",
                principalTable: "Matratt",
                principalColumn: "MatrattID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matratt_MatrattTyp_MatrattTyp",
                table: "Matratt");

            migrationBuilder.DropForeignKey(
                name: "FK_MatrattProdukt_Matratt",
                table: "MatrattProdukt");

            migrationBuilder.AddForeignKey(
                name: "FK_Matratt_MatrattTyp_MatrattTyp",
                table: "Matratt",
                column: "MatrattTyp",
                principalTable: "MatrattTyp",
                principalColumn: "MatrattTyp",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatrattProdukt_Matratt",
                table: "MatrattProdukt",
                column: "MatrattID",
                principalTable: "Matratt",
                principalColumn: "MatrattID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
