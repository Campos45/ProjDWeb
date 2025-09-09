using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
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

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Utilizador");

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

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Utilizador",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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
