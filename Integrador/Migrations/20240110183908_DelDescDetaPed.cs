using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integrador.Migrations
{
    /// <inheritdoc />
    public partial class DelDescDetaPed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "DetallePedido");

            migrationBuilder.AddColumn<string>(
                name: "Descuento",
                table: "Pedido",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "Pedido");

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "DetallePedido",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
