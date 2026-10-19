namespace AppForSEII.API.DTOs.LibroDTOs
{
    public class LibroParaComprarDTO
    {
        /*
        DTO: El DTO es un objeto simple cuya única funcion es transportar datos entre el servidor (la API) y el 
        cliente (la interfaz de usuario u otras aplicaciones como Swagger/Postman). Usaremos Swagger para probar 
        la API.
    
        Lo que pasa internamente es esto:
        
            1. La interfaz le dice al controlador: "Dame los libros para comprar".

            2. El controlador va a la base de datos a través del DbContext y obtiene las entidades Libro completas.

            3. El controlador proyecta/transforma esas entidades en objetos DTO (LibroParaComprarDTO), dejando solo los atributos que te interesan.

            4. El DTO se envía a la interfaz para enseñárselo de forma limpia al usuario.

        Esto es importante porque no queremos exponer toda la entidad Libro al usuario, solo los atributos que son relevantes para la compra.

        Resumen: 
        El DTO es lo que uso pasa conectar la BDD con la interfaz, y cuando busco un libro por su nombre, lo recibe el controlador, y cuando el controlador 
        coge los libros que coinciden en su nombre, el DTO hace un nuevo objeto con los atributos que quiero que le muestre al usuario
        */
        // Constructor vacío: Necesario para que .NET cree el DTO automáticamente.
        public LibroParaComprarDTO()
        {
        }

        // Constructor con parámetros: Sirve para crear el DTO pasando los datos de golpe en una línea.
        public LibroParaComprarDTO(int id, string titulo, decimal precioCompra, DateTime fechaLanzamiento, string genero, string editorial)
        {
            Id = id;
            Titulo = titulo;
            PrecioCompra = precioCompra;
            FechaLanzamiento = fechaLanzamiento;
            Genero = genero;
            Editorial = editorial;
        }

        public int Id { get; set; }

        //He puesto string.Empty (que es equivalente a "") 
        //para inicializar la propiedad con una cadena de texto vacía en lugar de dejarla como null
        public string Titulo { get; set; } = string.Empty; 
        public decimal PrecioCompra { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Genero { get; set; } = string.Empty; 
        public string Editorial { get; set; } = string.Empty;

        // Equals: Compara si dos objetos tienen el mismo contenido (obligatorio para los tests automáticos).
        public override bool Equals(object? obj)
        {
            if (obj is LibroParaComprarDTO other)
            {
                return Id == other.Id &&
                       Titulo == other.Titulo &&
                       PrecioCompra == other.PrecioCompra &&
                       FechaLanzamiento == other.FechaLanzamiento &&
                       Genero == other.Genero &&
                       Editorial == other.Editorial;
            }
            return false;
        }

        // GetHashCode: C# obliga por norma a ponerlo siempre de la mano del Equals para que el sistema sepa ubicar y comparar el DTO correctamente.
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Titulo, PrecioCompra, FechaLanzamiento, Genero, Editorial);
        }

    }
}