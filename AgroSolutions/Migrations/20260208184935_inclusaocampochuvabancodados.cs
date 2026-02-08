using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertaService.Migrations
{
    /// <inheritdoc />
    public partial class inclusaocampochuvabancodados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Chuva",
                table: "Alertas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chuva",
                table: "Alertas");
        }
    }
}
