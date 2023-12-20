using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integrador.Migrations
{
    /// <inheritdoc />
    public partial class fixRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Suministro_ProductoId",
                table: "Suministro",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Suministro_ProveedorId",
                table: "Suministro",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Suministro_Producto_ProductoId",
                table: "Suministro",
                column: "ProductoId",
                principalTable: "Producto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suministro_Proveedor_ProveedorId",
                table: "Suministro",
                column: "ProveedorId",
                principalTable: "Proveedor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suministro_Producto_ProductoId",
                table: "Suministro");

            migrationBuilder.DropForeignKey(
                name: "FK_Suministro_Proveedor_ProveedorId",
                table: "Suministro");

            migrationBuilder.DropIndex(
                name: "IX_Suministro_ProductoId",
                table: "Suministro");

            migrationBuilder.DropIndex(
                name: "IX_Suministro_ProveedorId",
                table: "Suministro");
        }
    }
}
