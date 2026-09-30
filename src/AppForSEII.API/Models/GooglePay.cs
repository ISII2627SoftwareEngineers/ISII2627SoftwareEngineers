using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models
{
    public class GooglePay : MetodoPago //indica que GooglePay es hijo de MetodoPago
    {
        // Constructor vacio
        public GooglePay()
        {
        }

        // Constructor con parametro 
        public GooglePay(string email)
        {
            Email = email;
        }

        [Required]
        [EmailAddress] // esto lo pongo para validar que sea un email correcto que sigue esta sintaxis: usuario@dominio.extension (esto es para que si en el campo pone por ejemplo hola@hola, no lo deje pasar y diga que no es un email valido y que no hayan datos invalidos en la base de datos)
        public string Email { get; set; }
    }
}