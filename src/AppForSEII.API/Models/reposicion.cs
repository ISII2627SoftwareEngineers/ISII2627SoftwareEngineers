using System.ComponentModel.DataAnnotations;
namespace AppForSEII.API.Models;

[Table("Reposiciones")]
public class reposicion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime FechaReposicion { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.0, double.MaxValue, ErrorMessage = "El precio total no puede ser negativo.")]
    public decimal PrecioTotal { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
    public string? Comentario { get; set; }

    public reposicion()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is not reposicion other)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}