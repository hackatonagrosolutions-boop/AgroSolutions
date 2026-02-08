using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropriedadeService.Migrations
{
    /// <inheritdoc />
    public partial class alteraestruturabancodados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Talhao_Propriedades_PropriedadeId",
                table: "Talhao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Talhao",
                table: "Talhao");

            migrationBuilder.RenameTable(
                name: "Talhao",
                newName: "Talhoes");

            migrationBuilder.RenameIndex(
                name: "IX_Talhao_PropriedadeId",
                table: "Talhoes",
                newName: "IX_Talhoes_PropriedadeId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Talhoes",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Normal",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50)
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<int>(
                name: "PropriedadeId",
                table: "Talhoes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Talhoes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "Cultura",
                table: "Talhoes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "AreaHectares",
                table: "Talhoes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)")
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Talhoes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Talhoes",
                table: "Talhoes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Talhoes_Propriedades_PropriedadeId",
                table: "Talhoes",
                column: "PropriedadeId",
                principalTable: "Propriedades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Talhoes_Propriedades_PropriedadeId",
                table: "Talhoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Talhoes",
                table: "Talhoes");

            migrationBuilder.RenameTable(
                name: "Talhoes",
                newName: "Talhao");

            migrationBuilder.RenameIndex(
                name: "IX_Talhoes_PropriedadeId",
                table: "Talhao",
                newName: "IX_Talhao_PropriedadeId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Talhao",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Normal")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<int>(
                name: "PropriedadeId",
                table: "Talhao",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Talhao",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "Cultura",
                table: "Talhao",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "AreaHectares",
                table: "Talhao",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Talhao",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Talhao",
                table: "Talhao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Talhao_Propriedades_PropriedadeId",
                table: "Talhao",
                column: "PropriedadeId",
                principalTable: "Propriedades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
