using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertaService.Migrations
{
    /// <inheritdoc />
    public partial class inclusaotipoalertabancodados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoAlerta",
                table: "Alertas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoAlerta",
                table: "Alertas");
        }
    }
}
