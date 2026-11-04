using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.ReposicionDTOs;

public class ReposicionItemDTO
{
    [Required]
    [StringLength(100)]
    public string TituloLibro { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    [Range(2, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 1.")]
    public int Cantidad { get; set; }

    public int LibroId { get; set; }

    public double PrecioReposicion { get; set; }

    public ReposicionItemDTO() { }

    public ReposicionItemDTO(int libroID, string tituloLibro, string autor, int cantidad)
    {
        LibroId = libroID;
        TituloLibro = tituloLibro;
        Autor = autor;
        Cantidad = cantidad;
    }

    public override bool Equals(object? obj)
    {
        return obj is ReposicionItemDTO other &&
               LibroId == other.LibroId &&
               TituloLibro == other.TituloLibro &&
               Autor == other.Autor &&
               Cantidad == other.Cantidad &&
               PrecioReposicion == other.PrecioReposicion;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LibroId, TituloLibro, Autor, Cantidad, PrecioReposicion);
    }
}