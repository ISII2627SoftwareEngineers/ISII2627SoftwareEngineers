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

        public Libro(int id, string titulo, string tipoLibro, string autor, double calificacionMedia, DateTime fechaLanzamiento, IList<CompraItem> compraItems, IList<ResenaItem> resenaItems, IList<ReposicionItem> reposicionItems, Genero genero, Editorial editorial, decimal precioCompra, int stock, decimal precioReposicion)
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
            PrecioCompra = precioCompra;
            Stock = stock;
            PrecioReposicion = precioReposicion;
            
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;  

        public string TipoLibro { get; set; } = string.Empty;  

        public string Autor { get; set; } = string.Empty;

        public double CalificacionMedia { get; set; }

        public DateTime FechaLanzamiento { get; set; }

        public decimal PrecioCompra { get; set; } //exclusivo para compra

        public int Stock { get; set; } //de reposicion y de compra

        public decimal PrecioReposicion { get; set; } //exclusivo para reposicion

        public IList<CompraItem> CompraItems { get; set; } // Relación 1 a N con CompraItem
        public IList<ResenaItem> ResenaItems { get; set; }

        public IList<ReposicionItem>ReposicionItems {get; set;}



        public Genero Genero { get; set; }
        public Editorial Editorial { get; set; }
    }
}