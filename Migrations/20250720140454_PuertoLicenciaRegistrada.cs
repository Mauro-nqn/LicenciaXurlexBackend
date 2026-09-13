using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenciaBackend.Migrations
{
    /// <inheritdoc />
    public partial class PuertoLicenciaRegistrada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Puerto",
                table: "Licencias",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Puerto",
                table: "Licencias");
        }
    }
}
