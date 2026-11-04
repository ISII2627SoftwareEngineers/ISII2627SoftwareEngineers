using AppForSEII.API.DTOs;

namespace AppForSEII.API.DTOs.CompraDTOs
{
    // DTO para mostrar los detalles completos de una compra de forma independiente
    public class CompraDetailDTO
    {
      
        // Constructor que inicializa directamente todas las propiedades de la clase
        public CompraDetailDTO(int id, string nombre, string apellidos, string direccion, string telefono, DateTime fecha, decimal precioTotal, string metodoPago, string? codigoDescuento, IList<CompraItemDTO> compraItems)
        {
            Id = id;
            Nombre = nombre;
            Apellidos = apellidos;
            Direccion = direccion;
            Telefono = telefono;
            Fecha = fecha;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            CodigoDescuento = codigoDescuento;
            CompraItems = compraItems ?? new List<CompraItemDTO>(); // Inicializamos la lista de libros comprados, asegurándonos de que nunca sea nula
        }
        

        public int Id { get; set; } // Identificador único
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public DateTime Fecha { get; set; } // Fecha en la que se hizo la compra
        public decimal PrecioTotal { get; set; } // Precio total de la compra
        public string MetodoPago { get; set; }
        public string? CodigoDescuento { get; set; } //código opcional segun flujo basico
        public IList<CompraItemDTO> CompraItems { get; set; }

        // Compara si dos objetos CompraDetailDTO son iguales
        public override bool Equals(object? obj)
        {
            return obj is CompraDetailDTO dTO &&
                   Id == dTO.Id &&
                   Nombre == dTO.Nombre &&
                   Apellidos == dTO.Apellidos &&
                   Direccion == dTO.Direccion &&
                   Telefono == dTO.Telefono &&
                   Fecha == dTO.Fecha && // Compara solo el día: en el post la fecha es DateTime.Now y nunca coincidiría al milisegundo, por eso se compara solo con el día (sin horas, minutos ni segundos)
                   PrecioTotal == dTO.PrecioTotal &&
                   MetodoPago == dTO.MetodoPago &&
                   CodigoDescuento == dTO.CodigoDescuento &&
                  CompraItems.SequenceEqual(dTO.CompraItems); // Compara el contenido de las listas elemento a elemento (con el Equals de CompraItemDTO).
                 // Con EqualityComparer se compararían solo las referencias y devolvería false aunque el contenido fuera igual.
        }
        // Genera un código hash único para el objeto CompraDetailDTO(lo pone asi y no en una linea porque no acepta mas de 9 argumentos)
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Nombre);
            hash.Add(Apellidos);
            hash.Add(Direccion);
            hash.Add(Telefono);
            hash.Add(Fecha);
            hash.Add(PrecioTotal);
            hash.Add(MetodoPago);
            hash.Add(CodigoDescuento);
            hash.Add(CompraItems);
            return hash.ToHashCode();
        }
    }
}