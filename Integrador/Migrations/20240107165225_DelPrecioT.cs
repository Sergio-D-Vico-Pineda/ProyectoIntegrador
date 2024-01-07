using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integrador.Migrations
{
    /// <inheritdoc />
    public partial class DelPrecioT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioTotal",
                table: "DetallePedido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioTotal",
                table: "DetallePedido",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
