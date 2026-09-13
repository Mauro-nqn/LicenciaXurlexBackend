using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenciaBackend.Migrations
{
    /// <inheritdoc />
    public partial class camposlicenciaregistrada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxDispositivos",
                table: "Licencias",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PermitirRemoto",
                table: "Licencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDispositivos",
                table: "Licencias");

            migrationBuilder.DropColumn(
                name: "PermitirRemoto",
                table: "Licencias");
        }
    }
}
