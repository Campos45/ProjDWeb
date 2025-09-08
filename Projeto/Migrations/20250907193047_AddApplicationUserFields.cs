using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilizador_AspNetUsers_IdentityUserId",
                table: "Utilizador");

            migrationBuilder.DropIndex(
                name: "IX_VisitaMonumento_MonumentoId",
                table: "VisitaMonumento");

            migrationBuilder.DropIndex(
                name: "IX_Utilizador_IdentityUserId",
                table: "Utilizador");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Utilizador",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "LocalidadeUtilizador",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaMonumento_MonumentoId_UtilizadorId",
                table: "VisitaMonumento",
                columns: new[] { "MonumentoId", "UtilizadorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitaMonumento_MonumentoId_UtilizadorId",
                table: "VisitaMonumento");

            migrationBuilder.DropColumn(
                name: "LocalidadeUtilizador",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Utilizador",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitaMonumento_MonumentoId",
                table: "VisitaMonumento",
                column: "MonumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizador_IdentityUserId",
                table: "Utilizador",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizador_AspNetUsers_IdentityUserId",
                table: "Utilizador",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
