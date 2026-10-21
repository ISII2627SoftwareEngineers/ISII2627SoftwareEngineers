using System;

namespace AppForSEII.API.DTOs
{
    public class ResenaItemDTO
    {
        public ResenaItemDTO(int libroId, string titulo, string editorial, string tipoLibro, int calificacion, string? descripcion = "")
        {
            LibroId = libroId;
            Titulo = titulo;
            Editorial = editorial;
            TipoLibro = tipoLibro;
            Calificacion = calificacion;
            Descripcion = descripcion;
        }

        public int LibroId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public string TipoLibro { get; set; } = string.Empty;
        public int Calificacion { get; set; }
        public string? Descripcion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenaItemDTO dTO &&
                   LibroId == dTO.LibroId &&
                   Titulo == dTO.Titulo &&
                   Editorial == dTO.Editorial &&
                   TipoLibro == dTO.TipoLibro &&
                   Calificacion == dTO.Calificacion &&
                   Descripcion == dTO.Descripcion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LibroId, Titulo, Editorial, TipoLibro, Calificacion, Descripcion);
        }
    }
}