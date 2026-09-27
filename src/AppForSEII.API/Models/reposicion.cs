using System.ComponentModel.DataAnnotations;
namespace AppForSEII.API.Models;

[Table("Reposiciones")]
public class Reposicion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime FechaReposicion { get; set; }

    [Required]
    public decimal PrecioTotal { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
    public string? Comentario { get; set; }

    public Reposicion()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Reposicion other)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}