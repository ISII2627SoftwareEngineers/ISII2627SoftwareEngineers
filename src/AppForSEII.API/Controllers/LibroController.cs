using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs;
using AppForSEII.API.Data;
using System.Net;
using AppForSEII.API.DTOs.LibroDTOs; // importé el namespace del DTO LibroParaComprarDTO para poder usarlo en el controlador

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LibroController> _logger;

        public LibroController(ApplicationDbContext context, ILogger<LibroController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        //Controlador para obtener los libros disponibles para reseñar, devolviendo un DTO con los atributos relevantes para la reseña.
 [HttpGet]
[Route("[action]")]
[ProducesResponseType(typeof(IList<LibroParaResenarDTO>), (int) HttpStatusCode.OK)]
public async Task<ActionResult> GetLibrosParaResenar(string? tipoLibro, string? genero)
{
    var libros = await _context.Libros
        .Include(l => l.Genero)
        .Include(l => l.Editorial)
        // Usamos Contains() para búsquedas parciales
        .Where(l => (string.IsNullOrEmpty(tipoLibro) || l.TipoLibro.Contains(tipoLibro)) 
                 && (string.IsNullOrEmpty(genero) || (l.Genero != null && l.Genero.Nombre.Contains(genero))))
        .Select(l => new LibroParaResenarDTO(
            l.Id,
            l.Titulo,
            l.TipoLibro,
            l.Genero != null ? l.Genero.Nombre : string.Empty,
            l.Editorial != null ? l.Editorial.Nombre : string.Empty,
            l.Autor,
            l.CalificacionMedia,
            l.FechaLanzamiento
        ))
        .ToListAsync();

    return Ok(libros);
}

        //Controlador para obtener los libros disponibles para comprar (stock > 0), devolviendo un DTO con los atributos relevantes para la compra.
        //permitiendo filtrar opcionalmente por género y precio según el flujo alternativo.
    
[HttpGet]
[Route("[action]")]
[ProducesResponseType(typeof(IList<LibroParaComprarDTO>), (int)HttpStatusCode.OK)]
public async Task<ActionResult> GetLibrosParaComprar(string? genero, decimal? precio)
{
    var libros = await _context.Libros
        .Include(l => l.Genero)
        .Include(l => l.Editorial)
        .Where(l => l.Stock > 0 // Requisito obligatorio: stock > 0 según el flujo.
                 && (string.IsNullOrEmpty(genero) || (l.Genero != null && l.Genero.Nombre.Contains(genero)))
                 && (!precio.HasValue || l.PrecioCompra <= precio.Value))
        .Select(l => new LibroParaComprarDTO(
            l.Id,
            l.Titulo,
            l.PrecioCompra,
            l.FechaLanzamiento,
            l.Genero != null ? l.Genero.Nombre : string.Empty,
            l.Editorial != null ? l.Editorial.Nombre : string.Empty
        ))
        .ToListAsync();

    return Ok(libros);
}

[HttpGet]
[Route("[action]")]
[ProducesResponseType(typeof(IList<LibroParaReponerDTO>), (int)HttpStatusCode.OK)]
[ProducesResponseType((int)HttpStatusCode.NotFound)]
public async Task<ActionResult> GetLibrosParaReponer(string? genero, string? autor)
{
    // Validación crítica de seguridad
    if (_context.Libros == null)
    {
        return NotFound("La tabla de libros no existe.");
    }

    var libros = await _context.Libros
        .Include(l => l.Genero) 
        .Where(l => l.Stock == 0 
                 && (string.IsNullOrEmpty(genero) || (l.Genero != null && l.Genero.Nombre.Equals(genero)))
                 && (string.IsNullOrEmpty(autor) || l.Autor.Contains(autor)))
        .Select(l => new LibroParaReponerDTO(
            l.Id,
            l.Titulo,
            l.Genero != null ? l.Genero.Nombre : string.Empty,
            l.Autor,
            l.FechaLanzamiento,
            l.PrecioReposicion 
        ))
        .ToListAsync();

    return Ok(libros);
}
    }


    
}