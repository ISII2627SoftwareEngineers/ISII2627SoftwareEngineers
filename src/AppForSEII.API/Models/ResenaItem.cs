using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    [PrimaryKey(nameof(ResenaId), nameof(LibroId))]
    public class ResenaItem
    {
        [Required]
        [Range(1, 5)]
        public int Calificacion { get; set; }

        [StringLength(100, MinimumLength = 20)]
        public string? Descripcion { get; set; }

        public int LibroId { get; set; }

        public int ResenaId { get; set; }
    }
}