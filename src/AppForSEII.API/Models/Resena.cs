using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class Resena
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string Titulo { get; set; }

        [Required]
        public DateTime FechaResena { get; set; }
    }
}