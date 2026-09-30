using System.ComponentModel.DataAnnotations;
using System;

namespace AppForSEII.API.Models
{
    public class Visa:MetodoPago
    {
        [Required]
        public int NumeroTarjeta { get; set; }


        [Required]
        public DateTime FechaCaducidad { get; set; }

        public Visa(){}

    }
}