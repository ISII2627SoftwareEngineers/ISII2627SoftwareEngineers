using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class MetodoPago
    {
        public MetodoPago()
        {
            Compras = new List<Compra>();
        }

        public MetodoPago(int id, IList<Reposicion> reposiciones, IList<Compra> compras)
        {
            Id = id;
            Reposiciones = reposiciones ?? new List<Reposicion>();
            Compras = compras ?? new List<Compra>();
        }

        [Key]
        public int Id { get; set; }


        public IList<Reposicion> Reposiciones  {get; set;} // Relacion 1 a N hacia Reposicion
        public IList<Compra> Compras { get; set; } // Relacion 1 a N hacia Compra
    }
}