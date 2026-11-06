using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.ReposicionDTOs;

public class ReposicionDetailsDTO
{
    public int Id { get; set; }

    [Required]
    public DateTime FechaReposicion { get; set; }

    [Required]
    public decimal PrecioTotal { get; set; }

    // Datos del usuario (nombre, ambos apellidos y teléfono)
    public string NombreUsuario { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string TelefonoUsuario { get; set; } = string.Empty;

    // Método de pago utilizado (ej. nombre o descripción del método)
    public string MetodoPago { get; set; } = string.Empty;

    // Lista de libros repuestos
    public IList<ReposicionItemDTO> ReposicionItems { get; set; } = new List<ReposicionItemDTO>();

    public ReposicionDetailsDTO() { }

    public ReposicionDetailsDTO(int id, DateTime fechaReposicion, decimal precioTotal, string nombreUsuario, string apellidos, string telefonoUsuario, string metodoPago, IList<ReposicionItemDTO> reposicionItems)
    {
        Id = id;
        FechaReposicion = fechaReposicion;
        PrecioTotal = precioTotal;
        NombreUsuario = nombreUsuario;
        Apellidos = apellidos;
        TelefonoUsuario = telefonoUsuario;
        MetodoPago = metodoPago;
        ReposicionItems = reposicionItems ?? new List<ReposicionItemDTO>();
    }
    
    public override bool Equals(object? obj)
    {
        return obj is ReposicionDetailsDTO other &&
               Id == other.Id &&
               FechaReposicion == other.FechaReposicion &&
               PrecioTotal == other.PrecioTotal &&
               NombreUsuario == other.NombreUsuario &&
               Apellidos == other.Apellidos &&
               TelefonoUsuario == other.TelefonoUsuario &&
               MetodoPago == other.MetodoPago &&
               ReposicionItems.SequenceEqual(other.ReposicionItems); // Compara que ambas listas tengan los mismos elementos en el mismo orden
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(FechaReposicion);
        hash.Add(PrecioTotal);
        hash.Add(NombreUsuario);
        hash.Add(Apellidos);
        hash.Add(TelefonoUsuario);
        hash.Add(MetodoPago);
        hash.Add(ReposicionItems.Count); 
        return hash.ToHashCode();
    }
}