using System;


namespace AppForSEII.API.Models
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; }

        [Required]
        public double PrecioTotal { get; set; }
	
	   //para poner como dice en el texto que el Codigo de descuento tenga entre 5 y 10 caracteres

        [StringLength(10, MinimumLength = 5, ErrorMessage = "El codigo de descuento debe tener entre 5 y 10 caracteres.")]
        public string CodigoDescuento { get; set; }
    }
}
