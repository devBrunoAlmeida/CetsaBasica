using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CestaBasica.Api.Migrations
{
    /// <inheritdoc />
    public partial class PadronizarNomesTabelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacoes_Funcionarios_FuncionarioId",
                table: "Notificacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Retiradas_Cestas_CestaId",
                table: "Retiradas");

            migrationBuilder.DropForeignKey(
                name: "FK_Retiradas_Funcionarios_FuncionarioId",
                table: "Retiradas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Retiradas",
                table: "Retiradas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notificacoes",
                table: "Notificacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Funcionarios",
                table: "Funcionarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cestas",
                table: "Cestas");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuarios");

            migrationBuilder.RenameTable(
                name: "Retiradas",
                newName: "retiradas");

            migrationBuilder.RenameTable(
                name: "Notificacoes",
                newName: "notificacoes");

            migrationBuilder.RenameTable(
                name: "Funcionarios",
                newName: "funcionarios");

            migrationBuilder.RenameTable(
                name: "Cestas",
                newName: "cestas");

            migrationBuilder.RenameIndex(
                name: "IX_Retiradas_FuncionarioId",
                table: "retiradas",
                newName: "IX_retiradas_FuncionarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Retiradas_CestaId",
                table: "retiradas",
                newName: "IX_retiradas_CestaId");

            migrationBuilder.RenameIndex(
                name: "IX_Notificacoes_FuncionarioId",
                table: "notificacoes",
                newName: "IX_notificacoes_FuncionarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_retiradas",
                table: "retiradas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notificacoes",
                table: "notificacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_funcionarios",
                table: "funcionarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cestas",
                table: "cestas",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "historico_importacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeArquivo = table.Column<string>(type: "text", nullable: false),
                    DataImportacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Usuario = table.Column<string>(type: "text", nullable: false),
                    QuantidadeRegistros = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_importacoes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_notificacoes_funcionarios_FuncionarioId",
                table: "notificacoes",
                column: "FuncionarioId",
                principalTable: "funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_retiradas_cestas_CestaId",
                table: "retiradas",
                column: "CestaId",
                principalTable: "cestas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_retiradas_funcionarios_FuncionarioId",
                table: "retiradas",
                column: "FuncionarioId",
                principalTable: "funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notificacoes_funcionarios_FuncionarioId",
                table: "notificacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_retiradas_cestas_CestaId",
                table: "retiradas");

            migrationBuilder.DropForeignKey(
                name: "FK_retiradas_funcionarios_FuncionarioId",
                table: "retiradas");

            migrationBuilder.DropTable(
                name: "historico_importacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_retiradas",
                table: "retiradas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notificacoes",
                table: "notificacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_funcionarios",
                table: "funcionarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cestas",
                table: "cestas");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "retiradas",
                newName: "Retiradas");

            migrationBuilder.RenameTable(
                name: "notificacoes",
                newName: "Notificacoes");

            migrationBuilder.RenameTable(
                name: "funcionarios",
                newName: "Funcionarios");

            migrationBuilder.RenameTable(
                name: "cestas",
                newName: "Cestas");

            migrationBuilder.RenameIndex(
                name: "IX_retiradas_FuncionarioId",
                table: "Retiradas",
                newName: "IX_Retiradas_FuncionarioId");

            migrationBuilder.RenameIndex(
                name: "IX_retiradas_CestaId",
                table: "Retiradas",
                newName: "IX_Retiradas_CestaId");

            migrationBuilder.RenameIndex(
                name: "IX_notificacoes_FuncionarioId",
                table: "Notificacoes",
                newName: "IX_Notificacoes_FuncionarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Retiradas",
                table: "Retiradas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notificacoes",
                table: "Notificacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Funcionarios",
                table: "Funcionarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cestas",
                table: "Cestas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacoes_Funcionarios_FuncionarioId",
                table: "Notificacoes",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Retiradas_Cestas_CestaId",
                table: "Retiradas",
                column: "CestaId",
                principalTable: "Cestas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Retiradas_Funcionarios_FuncionarioId",
                table: "Retiradas",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
