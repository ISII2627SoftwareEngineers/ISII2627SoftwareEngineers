using AppForSEII.API.DTOs;

namespace AppForSEII.API.DTOs.CompraDTOs
{
    // DTO para mostrar los detalles completos de una compra
    /*
    NOTA: La palabra base en C# se usa para comunicarse con la clase padre de la que estoy heredando (en este caso, CompraForCreateDTO)
    En este caso, CompraDetailDTO hereda de CompraForCreateDTO, lo que significa que CompraDetailDTO tiene todas las propiedades y métodos de CompraForCreateDTO, además de sus propias propiedades y métodos.
    Cuando creo los detalles de una compra, necesito rellenar tanto los datos nuevos (Id y Fecha) como los datos heredados.
    Por eso, en el constructor de CompraDetailDTO, llamo a base(...) para pasar los datos que son necesarios para inicializar la clase padre (CompraForCreateDTO).

    
    */
    public class CompraDetailDTO : CompraForCreateDTO
    {
        // Constructor que recibe todos los datos de la compra y los pasa al padre
        public CompraDetailDTO(int id, DateTime fecha, string nombre, string apellidos,
            string direccion, string telefono, string metodoPago, string? codigoDescuento,
            decimal precioTotal, IList<CompraItemDTO> compraItems)
            : base(nombre, apellidos, direccion, telefono, metodoPago, codigoDescuento, precioTotal, compraItems)
        {
            Id = id;            
            Fecha = fecha;      
            PrecioTotal = precioTotal; 
        }

        public int Id { get; set; } // Identificador único

        public DateTime Fecha { get; set; } // Fecha en la que se hizo la compra

        public decimal PrecioTotal { get; set; } // Precio total de la compra

        // Compara si dos objetos CompraDetailDTO son iguales
        public override bool Equals(object? obj)
        {
            return obj is CompraDetailDTO dTO &&
                   base.Equals(obj) &&
                   Id == dTO.Id &&
                   PrecioTotal == dTO.PrecioTotal &&
                   Fecha == dTO.Fecha;
        }

        
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, Fecha, PrecioTotal);
        }
    }
}