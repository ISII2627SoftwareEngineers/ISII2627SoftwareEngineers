using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class MetodoPago
    {
        public MetodoPago()
        {
            Compras = new List<Compra>();
        }

        public MetodoPago(int id, Reposicion reposicion, IList<Compra> compras)
        {
            Id = id;
            Reposicion = reposicion;
            Compras = compras ?? new List<Compra>();
        }

        [Key]
        public int Id { get; set; }


        public Reposicion Reposicion {get; set;}
        public IList<Compra> Compras { get; set; } // Relacion 1 a N hacia Compra
    }
}