using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenciaBackend.Migrations
{
    /// <inheritdoc />
    public partial class PuertoInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Licencias"" ALTER COLUMN ""Puerto"" TYPE integer USING ""Puerto""::integer;");
            migrationBuilder.Sql(@"UPDATE ""Licencias"" SET ""Puerto"" = 0 WHERE ""Puerto"" IS NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""Licencias"" ALTER COLUMN ""Puerto"" SET NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""Licencias"" ALTER COLUMN ""Puerto"" SET DEFAULT 0;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Puerto",
                table: "Licencias",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
