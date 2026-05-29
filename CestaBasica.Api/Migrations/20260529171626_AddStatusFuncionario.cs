using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CestaBasica.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusFuncionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Retirado",
                table: "Funcionarios");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Funcionarios",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Funcionarios");

            migrationBuilder.AddColumn<bool>(
                name: "Retirado",
                table: "Funcionarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
