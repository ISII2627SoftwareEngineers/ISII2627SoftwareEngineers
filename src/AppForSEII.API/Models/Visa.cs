using System.ComponentModel.DataAnnotations;
using System;

namespace AppForSEII.API.Models
{
    public class Visa:MetodoPago
    {
        public Visa()
        {
        }

        public Visa(string numeroTarjeta, DateTime fechaCaducidad)
        {
            NumeroTarjeta = numeroTarjeta;
            FechaCaducidad = fechaCaducidad;
            
        }

        [Required]
        public string NumeroTarjeta { get; set; } = string.Empty;


        [Required]
        public DateTime FechaCaducidad { get; set; }


    }
}