using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integrador.Migrations
{
    /// <inheritdoc />
    public partial class AddDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodDescuento",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "Pedido");

            migrationBuilder.AddColumn<int>(
                name: "DescuentoId",
                table: "Pedido",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Descuento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descuento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_DescuentoId",
                table: "Pedido",
                column: "DescuentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Descuento_DescuentoId",
                table: "Pedido",
                column: "DescuentoId",
                principalTable: "Descuento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Descuento_DescuentoId",
                table: "Pedido");

            migrationBuilder.DropTable(
                name: "Descuento");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_DescuentoId",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "DescuentoId",
                table: "Pedido");

            migrationBuilder.AddColumn<string>(
                name: "CodDescuento",
                table: "Pedido",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "Pedido",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
