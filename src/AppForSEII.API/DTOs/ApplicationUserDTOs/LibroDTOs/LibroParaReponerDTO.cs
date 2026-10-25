using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.LibroDTOs;

public class LibroParaReponerDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
    public string Titulo { get; set; }

    public string Genero { get; set; }

    public string Autor { get; set; }

    public DateTime FechaLanzamiento { get; set; }

    public decimal PrecioReposicion { get; set; }

    // Constructor para proyectar los campos fácilmente desde LINQ (.Select)
    public LibroParaReponerDTO(int id, string titulo, string genero, string autor, DateTime fechaLanzamiento, decimal precioReposicion)
    {
        Id = id;
        Titulo = titulo;
        Genero = genero;
        Autor = autor;
        FechaLanzamiento = fechaLanzamiento;
        PrecioReposicion = precioReposicion;
    }

    // Constructor vacío obligatorio para la serialización JSON
    public LibroParaReponerDTO() { }

    // Sobrescribimos el método Equals para comparar por contenido/valor
    public override bool Equals(object? obj)
    {
        return obj is LibroParaReponerDTO dto &&
               Id == dto.Id &&
               Titulo == dto.Titulo &&
               Genero == dto.Genero &&
               Autor == dto.Autor &&
               FechaLanzamiento == dto.FechaLanzamiento &&
               PrecioReposicion == dto.PrecioReposicion;
    }

    // Al sobrescribir Equals es necesario sobrescribir GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Titulo, Genero, Autor, FechaLanzamiento, PrecioReposicion);
    }
}