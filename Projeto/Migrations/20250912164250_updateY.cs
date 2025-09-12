using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class updateY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Localidade_Utilizador_UtilizadorId",
                table: "Localidade");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorId",
                table: "Localidade",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Localidade_Utilizador_UtilizadorId",
                table: "Localidade",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Localidade_Utilizador_UtilizadorId",
                table: "Localidade");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorId",
                table: "Localidade",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Localidade_Utilizador_UtilizadorId",
                table: "Localidade",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
