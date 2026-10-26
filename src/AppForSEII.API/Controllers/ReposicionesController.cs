using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.ReposicionDTOs;
using System.Net;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReposicionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReposicionesController> _logger;

    public ReposicionesController(ApplicationDbContext context, ILogger<ReposicionesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Reposiciones/GetReposicionDetails/5
    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(ReposicionDetailsDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ReposicionDetailsDTO>> GetReposicionDetails(int id)
    {
        if (_context.Reposiciones == null)
        {
            _logger.LogError("Error: Reposiciones table does not exist");
            return NotFound();
        }

        _logger.LogInformation("Solicitando detalles de la reposición con ID: {Id}", id);

        var reposicionDetails = await _context.Reposiciones
            .Include(r => r.Usuario)
            .Include(r => r.MetodosPago)
            .Include(r => r.ReposicionItems)
                .ThenInclude(ri => ri.Libro)
            .Where(r => r.Id == id)
            .Select(r => new ReposicionDetailsDTO
            {
                Id = r.Id,
                FechaReposicion = r.FechaReposicion,
                PrecioTotal = r.PrecioTotal,
                
                // Mapeo unificado del nombre y los apellidos del usuario
                NombreUsuario = r.Usuario != null ? r.Usuario.Nombre : string.Empty,
                Apellidos = r.Usuario != null ? r.Usuario.Apellidos : string.Empty,
                TelefonoUsuario = r.Usuario != null ? r.Usuario.PhoneNumber : string.Empty,
                
                MetodoPago = r.MetodosPago != null && r.MetodosPago.Any() ? r.MetodosPago.First().ToString() : string.Empty,
                
                ReposicionItems = r.ReposicionItems.Select(ri => new ReposicionItemDTO(
                    ri.Libro != null ? ri.Libro.Titulo : string.Empty,
                    ri.Libro != null ? ri.Libro.Autor : string.Empty,
                    ri.CantidadReposicion
                )).ToList()
            })
            .FirstOrDefaultAsync();

        if (reposicionDetails == null)
        {
            _logger.LogError("Error: Reposicion with id {Id} does not exist", id);
            return NotFound();
        }

        return Ok(reposicionDetails);
    }
}