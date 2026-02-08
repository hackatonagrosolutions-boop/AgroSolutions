using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertaService.Migrations
{
    /// <inheritdoc />
    public partial class alteracaoestruturabancodados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValorVento",
                table: "Alertas",
                newName: "Vento");

            migrationBuilder.RenameColumn(
                name: "ValorUmidade",
                table: "Alertas",
                newName: "UmidadeSolo");

            migrationBuilder.AddColumn<double>(
                name: "Temperatura",
                table: "Alertas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Temperatura",
                table: "Alertas");

            migrationBuilder.RenameColumn(
                name: "Vento",
                table: "Alertas",
                newName: "ValorVento");

            migrationBuilder.RenameColumn(
                name: "UmidadeSolo",
                table: "Alertas",
                newName: "ValorUmidade");
        }
    }
}
