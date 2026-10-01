using System.ComponentModel.DataAnnotations;
using System;

namespace AppForSEII.API.Models
{
    public class Visa:MetodoPago
    {
        public Visa()
        {
        }

        public Visa(int numeroTarjeta, DateTime fechaCaducidad)
        {
            NumeroTarjeta = numeroTarjeta;
            FechaCaducidad = fechaCaducidad;
        }

        [Required]
        public int NumeroTarjeta { get; set; }


        [Required]
        public DateTime FechaCaducidad { get; set; }


    }
}