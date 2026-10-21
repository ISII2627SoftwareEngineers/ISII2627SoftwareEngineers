using AppForSEII.API.Data;
using AppForSEII.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ResenasController> _logger;

        public ResenasController(ApplicationDbContext context, ILogger<ResenasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResenaDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDetailsForResenas(int id)
        {
            var resena = await _context.Resenas
                .Where(r => r.Id == id)
                .Include(r => r.Usuario) // Join a la tabla de ApplicationUser para nombre, apellidos y teléfono
                .Include(r => r.ResenaItems) // Join a la tabla intermedia
                    .ThenInclude(ri => ri.Libro) // Join a la tabla Libro para el título y tipo
                        .ThenInclude(l => l.Editorial) // Join a la tabla Editorial para el nombre de la editorial
                .Select(r => new ResenaDetailDTO(
                    r.Id,
                    r.Usuario != null ? (r.Usuario.Name ?? string.Empty) : string.Empty,
                    r.Usuario != null ? (r.Usuario.Surname ?? string.Empty) : string.Empty,
                    r.Usuario != null ? (r.Usuario.PhoneNumber ?? string.Empty) : string.Empty,
                    r.FechaResena,
                    r.ResenaItems.Select(ri => new ResenaItemDTO(
                     ri.LibroId,
                     ri.Libro != null ? ri.Libro.Titulo : string.Empty,
                     ri.Libro != null && ri.Libro.Editorial != null ? ri.Libro.Editorial.Nombre : string.Empty,
                     ri.Libro != null ? ri.Libro.TipoLibro : string.Empty,
                     ri.Calificacion,
                     ri.Descripcion ?? string.Empty
                )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (resena == null)
            {
                _logger.LogError($"Error: La reseña con id {id} no existe");
                return NotFound();
            }

            return Ok(resena);
        }
    }
}