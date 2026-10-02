using System;


namespace AppForSEII.API.Models
{
    public class Compra
    {
        public Compra()
        {
            CompraItems = new List<CompraItem>(); //Inicializo la lista de CompraItems para evitar null reference exception
        }

        public Compra(int id, DateTime fechaCompra, double precioTotal, string codigoDescuento, MetodoPago metodoPago, IList<CompraItem> compraItems, ApplicationUser usuario)
        {
            Id = id;
            FechaCompra = fechaCompra;
            PrecioTotal = precioTotal;
            CodigoDescuento = codigoDescuento;
            MetodoPago = metodoPago;
            CompraItems = compraItems ?? new List<CompraItem>();
            Usuario = usuario;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; }

        [Required]
        public double PrecioTotal { get; set; }
	
	   //para poner como dice en el texto que el Codigo de descuento tenga entre 5 y 10 caracteres

        [StringLength(10, MinimumLength = 5, ErrorMessage = "El codigo de descuento debe tener entre 5 y 10 caracteres.")]
        public string CodigoDescuento { get; set; }
        public MetodoPago? MetodoPago { get; set; } // Relacion N a 1 con MetodoPago

        public IList<CompraItem> CompraItems { get; set; }// Relacion 1 a N con CompraItem

        public ApplicationUser? Usuario { get; set; }
    }
}
