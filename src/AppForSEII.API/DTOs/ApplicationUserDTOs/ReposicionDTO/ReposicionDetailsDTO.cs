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
    
    public override bool Equals(object? obj)
    {
        return obj is ReposicionDetailsDTO other &&
               Id == other.Id &&
               FechaReposicion == other.FechaReposicion &&
               PrecioTotal == other.PrecioTotal &&
               NombreUsuario == other.NombreUsuario &&
               Apellidos == other.Apellidos &&
               TelefonoUsuario == other.TelefonoUsuario &&
               MetodoPago == other.MetodoPago;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, FechaReposicion, PrecioTotal, NombreUsuario, Apellidos, TelefonoUsuario);
    }
}