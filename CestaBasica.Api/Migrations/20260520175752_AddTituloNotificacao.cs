using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CestaBasica.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTituloNotificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Notificacoes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Notificacoes");
        }
    }
}
