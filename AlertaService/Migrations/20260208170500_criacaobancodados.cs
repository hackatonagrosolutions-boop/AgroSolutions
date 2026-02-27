using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertaService.Migrations
{
    /// <inheritdoc />
    public partial class criacaobancodados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TalhaoId = table.Column<int>(type: "int", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorUmidade = table.Column<double>(type: "float", nullable: false),
                    ValorVento = table.Column<double>(type: "float", nullable: false),
                    DataAlerta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");
        }
    }
}
