using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integrador.Migrations
{
    /// <inheritdoc />
    public partial class AddDesc2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Porcentaje",
                table: "Descuento",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Porcentaje",
                table: "Descuento",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
