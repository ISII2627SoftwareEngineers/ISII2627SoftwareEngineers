using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppForSEII.API.DTOs.CompraDTOs
{
    /*
    DTO base que contiene los datos necesarios para CREAR una compra.

    Es el objeto que recoge todos los datos necesarios que introduce el usuario para realizar y 
    procesar una nueva compra. 
    
    Sirve como base reutilizable para no duplicar código en otras partes de la aplicación(como en CompraDetailDTO).
    */
    public class CompraForCreateDTO
    {
        // Constructor que recibe todos los datos obligatorios para rellenar el DTO
        public CompraForCreateDTO(string nombre, string apellidos, string direccion, string telefono, 
            string metodoPago, string? codigoDescuento, decimal precioTotal, IList<CompraItemDTO> compraItems)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellidos = apellidos ?? throw new ArgumentNullException(nameof(apellidos));
            Direccion = direccion ?? throw new ArgumentNullException(nameof(direccion));
            Telefono = telefono ?? throw new ArgumentNullException(nameof(telefono));
            MetodoPago = metodoPago ?? throw new ArgumentNullException(nameof(metodoPago));
            CodigoDescuento = codigoDescuento;
            PrecioTotal = precioTotal;
            CompraItems = compraItems ?? throw new ArgumentNullException(nameof(compraItems));
        }

        
        public CompraForCreateDTO()
        {
            CompraItems = new List<CompraItemDTO>();
        }

        [Required(ErrorMessage = "Por favor, introduce tu nombre")]
        public string Nombre { get; set; } 

        [Required(ErrorMessage = "Por favor, introduce tus apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Por favor, introduce tu dirección")]
        public string Direccion { get; set; } // Dirección de envío

        [Required(ErrorMessage = "Por favor, introduce tu teléfono")]
        public string Telefono { get; set; } 

        [Required(ErrorMessage = "Por favor, introduce el método de pago")]
        public string? MetodoPago { get; set; } // Método de pago elegido

        public string? CodigoDescuento { get; set; } // Código de descuento opcional(segun flujo alternativo)

        public decimal PrecioTotal { get; set; } 

        public IList<CompraItemDTO> CompraItems { get; set; } // Lista de libros incluidos en la compra

        // Compara si dos objetos CompraForCreateDTO son exactamente iguales
        public override bool Equals(object? obj)
        {
            return obj is CompraForCreateDTO dTO &&
                   Nombre == dTO.Nombre &&
                   Apellidos == dTO.Apellidos &&
                   Direccion == dTO.Direccion &&
                   Telefono == dTO.Telefono &&
                   MetodoPago == dTO.MetodoPago &&
                   CodigoDescuento == dTO.CodigoDescuento &&
                   PrecioTotal == dTO.PrecioTotal &&
                   CompraItems.SequenceEqual(dTO.CompraItems);
        }

        
        public override int GetHashCode()
        {
            return HashCode.Combine(Nombre, Apellidos, Direccion, Telefono, MetodoPago, PrecioTotal);
        }
    }
}