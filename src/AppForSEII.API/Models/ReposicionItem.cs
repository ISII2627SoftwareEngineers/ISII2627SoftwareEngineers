using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    [PrimaryKey(nameof(LibroId), nameof(ReposicionId))]
        public class ReposicionItem
    {
        public ReposicionItem(){
        }   

        public ReposicionItem(int cantidadReposicion, int libroId, Libro libro, Reposicion reposicion, int reposicionId){
            CantidadReposicion = cantidadReposicion;
            LibroId = libroId;
            Libro = libro;
            Reposicion = reposicion;
            ReposicionId = reposicionId;
        }

        [Required]
        public int CantidadReposicion  { get; set; }

        [Required]
        public int LibroId { get; set; }
        public Libro Libro{ get; set;}

        [Required]
        public int ReposicionId { get; set;}
        public Reposicion Reposicion { get; set;}
    }
}