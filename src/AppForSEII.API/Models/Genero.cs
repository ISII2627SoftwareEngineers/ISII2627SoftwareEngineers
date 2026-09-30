using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class Genero
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        public IList<Libro> Libros { get; set; }
    }
}