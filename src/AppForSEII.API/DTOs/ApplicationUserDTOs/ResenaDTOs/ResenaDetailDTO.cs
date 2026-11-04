using System;
using System.Collections.Generic;

namespace AppForSEII.API.DTOs
{
    public class ResenaDetailDTO
    {
        public ResenaDetailDTO() 
        { 
        }

        public ResenaDetailDTO(int id, string nombre, string apellidos, string telefono, DateTime fechaResena, IList<ResenaItemDTO> resenaItems)
        {
            Id = id;
            Nombre = nombre;
            Apellidos = apellidos;
            Telefono = telefono;
            FechaResena = fechaResena;
            ResenaItems = resenaItems ?? new List<ResenaItemDTO>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaResena { get; set; }
        
        public IList<ResenaItemDTO> ResenaItems { get; set; } = new List<ResenaItemDTO>(); // Inicializa la lista de ResenaItemDTO para evitar valores nulos

        public override bool Equals(object? obj)
        {
            return obj is ResenaDetailDTO dTO &&
                   Id == dTO.Id &&
                   Nombre == dTO.Nombre &&
                   Apellidos == dTO.Apellidos &&
                   Telefono == dTO.Telefono &&
                   FechaResena == dTO.FechaResena &&
                   ResenaItems.SequenceEqual(dTO.ResenaItems);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nombre, Apellidos, Telefono, FechaResena);
        }
    }
}