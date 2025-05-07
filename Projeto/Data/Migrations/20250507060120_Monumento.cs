using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Data.Migrations
{
    /// <inheritdoc />
    public partial class Monumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caracteristicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    Estilo = table.Column<string>(type: "TEXT", nullable: false),
                    ClassPatrimonial = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caracteristicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localidade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeLocalidade = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    LocalidadeUtilizador = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Designacao = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", nullable: false),
                    Coordenadas = table.Column<string>(type: "TEXT", nullable: false),
                    EpocaConstrucao = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalidadeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monumento_Localidade_LocalidadeId",
                        column: x => x.LocalidadeId,
                        principalTable: "Localidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monumento_Utilizador_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaracteristicasMonumento",
                columns: table => new
                {
                    CaracteristicasId = table.Column<int>(type: "INTEGER", nullable: false),
                    MonumentosId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaracteristicasMonumento", x => new { x.CaracteristicasId, x.MonumentosId });
                    table.ForeignKey(
                        name: "FK_CaracteristicasMonumento_Caracteristicas_CaracteristicasId",
                        column: x => x.CaracteristicasId,
                        principalTable: "Caracteristicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaracteristicasMonumento_Monumento_MonumentosId",
                        column: x => x.MonumentosId,
                        principalTable: "Monumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Imagem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeImagem = table.Column<string>(type: "TEXT", nullable: false),
                    MonumentoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imagem_Monumento_MonumentoId",
                        column: x => x.MonumentoId,
                        principalTable: "Monumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComentarioTexto = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImagemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentario_Imagem_ImagemId",
                        column: x => x.ImagemId,
                        principalTable: "Imagem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentario_Utilizador_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaracteristicasMonumento_MonumentosId",
                table: "CaracteristicasMonumento",
                column: "MonumentosId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_ImagemId",
                table: "Comentario",
                column: "ImagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_UtilizadorId",
                table: "Comentario",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Imagem_MonumentoId",
                table: "Imagem",
                column: "MonumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Monumento_LocalidadeId",
                table: "Monumento",
                column: "LocalidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Monumento_UtilizadorId",
                table: "Monumento",
                column: "UtilizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaracteristicasMonumento");

            migrationBuilder.DropTable(
                name: "Comentario");

            migrationBuilder.DropTable(
                name: "Caracteristicas");

            migrationBuilder.DropTable(
                name: "Imagem");

            migrationBuilder.DropTable(
                name: "Monumento");

            migrationBuilder.DropTable(
                name: "Localidade");

            migrationBuilder.DropTable(
                name: "Utilizador");
        }
    }
}
