using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class Resena
    {
        public Resena()
        {
            ResenaItems = new List<ResenaItem>();
        }

        public Resena(int id, string titulo, DateTime fechaResena, ApplicationUser usuario, IList<ResenaItem> resenaItems)
        {
            Id = id;
            Titulo = titulo;
            FechaResena = fechaResena;
            Usuario = usuario;
            ResenaItems = resenaItems ?? new List<ResenaItem>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string Titulo { get; set; }

        [Required]
        public DateTime FechaResena { get; set; }

        public ApplicationUser Usuario { get; set; }
        public IList<ResenaItem> ResenaItems { get; set; }
    }
}