using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Anemette.Web.Data.Migreringer
{
    /// <inheritdoc />
    public partial class Start : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Administratorer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Brugernavn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    AdgangskodeHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    GammelHash = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    GammelSalt = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Oprettet = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SidstLoggetInd = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administratorer", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Arrangementer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Titel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Beskrivelse = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    Start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Sted = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    Pris = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Pladser = table.Column<int>(type: "int", nullable: true),
                    Synlig = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arrangementer", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Artikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Titel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "varchar(220)", maxLength: 220, nullable: false),
                    Indledning = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true),
                    Indhold = table.Column<string>(type: "longtext", nullable: false),
                    Billede = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    Udgivet = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Synlig = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artikler", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Boeger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Titel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "varchar(220)", maxLength: 220, nullable: false),
                    Undertitel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Beskrivelse = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    Pris = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Lager = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Emne = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Udgivelsesaar = table.Column<int>(type: "int", nullable: true),
                    Sider = table.Column<int>(type: "int", nullable: true),
                    Isbn = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Forside = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    Fremhaevet = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Oprettet = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boeger", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FriendlyName = table.Column<string>(type: "longtext", nullable: true),
                    Xml = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Indstillinger",
                columns: table => new
                {
                    Noegle = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    Vaerdi = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indstillinger", x => x.Noegle);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kontaktbeskeder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Oprettet = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Laest = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Navn = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Telefon = table.Column<string>(type: "longtext", nullable: true),
                    Emne = table.Column<string>(type: "longtext", nullable: false),
                    Besked = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kontaktbeskeder", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ordrer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Oprettet = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Sendt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Anonymiseret = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Navn = table.Column<string>(type: "longtext", nullable: false),
                    Adresse = table.Column<string>(type: "longtext", nullable: false),
                    Postnummer = table.Column<string>(type: "longtext", nullable: false),
                    By = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Telefon = table.Column<string>(type: "longtext", nullable: false),
                    Besked = table.Column<string>(type: "longtext", nullable: true),
                    Note = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordrer", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sider",
                columns: table => new
                {
                    Noegle = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    Titel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Indhold = table.Column<string>(type: "longtext", nullable: false),
                    Opdateret = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sider", x => x.Noegle);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tilmeldinger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ArrangementId = table.Column<int>(type: "int", nullable: false),
                    Oprettet = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Antal = table.Column<int>(type: "int", nullable: false),
                    Navn = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Telefon = table.Column<string>(type: "longtext", nullable: false),
                    Besked = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tilmeldinger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tilmeldinger_Arrangementer_ArrangementId",
                        column: x => x.ArrangementId,
                        principalTable: "Arrangementer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ordrelinjer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    OrdreId = table.Column<int>(type: "int", nullable: false),
                    BogId = table.Column<int>(type: "int", nullable: true),
                    Titel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Antal = table.Column<int>(type: "int", nullable: false),
                    Stykpris = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordrelinjer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordrelinjer_Boeger_BogId",
                        column: x => x.BogId,
                        principalTable: "Boeger",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ordrelinjer_Ordrer_OrdreId",
                        column: x => x.OrdreId,
                        principalTable: "Ordrer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Administratorer_Brugernavn",
                table: "Administratorer",
                column: "Brugernavn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artikler_Slug",
                table: "Artikler",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boeger_Slug",
                table: "Boeger",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ordrelinjer_BogId",
                table: "Ordrelinjer",
                column: "BogId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordrelinjer_OrdreId",
                table: "Ordrelinjer",
                column: "OrdreId");

            migrationBuilder.CreateIndex(
                name: "IX_Tilmeldinger_ArrangementId",
                table: "Tilmeldinger",
                column: "ArrangementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administratorer");

            migrationBuilder.DropTable(
                name: "Artikler");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "Indstillinger");

            migrationBuilder.DropTable(
                name: "Kontaktbeskeder");

            migrationBuilder.DropTable(
                name: "Ordrelinjer");

            migrationBuilder.DropTable(
                name: "Sider");

            migrationBuilder.DropTable(
                name: "Tilmeldinger");

            migrationBuilder.DropTable(
                name: "Boeger");

            migrationBuilder.DropTable(
                name: "Ordrer");

            migrationBuilder.DropTable(
                name: "Arrangementer");
        }
    }
}
