using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs
{
    public class LibroParaResenarDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;  //Pongo string.Empty para evitar que sea null, ya que puede dar warnings
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Este campo debe tener entre 10 y 50 caracteres.")]
        public string TipoLibro { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty; 
        public string Editorial { get; set; } = string.Empty;
         public string Autor { get; set; } = string.Empty;
         public double CalificacionMedia { get; set; }
         public DateTime FechaLanzamiento { get; set; }

         public LibroParaResenarDTO() //Constructor vacío
        {
        }

        public LibroParaResenarDTO(int id, string titulo, string tipoLibro, string genero, string editorial, string autor, double calificacionMedia, DateTime fechaLanzamiento)
        {
            Id = id;
            Titulo = titulo;
            TipoLibro = tipoLibro;
            Genero = genero;
            Editorial = editorial;
            Autor = autor;
            CalificacionMedia = calificacionMedia;
            FechaLanzamiento = fechaLanzamiento;
        }

        public override bool Equals(object? obj)
        {
            return obj is LibroParaResenarDTO dTO &&
                   Id == dTO.Id &&
                   Titulo == dTO.Titulo &&
                   TipoLibro == dTO.TipoLibro &&
                   Genero == dTO.Genero &&
                   Editorial == dTO.Editorial &&
                   Autor == dTO.Autor &&
                   CalificacionMedia == dTO.CalificacionMedia &&
                   FechaLanzamiento == dTO.FechaLanzamiento;
        }

        // Obligatorio añadirlo siempre que se usa Equals()
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Titulo, TipoLibro, Genero, Editorial, Autor, CalificacionMedia, FechaLanzamiento);
        }
    }
}