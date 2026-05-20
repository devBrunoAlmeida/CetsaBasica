using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CestaBasica.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDataEnvioNotificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnviadoEm",
                table: "Notificacoes",
                newName: "DataEnvio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataEnvio",
                table: "Notificacoes",
                newName: "EnviadoEm");
        }
    }
}
