using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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

    public ApplicationUser(string? nombre, string? apellidos, string? direccion, string? telefono, IList<Resena> resenas, IList<Compra> compras, IList<Reposicion> reposiciones)
    {
        Nombre = nombre;
        Apellidos = apellidos;
        Direccion = direccion;
        PhoneNumber = telefono;
        Resenas = resenas ?? new List<Resena>();
        Compras = compras ?? new List<Compra>();
        Reposiciones = reposiciones ?? new List<Reposicion>();
    }

    [StringLength(50)]
    public string? Nombre {get;set;}

    [StringLength(50)]
    public string? Apellidos {get;set;}
    public string? Direccion {get;set;}
    public IList<Resena> Resenas { get; set; }

    public IList<Compra> Compras { get; set; } // Relacion 1 a N con Compra
    public IList<Reposicion> Reposiciones { get; set; }

}
