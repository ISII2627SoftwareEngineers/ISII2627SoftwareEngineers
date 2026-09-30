using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    [PrimaryKey(nameof(LibroId), nameof(ReposicionId))]
        public class ReposicionItem
    {
        [Required]
        public int CantidadReposicion  { get; set; }

        [Required]
        public int LibroId { get; set; }

        [Required]
        public int ReposicionId { get; set; }

        public ReposicionItem(){}


    }
}