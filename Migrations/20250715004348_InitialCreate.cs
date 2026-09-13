using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LicenciaBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Licencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Info_EstudioNombre = table.Column<string>(type: "text", nullable: false),
                    Info_CUIT = table.Column<string>(type: "text", nullable: false),
                    Info_Clave = table.Column<string>(type: "text", nullable: false),
                    Info_DispositivoId = table.Column<string>(type: "text", nullable: false),
                    Info_Modo = table.Column<string>(type: "text", nullable: false),
                    Info_ValidoDesde = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Info_ValidoHasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Info_ApiUrl = table.Column<string>(type: "text", nullable: true),
                    Info_MaxUsuariosLocales = table.Column<int>(type: "integer", nullable: false),
                    Info_MaxUsuariosRemotos = table.Column<int>(type: "integer", nullable: false),
                    Info_PermitirRemoto = table.Column<bool>(type: "boolean", nullable: false),
                    Info_MaxDispositivos = table.Column<int>(type: "integer", nullable: false),
                    Info_Notas = table.Column<string>(type: "text", nullable: true),
                    Firma_DatosBase64 = table.Column<string>(type: "text", nullable: false),
                    Firma_FirmaBase64 = table.Column<string>(type: "text", nullable: false),
                    IpOrigen = table.Column<string>(type: "text", nullable: true),
                    Origen = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licencias", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Licencias");
        }
    }
}
