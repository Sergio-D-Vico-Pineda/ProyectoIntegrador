﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Display(Name = "Número de pedido")]
        public required int PedidoId { get; set; }

        [Display(Name = "Id del producto")]
        public required int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Debe contener solo números enteros positivos.")]
        [Range(1, double.MaxValue, ErrorMessage = "La cantidad tiene que ser mayor que 0.")]
        public required int Cantidad { get; set; }

        private decimal? precioUnidad;

        [Display(Name = "Precio Unitario")]
        [Range(0.1, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$",
            ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PrecioUnidad // Mayor que 0
        {
            get => precioUnidad;
            set
            {
                if (value > 0)
                    precioUnidad = value;
            }
        }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Descuento { get; set; }

        public virtual Producto? Producto { get; set; }
        public virtual Pedido? Pedido { get; set; }
    }
}
