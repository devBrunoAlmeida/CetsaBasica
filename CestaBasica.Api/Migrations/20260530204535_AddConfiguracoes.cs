using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CestaBasica.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuracao_sistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WhatsAppHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    SmsHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    EnvioAutomaticoLembretes = table.Column<bool>(type: "boolean", nullable: false),
                    TempoSessaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    EncerrarPorInatividade = table.Column<bool>(type: "boolean", nullable: false),
                    FrequenciaBackup = table.Column<string>(type: "text", nullable: false),
                    PermitirCadastroOperadores = table.Column<bool>(type: "boolean", nullable: false),
                    BloqueioTentativasInvalidas = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracao_sistema", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracao_sistema");
        }
    }
}
