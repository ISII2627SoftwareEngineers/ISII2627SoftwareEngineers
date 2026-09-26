public class Reposición
{
    public int Id { get; set; }
    public DateTime FechaReposicion { get; set; }
    public decimal PrecioTotal { get; set; }
    public string? Comentario { get; set; }

    public Reposición(){}
    public override bool Equals(object? obj)
    {
        if (obj is not Reposiciousing System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models;

[Table("Reposiciones")]
public class Reposicion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime FechaReposicion { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.0, double.MaxValue, ErrorMessage = "El precio total no puede ser negativo.")]
    public decimal PrecioTotal { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
    public string? Comentario { get; set; }

    public Reposicion() { }

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
}n other)
            return false;

        return Id == other.Id;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}public class Reposición
{
    public int Id { get; set; }
    public DateTime FechaReposicion { get; set; }
    public decimal PrecioTotal { get; set; }
    public string? Comentario { get; set; }

    public Reposición(){}
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
    public DbSet<Reposicion> Reposiciones { get; set; }
}