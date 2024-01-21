using Integrador.Models;
using Microsoft.EntityFrameworkCore;

namespace Integrador.Data
{
    public class IntegradorContexto(DbContextOptions<IntegradorContexto> options) : DbContext(options)
    {
        public DbSet<Cliente>? Clientes { get; set; }

        public DbSet<Pedido>? Pedidos { get; set; }

        public DbSet<DetallePedido>? DetallePedidos { get; set; }

        public DbSet<Producto>? Productos { get; set; }

        public DbSet<Estado>? Estados { get; set; }

        public DbSet<Modelo>? Modelos { get; set; }

        public DbSet<Marca>? Marcas { get; set; }

        public DbSet<Suministro>? Suministros { get; set; }

        public DbSet<Proveedor>? Proveedores { get; set; }

        public DbSet<Descuento>? Descuentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Poner el nombre de las tablas en singular
            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<Pedido>().ToTable("Pedido");
            modelBuilder.Entity<DetallePedido>().ToTable("DetallePedido");
            modelBuilder.Entity<Producto>().ToTable("Producto");
            modelBuilder.Entity<Estado>().ToTable("Estado");
            modelBuilder.Entity<Modelo>().ToTable("Modelo");
            modelBuilder.Entity<Marca>().ToTable("Marca");
            modelBuilder.Entity<Suministro>().ToTable("Suministro");
            modelBuilder.Entity<Proveedor>().ToTable("Proveedor");
            modelBuilder.Entity<Descuento>().ToTable("Descuento");

            // Deshabilitar la eliminación en cascada en todas las relaciones
            base.OnModelCreating(modelBuilder);
            foreach (var relationship in
            modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}