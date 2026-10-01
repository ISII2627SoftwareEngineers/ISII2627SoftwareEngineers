using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class Genero
    {
        public Genero()
        {
            Libros = new List<Libro>();
        }

        public Genero(int id, string nombre, IList<Libro> libros)
        {
            Id = id;
            Nombre = nombre;
            Libros = libros ?? new List<Libro>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        public IList<Libro> Libros { get; set; }
    }
}