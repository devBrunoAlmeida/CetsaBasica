using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CestaBasica.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEvolutionConfiguracoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EvolutionApiKey",
                table: "configuracao_sistema",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EvolutionBaseUrl",
                table: "configuracao_sistema",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EvolutionInstance",
                table: "configuracao_sistema",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EvolutionTelefone",
                table: "configuracao_sistema",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvolutionApiKey",
                table: "configuracao_sistema");

            migrationBuilder.DropColumn(
                name: "EvolutionBaseUrl",
                table: "configuracao_sistema");

            migrationBuilder.DropColumn(
                name: "EvolutionInstance",
                table: "configuracao_sistema");

            migrationBuilder.DropColumn(
                name: "EvolutionTelefone",
                table: "configuracao_sistema");
        }
    }
}
