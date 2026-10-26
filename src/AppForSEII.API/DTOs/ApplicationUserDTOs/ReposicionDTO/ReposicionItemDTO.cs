using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.ReposicionDTOs;

public class ReposicionItemDTO
{
    [Required]
    [StringLength(100)]
    public string TituloLibro { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }

    // Constructor vacío para serialización JSON
    public ReposicionItemDTO() { }

    // Constructor para proyectar desde LINQ (.Select)
    public ReposicionItemDTO(string tituloLibro, string autor, int cantidad)
    {
        TituloLibro = tituloLibro;
        Autor = autor;
        Cantidad = cantidad;
    }

    public override bool Equals(object? obj)
    {
        return obj is ReposicionItemDTO other &&
               TituloLibro == other.TituloLibro &&
               Autor == other.Autor &&
               Cantidad == other.Cantidad;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TituloLibro, Autor, Cantidad);
    }
}