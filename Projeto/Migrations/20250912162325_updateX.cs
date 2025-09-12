using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class updateX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UtilizadorId",
                table: "Localidade",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Localidade_UtilizadorId",
                table: "Localidade",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Localidade_Utilizador_UtilizadorId",
                table: "Localidade",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Localidade_Utilizador_UtilizadorId",
                table: "Localidade");

            migrationBuilder.DropIndex(
                name: "IX_Localidade_UtilizadorId",
                table: "Localidade");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "Localidade");
        }
    }
}
