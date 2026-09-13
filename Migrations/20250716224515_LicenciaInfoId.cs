using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenciaBackend.Migrations
{
    /// <inheritdoc />
    public partial class LicenciaInfoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Info_Id",
                table: "Licencias",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Info_Id",
                table: "Licencias");
        }
    }
}
