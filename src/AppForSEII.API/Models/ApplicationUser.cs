using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Resenas = new List<Resena>();
        Compras = new List<Compra>();
        Reposiciones = new List<Reposicion>();
    }

    public ApplicationUser(string? name, string? surname, IList<Resena> resenas, IList<Compra> compras, IList<Reposicion> reposiciones)
    {
        Name = name;
        Surname = surname;
        Resenas = resenas ?? new List<Resena>();
        Compras = compras ?? new List<Compra>();
        Reposiciones = reposiciones ?? new List<Reposicion>();
    }

    [StringLength(50)]
    public string? Name {get;set;}

    [StringLength(50)]
    public string? Surname {get;set;}
    public IList<Resena> Resenas { get; set; }

    public IList<Compra> Compras { get; set; } // Relacion 1 a N con Compra
    public IList<Reposicion> Reposiciones { get; set; }

}
