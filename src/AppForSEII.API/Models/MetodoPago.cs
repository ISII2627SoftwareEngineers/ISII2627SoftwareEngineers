using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class MetodoPago
    {
        [Key]
        public int Id { get; set; }

        public Reposicion Reposicion {get; set;}
    }
}