using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenciaBackend.Migrations
{
    /// <inheritdoc />
    public partial class CampoActiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activa",
                table: "Licencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Clave",
                table: "Licencias",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DispositivoId",
                table: "Licencias",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Info_Activa",
                table: "Licencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsuariosLocales",
                table: "Licencias",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidoHasta",
                table: "Licencias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activa",
                table: "Licencias");

            migrationBuilder.DropColumn(
                name: "Clave",
                table: "Licencias");

            migrationBuilder.DropColumn(
                name: "DispositivoId",
                table: "Licencias");

            migrationBuilder.DropColumn(
                name: "Info_Activa",
                table: "Licencias");

            migrationBuilder.DropColumn(
                name: "MaxUsuariosLocales",
                table: "Licencias");

            migrationBuilder.DropColumn(
                name: "ValidoHasta",
                table: "Licencias");
        }
    }
}
