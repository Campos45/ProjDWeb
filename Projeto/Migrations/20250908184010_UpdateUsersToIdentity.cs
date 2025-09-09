using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsersToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Utilizador_UtilizadorId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Imagem_Utilizador_UtilizadorId",
                table: "Imagem");

            migrationBuilder.DropForeignKey(
                name: "FK_Monumento_Utilizador_UtilizadorId",
                table: "Monumento");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizador_AspNetUsers_IdentityUserId",
                table: "Utilizador");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitaMonumento_Utilizador_UtilizadorId",
                table: "VisitaMonumento");

            migrationBuilder.DropIndex(
                name: "IX_VisitaMonumento_MonumentoId",
                table: "VisitaMonumento");

            migrationBuilder.DropIndex(
                name: "IX_Utilizador_IdentityUserId",
                table: "Utilizador");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Utilizador");

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadorId",
                table: "VisitaMonumento",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "UtilizadorId1",
                table: "VisitaMonumento",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadorId",
                table: "Monumento",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "UtilizadorId1",
                table: "Monumento",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadorId",
                table: "Imagem",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadorId",
                table: "Comentario",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "UtilizadorId1",
                table: "Comentario",
                type: "INTEGER",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_VisitaMonumento_UtilizadorId1",
                table: "VisitaMonumento",
                column: "UtilizadorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Monumento_UtilizadorId1",
                table: "Monumento",
                column: "UtilizadorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_UtilizadorId1",
                table: "Comentario",
                column: "UtilizadorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_AspNetUsers_UtilizadorId",
                table: "Comentario",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Utilizador_UtilizadorId1",
                table: "Comentario",
                column: "UtilizadorId1",
                principalTable: "Utilizador",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Imagem_AspNetUsers_UtilizadorId",
                table: "Imagem",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Monumento_AspNetUsers_UtilizadorId",
                table: "Monumento",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Monumento_Utilizador_UtilizadorId1",
                table: "Monumento",
                column: "UtilizadorId1",
                principalTable: "Utilizador",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaMonumento_AspNetUsers_UtilizadorId",
                table: "VisitaMonumento",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaMonumento_Utilizador_UtilizadorId1",
                table: "VisitaMonumento",
                column: "UtilizadorId1",
                principalTable: "Utilizador",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_AspNetUsers_UtilizadorId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Utilizador_UtilizadorId1",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Imagem_AspNetUsers_UtilizadorId",
                table: "Imagem");

            migrationBuilder.DropForeignKey(
                name: "FK_Monumento_AspNetUsers_UtilizadorId",
                table: "Monumento");

            migrationBuilder.DropForeignKey(
                name: "FK_Monumento_Utilizador_UtilizadorId1",
                table: "Monumento");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitaMonumento_AspNetUsers_UtilizadorId",
                table: "VisitaMonumento");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitaMonumento_Utilizador_UtilizadorId1",
                table: "VisitaMonumento");

            migrationBuilder.DropIndex(
                name: "IX_VisitaMonumento_MonumentoId_UtilizadorId",
                table: "VisitaMonumento");

            migrationBuilder.DropIndex(
                name: "IX_VisitaMonumento_UtilizadorId1",
                table: "VisitaMonumento");

            migrationBuilder.DropIndex(
                name: "IX_Monumento_UtilizadorId1",
                table: "Monumento");

            migrationBuilder.DropIndex(
                name: "IX_Comentario_UtilizadorId1",
                table: "Comentario");

            migrationBuilder.DropColumn(
                name: "UtilizadorId1",
                table: "VisitaMonumento");

            migrationBuilder.DropColumn(
                name: "UtilizadorId1",
                table: "Monumento");

            migrationBuilder.DropColumn(
                name: "UtilizadorId1",
                table: "Comentario");

            migrationBuilder.DropColumn(
                name: "LocalidadeUtilizador",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorId",
                table: "VisitaMonumento",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Utilizador",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorId",
                table: "Monumento",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorId",
                table: "Imagem",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorId",
                table: "Comentario",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaMonumento_MonumentoId",
                table: "VisitaMonumento",
                column: "MonumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizador_IdentityUserId",
                table: "Utilizador",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Utilizador_UtilizadorId",
                table: "Comentario",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Imagem_Utilizador_UtilizadorId",
                table: "Imagem",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Monumento_Utilizador_UtilizadorId",
                table: "Monumento",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizador_AspNetUsers_IdentityUserId",
                table: "Utilizador",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaMonumento_Utilizador_UtilizadorId",
                table: "VisitaMonumento",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
