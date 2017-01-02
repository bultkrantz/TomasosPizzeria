using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TomasosPizzeria.Migrations.Tomasos
{
    public partial class DeleteOnCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kund",
                columns: table => new
                {
                    KundID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnvandarNamn = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: true),
                    Gatuadress = table.Column<string>(type: "varchar(50)", nullable: false),
                    Losenord = table.Column<string>(type: "varchar(20)", nullable: false),
                    Namn = table.Column<string>(type: "varchar(100)", nullable: false),
                    Postnr = table.Column<string>(type: "varchar(20)", nullable: false),
                    Postort = table.Column<string>(type: "varchar(100)", nullable: false),
                    Telefon = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kund", x => x.KundID);
                });

            migrationBuilder.CreateTable(
                name: "MatrattTyp",
                columns: table => new
                {
                    MatrattTyp = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Beskrivning = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatrattTyp", x => x.MatrattTyp);
                });

            migrationBuilder.CreateTable(
                name: "Produkt",
                columns: table => new
                {
                    ProduktID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProduktNamn = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produkt", x => x.ProduktID);
                });

            migrationBuilder.CreateTable(
                name: "Bestallning",
                columns: table => new
                {
                    BestallningID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BestallningDatum = table.Column<DateTime>(type: "datetime", nullable: false),
                    KundID = table.Column<int>(nullable: false),
                    Levererad = table.Column<bool>(nullable: false),
                    Totalbelopp = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bestallning", x => x.BestallningID);
                    table.ForeignKey(
                        name: "FK_Bestallning_Kund",
                        column: x => x.KundID,
                        principalTable: "Kund",
                        principalColumn: "KundID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matratt",
                columns: table => new
                {
                    MatrattID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Beskrivning = table.Column<string>(type: "varchar(200)", nullable: true),
                    MatrattNamn = table.Column<string>(type: "varchar(50)", nullable: false),
                    MatrattTyp = table.Column<int>(nullable: false),
                    Pris = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matratt", x => x.MatrattID);
                    table.ForeignKey(
                        name: "FK_Matratt_MatrattTyp_MatrattTyp",
                        column: x => x.MatrattTyp,
                        principalTable: "MatrattTyp",
                        principalColumn: "MatrattTyp",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BestallningMatratt",
                columns: table => new
                {
                    MatrattID = table.Column<int>(nullable: false),
                    BestallningID = table.Column<int>(nullable: false),
                    Antal = table.Column<int>(nullable: false, defaultValueSql: "1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestallningMatratt", x => new { x.MatrattID, x.BestallningID });
                    table.ForeignKey(
                        name: "FK_BestallningMatratt_Bestallning",
                        column: x => x.BestallningID,
                        principalTable: "Bestallning",
                        principalColumn: "BestallningID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BestallningMatratt_Matratt",
                        column: x => x.MatrattID,
                        principalTable: "Matratt",
                        principalColumn: "MatrattID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatrattProdukt",
                columns: table => new
                {
                    MatrattID = table.Column<int>(nullable: false),
                    ProduktID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatrattProdukt", x => new { x.MatrattID, x.ProduktID });
                    table.ForeignKey(
                        name: "FK_MatrattProdukt_Matratt",
                        column: x => x.MatrattID,
                        principalTable: "Matratt",
                        principalColumn: "MatrattID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatrattProdukt_Produkt",
                        column: x => x.ProduktID,
                        principalTable: "Produkt",
                        principalColumn: "ProduktID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bestallning_KundID",
                table: "Bestallning",
                column: "KundID");

            migrationBuilder.CreateIndex(
                name: "IX_BestallningMatratt_BestallningID",
                table: "BestallningMatratt",
                column: "BestallningID");

            migrationBuilder.CreateIndex(
                name: "IX_BestallningMatratt_MatrattID",
                table: "BestallningMatratt",
                column: "MatrattID");

            migrationBuilder.CreateIndex(
                name: "IX_Matratt_MatrattTyp",
                table: "Matratt",
                column: "MatrattTyp");

            migrationBuilder.CreateIndex(
                name: "IX_MatrattProdukt_MatrattID",
                table: "MatrattProdukt",
                column: "MatrattID");

            migrationBuilder.CreateIndex(
                name: "IX_MatrattProdukt_ProduktID",
                table: "MatrattProdukt",
                column: "ProduktID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BestallningMatratt");

            migrationBuilder.DropTable(
                name: "MatrattProdukt");

            migrationBuilder.DropTable(
                name: "Bestallning");

            migrationBuilder.DropTable(
                name: "Matratt");

            migrationBuilder.DropTable(
                name: "Produkt");

            migrationBuilder.DropTable(
                name: "Kund");

            migrationBuilder.DropTable(
                name: "MatrattTyp");
        }
    }
}
