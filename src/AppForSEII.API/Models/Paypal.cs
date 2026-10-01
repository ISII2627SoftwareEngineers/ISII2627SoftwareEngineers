using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class PayPal : MetodoPago // Indica que PayPal es hijo de MetodoPago
    {
        // Constructor vacio
        public PayPal()
        {
        }

        // Constructor con parametro
        public PayPal(string numeroTelefono){
            NumeroTelefono = numeroTelefono;
        }

        [Required]
        public string NumeroTelefono { get; set; }
    }
}