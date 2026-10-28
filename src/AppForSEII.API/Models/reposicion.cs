using System.ComponentModel.DataAnnotations;
namespace AppForSEII.API.Models;

[Table("Reposiciones")]
public class Reposicion
{
public Reposicion()
    {
        ReposicionItems = new List<ReposicionItem>();
    }

    public Reposicion(int id, DateTime fechaReposicion, decimal precioTotal, string? comentario, IList<ReposicionItem> reposicionItems, MetodoPago metodoPago, ApplicationUser usuario)
    {
        Id = id;
        FechaReposicion = fechaReposicion;
        PrecioTotal = precioTotal;
        Comentario = comentario;
        ReposicionItems = reposicionItems ?? new List<ReposicionItem>();
        MetodoPago = metodoPago;
        Usuario = usuario;
    }

    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime FechaReposicion { get; set; }

    [Required]
    public decimal PrecioTotal { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
    public string? Comentario { get; set; }

    public IList<ReposicionItem>ReposicionItems {get; set;}

    public MetodoPago? MetodoPago { get; set; }

    public ApplicationUser Usuario{ get; set;}

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