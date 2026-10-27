namespace AppForSEII.API.DTOs.CompraDTOs
{
    // DTO para representar cada libro de manera individual dentro de la lista de la compra
    public class CompraItemDTO
    {
        // Constructor que recibe los datos principales del libro comprado
        public CompraItemDTO(int libroID, string titulo, string genero, string editorial, int cantidad)
        {
            LibroID = libroID;     
            Titulo = titulo;       
            Genero = genero;       
            Editorial = editorial; 
            Cantidad= cantidad;
        }

        public int LibroID { get; set; } 

        public string Titulo { get; set; } 

        public string Genero { get; set; } 

        public string Editorial { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(2, int.MaxValue, ErrorMessage = "La cantidad de compra debe ser mayor que 1")]
        public int Cantidad { get; set; }

        // Compara si dos objetos CompraItemDTO son iguales
        public override bool Equals(object? obj)
        {
            return obj is CompraItemDTO dTO &&
                   LibroID == dTO.LibroID &&
                   Titulo == dTO.Titulo &&
                   Genero == dTO.Genero &&
                   Editorial == dTO.Editorial &&
                   Cantidad ==dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LibroID, Titulo, Genero, Editorial, Cantidad);
        }

        

    }
}