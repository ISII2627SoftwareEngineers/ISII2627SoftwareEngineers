using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class Libro
    {
        public Libro()
        {
            CompraItems = new List<CompraItem>();
            ResenaItems = new List<ResenaItem>();
            ReposicionItems = new List<ReposicionItem>();
        }

        public Libro(int id, string titulo, string tipoLibro, string autor, double calificacionMedia, DateTime fechaLanzamiento, IList<CompraItem> compraItems, IList<ResenaItem> resenaItems, IList<ReposicionItem> reposicionItems, Genero genero, Editorial editorial)
        {
            Id = id;
            Titulo = titulo;
            TipoLibro = tipoLibro;
            Autor = autor;
            CalificacionMedia = calificacionMedia;
            FechaLanzamiento = fechaLanzamiento;
            CompraItems = compraItems ?? new List<CompraItem>();
            ResenaItems = resenaItems ?? new List<ResenaItem>();
            ReposicionItems = reposicionItems ?? new List<ReposicionItem>();
            Genero = genero;
            Editorial = editorial;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string TipoLibro { get; set; }

        public string Autor { get; set; }

        public double CalificacionMedia { get; set; }

        public DateTime FechaLanzamiento { get; set; }

        public IList<CompraItem> CompraItems { get; set; } // Relación 1 a N con CompraItem
        public IList<ResenaItem> ResenaItems { get; set; }

        public IList<ReposicionItem>ReposicionItems {get; set;}

        public Genero Genero { get; set; }
        public Editorial Editorial { get; set; }
    }
}