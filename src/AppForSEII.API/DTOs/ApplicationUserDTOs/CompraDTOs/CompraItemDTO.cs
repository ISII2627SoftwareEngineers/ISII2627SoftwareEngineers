namespace AppForSEII.API.DTOs.CompraDTOs
{
    // DTO para representar cada libro de manera individual dentro de la lista de la compra
    public class CompraItemDTO
    {
        // Constructor que recibe los datos principales del libro comprado
        public CompraItemDTO(int libroID, string titulo, string genero, string editorial)
        {
            LibroID = libroID;     
            Titulo = titulo;       
            Genero = genero;       
            Editorial = editorial; 
        }

        public int LibroID { get; set; } 

        public string Titulo { get; set; } 

        public string Genero { get; set; } 

        public string Editorial { get; set; }

        // Compara si dos objetos CompraItemDTO son iguales
        public override bool Equals(object? obj)
        {
            return obj is CompraItemDTO dTO &&
                   LibroID == dTO.LibroID &&
                   Titulo == dTO.Titulo &&
                   Genero == dTO.Genero &&
                   Editorial == dTO.Editorial;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LibroID, Titulo, Genero, Editorial);
        }

        

    }
}