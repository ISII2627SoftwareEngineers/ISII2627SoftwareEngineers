using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models
{
    public class CompraItem
    {

        // Constructor vacio
        public CompraItem()
        {
        }

        // Constructor con parametros para inicializar los atributos
        public CompraItem(int cantidad, int libroId, int compraId)
        {
            Cantidad = cantidad;
            LibroId = libroId;
            CompraId = compraId;
        }

        [Required]
        [Range(2, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 1.")] // Según indica el paso 5 del caso de uso (mayor que 1 la cantidad)
        public int Cantidad { get; set; }

       
        public int LibroId { get; set; }
        public Libro Libro { get; set; } // relacion hacia la entidad Libro
        
        public int CompraId { get; set; }
        public Compra Compra { get; set; } // relacion hacia la entidad Compra

    }
}