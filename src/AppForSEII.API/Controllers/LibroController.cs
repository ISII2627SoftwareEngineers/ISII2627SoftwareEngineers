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
        
      [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<LibroParaResenarDTO>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult> GetLibrosParaResenar()
        {
        
            var libros = await _context.Libros
                .Select(l => new LibroParaResenarDTO
                {
                    Id = l.Id,
                    Titulo = l.Titulo
                })
                .ToListAsync();

            return Ok(libros);
        }


        //Controlador para obtener los libros disponibles para comprar, devolviendo un DTO con los atributos relevantes para la compra.
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<LibroParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetLibrosParaComprar()
        {
            var libros = await _context.Libros
                .Include(l => l.Genero)
                .Include(l => l.Editorial)
                .Select(l => new LibroParaComprarDTO(
                    l.Id,
                    l.Titulo,
                    l.Autor,
                    l.TipoLibro,
                    l.CalificacionMedia,
                    l.Genero != null ? l.Genero.Nombre : string.Empty,
                    l.Editorial != null ? l.Editorial.Nombre : string.Empty
                ))
                .ToListAsync();

            return Ok(libros);
        }
    }



    
}