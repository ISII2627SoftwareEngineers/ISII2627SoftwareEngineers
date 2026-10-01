using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    [PrimaryKey(nameof(ResenaId), nameof(LibroId))]
    public class ResenaItem
    {
        public ResenaItem()
        {
        }

        public ResenaItem(int calificacion, string? descripcion, int libroId, Libro libro, int resenaId, Resena resena)
        {
            Calificacion = calificacion;
            Descripcion = descripcion;
            LibroId = libroId;
            Libro = libro;
            ResenaId = resenaId;
            Resena = resena;
        }

        [Required]
        [Range(1, 5)]
        public int Calificacion { get; set; }

        [StringLength(100, MinimumLength = 20)]
        public string? Descripcion { get; set; }

        public int LibroId { get; set; }
        public Libro Libro { get; set; }

        public int ResenaId { get; set; }
        public Resena Resena { get; set; }
    }
}