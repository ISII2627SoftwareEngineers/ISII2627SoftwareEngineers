using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class Libro
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string TipoLibro { get; set; }

        public string Autor { get; set; }

        public double CalificacionMedia { get; set; }

        public DateTime FechaLanzamiento { get; set; }
    }
}