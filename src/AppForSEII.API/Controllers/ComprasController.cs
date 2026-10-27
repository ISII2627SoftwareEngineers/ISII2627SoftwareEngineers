using AppForSEII.API.DTOs.CompraDTOs;
using AppForSEII.API.Data;
using Microsoft.AspNetCore.Http;
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

        public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //----------------------------------------------------------METODO DETAIL-------------------------------------------------------------------
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDetailsForCompras(int id)
        {
            if (_context.Compras == null) //esto salta si en la base de datos no existe la tabla Compras, lo cual no debería pasar nunca, pero por si acaso
            {
                _logger.LogError("Error: Compras table does not exist");
                return NotFound();
            }

            var compra = await _context.Compras //creo un objeto compra que contiene todos los datos de la compra con el id especificado, incluyendo los datos del usuario y los libros comprados
               .Where(c => c.Id == id) //filtro con where para que solo devuelva la compra con el id especificado



               .Include(c => c.Usuario) // hago el Join con ApplicationUser, que el join era que unia la tabla Compras con la tabla ApplicationUser, mediante la columna UsuarioId de la tabla Compras y la columna Id de la tabla ApplicationUser, para poder acceder a los datos del usuario que hizo la compra (para obtener las columnas Nombre, Apellidos, Direccion y Telefono del usuario que hizo la compra)
               .Include(c => c.CompraItems) // Join de compra con CompraItem, que es la tabla intermedia que relaciona Compras con Libros, para poder acceder a los libros comprados en esa compra mediante la columna CompraId de la tabla CompraItem y la columna Id de la tabla Compras (para obtener el los libros comprados en una compra específica)
                   .ThenInclude(ci => ci.Libro) // Join con la tabla Libro para obtener el título, género y editorial de cada libro comprado, mediante la columna LibroId de la tabla CompraItem y la columna Id de la tabla Libro (para obtener el título, género y editorial de cada libro comprado en una compra específica)
                       .ThenInclude(l => l.Genero) // Join con Genero del libro
               .Include(c => c.CompraItems) // lo vuelvo a incluir para poder hacer el join con Editorial del libro, ya que no se puede hacer un ThenInclude de un ThenInclude
                   .ThenInclude(ci => ci.Libro) //
                       .ThenInclude(l => l.Editorial) // Join con Editorial del libro



               .Select(c => new CompraDetailDTO(
                   c.Id,
                   // Validamos que el usuario no sea nulo antes de coger sus datos (evita warnings y errores)
                   c.Usuario != null ? c.Usuario.Nombre : string.Empty, //Si el usuario existe, coge su nombre; si no, lo dejo vacío (Condición ? Resultado si es Verdadero : Resultado si es Falso)
                   c.Usuario != null ? c.Usuario.Apellidos : string.Empty,
                   c.Usuario != null ? c.Usuario.Direccion : string.Empty,
                   c.Usuario != null ? c.Usuario.PhoneNumber ?? string.Empty : string.Empty,        
                   c.FechaCompra,
                   (decimal)c.PrecioTotal,
                   c.MetodoPago != null ? c.MetodoPago.ToString() : string.Empty,
                   c.CodigoDescuento,
                   c.CompraItems
                       .Select(ci => new CompraItemDTO(
                           ci.LibroId,
                           ci.Libro != null ? ci.Libro.Titulo : string.Empty,
                           ci.Libro != null && ci.Libro.Genero != null ? ci.Libro.Genero.Nombre : string.Empty,
                           ci.Libro != null && ci.Libro.Editorial != null ? ci.Libro.Editorial.Nombre : string.Empty
                       )).ToList<CompraItemDTO>()
               ))
               .FirstOrDefaultAsync();

            if (compra == null) // Si no se encuentra la compra, devolvemos un 404 Not Found
            {
                _logger.LogError($"Error: Compra with id {id} does not exist");
                return NotFound();
            }

            return Ok(compra);
        }
    }

    //-----------------------------------------------------------------METODO POST-----------------------------------------------------------------------


    
}