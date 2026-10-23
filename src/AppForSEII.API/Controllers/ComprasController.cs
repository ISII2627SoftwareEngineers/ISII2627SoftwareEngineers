using AppForSEII.API.Data;
using AppForSEII.API.DTOs.CompraDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprasController> _logger;

        // Constructor para inicializar la base de datos y los registros de errores
        public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDetailsForCompras(int id)
        {
            // 1. Buscamos la compra en la base de datos por su ID y traemos también 
            // toda su información relacionada (usuario, libros, géneros y editoriales)
            var compra = await _context.Compras
                .Where(c => c.Id == id)
                .Include(c => c.Usuario) // Join con ApplicationUser para obtener los datos del usuario
                .Include(c => c.CompraItems) // Join a la tabla intermedia
                    .ThenInclude(ci => ci.Libro) // Join a Libro
                        .ThenInclude(l => l.Genero) // Join a Genero
                .Include(c => c.CompraItems)
                    .ThenInclude(ci => ci.Libro)
                        .ThenInclude(l => l.Editorial) // Join a Editorial
                    // 2. Transformo (mapeo) los datos de la base de datos al DTO limpio que verá el usuario
                .Select(c => new CompraDetailDTO(
                    c.Id,
                    c.FechaCompra,
                    c.Usuario != null ? (c.Usuario.Nombre ?? string.Empty) : string.Empty,
                    c.Usuario != null ? (c.Usuario.Apellidos ?? string.Empty) : string.Empty,
                    c.Usuario != null ? (c.Usuario.Direccion ?? string.Empty) : string.Empty, 
                    c.Usuario != null ? (c.Usuario.PhoneNumber ?? string.Empty) : string.Empty,
                    c.MetodoPago != null ? c.MetodoPago.ToString() : string.Empty,
                    c.CodigoDescuento,
                    (decimal)c.PrecioTotal,
                    // Transformamos cada elemento de la lista de compra en su respectivo DTO
                    c.CompraItems.Select(ci => new CompraItemDTO(
                        ci.Libro != null ? ci.Libro.Id : 0,
                        ci.Libro != null ? ci.Libro.Titulo : string.Empty,
                        ci.Libro != null && ci.Libro.Genero != null ? ci.Libro.Genero.Nombre : string.Empty,
                        ci.Libro != null && ci.Libro.Editorial != null ? ci.Libro.Editorial.Nombre : string.Empty
                    )).ToList()
                ))
                .FirstOrDefaultAsync();
            // 3. Si la compra no existe, guardamos un error y devolvemos un 404 (No encontrado)
            if (compra == null)
            {
                _logger.LogError($"Error: La compra con id {id} no existe");
                return NotFound();
            }
            // 4. Si todo va bien, devolvemos los datos de la compra con un código 200 (OK)
            return Ok(compra);
        }
    }
}