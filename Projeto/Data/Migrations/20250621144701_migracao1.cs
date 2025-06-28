using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Data.Migrations
{
    /// <inheritdoc />
    public partial class migracao1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrincipal",
                table: "Imagem",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UtilizadorId",
                table: "Imagem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Imagem_UtilizadorId",
                table: "Imagem",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imagem_Utilizador_UtilizadorId",
                table: "Imagem",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imagem_Utilizador_UtilizadorId",
                table: "Imagem");

            migrationBuilder.DropIndex(
                name: "IX_Imagem_UtilizadorId",
                table: "Imagem");

            migrationBuilder.DropColumn(
                name: "IsPrincipal",
                table: "Imagem");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "Imagem");
        }
    }
}
