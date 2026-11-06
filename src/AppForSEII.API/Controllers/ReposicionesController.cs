using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.ReposicionDTOs;
using System.Net;
using Microsoft.AspNetCore.Authorization;

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

        _logger.LogInformation("Solicitando detalles de la reposición con ID: {Id}", id);

        var reposicionDetails = await _context.Reposiciones
            .Include(r => r.Usuario)
            .Include(r => r.MetodoPago)
            .Include(r => r.ReposicionItems)
                .ThenInclude(ri => ri.Libro)
            .Where(r => r.Id == id)
            .Select(r => new ReposicionDetailsDTO(
                r.Id,
                r.FechaReposicion,
                r.PrecioTotal,
                r.Usuario != null ? r.Usuario.Nombre : string.Empty,
                r.Usuario != null ? r.Usuario.Apellidos : string.Empty,
                r.Usuario != null ? r.Usuario.PhoneNumber : string.Empty,
                r.MetodoPago != null ? r.MetodoPago.GetType().Name : string.Empty,
                r.ReposicionItems.Select(ri => new ReposicionItemDTO(
                    ri.LibroId,
                    ri.Libro != null ? ri.Libro.Titulo : string.Empty,
                    ri.Libro != null ? ri.Libro.Autor : string.Empty,
                    ri.CantidadReposicion
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        if (reposicionDetails == null)
        {
            _logger.LogError("Error: Reposicion with id {Id} does not exist", id);
            return NotFound();
        }

        return Ok(reposicionDetails);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(ReposicionDetailsDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
public async Task<ActionResult> CreateReposicion(ReposicionForCreateDTO reposicionForCreate)
{
    // Validaciones iniciales
    if (reposicionForCreate.ReposicionItems.Count == 0)
        ModelState.AddModelError("ReposicionItems", "Error! Debes incluir al menos un libro para reponer.");

var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == reposicionForCreate.UsuarioUserName);    if (user == null)
        ModelState.AddModelError("Usuario", "Error! El usuario especificado no está registrado.");

    var metodoPago = await _context.MetodosPago.FirstOrDefaultAsync(mp => mp.Id == reposicionForCreate.MetodoPagoId);
    if (metodoPago == null)
    {
        ModelState.AddModelError("MetodoPago", "Error! El método de pago especificado no existe.");
    }
    else
    {

    // Requisito 5: Validar que el método de pago sea Visa, GooglePay o PayPal mediante sus IDs
    var metodosValidosIds = new List<int> { 1, 2, 3 }; 
    
    if (!metodosValidosIds.Contains(metodoPago.Id))
    {
        ModelState.AddModelError("MetodoPago", "Error! El método de pago debe ser Visa, GooglePay o PayPal.");
    }

}

    // Si hay errores iniciales, cortamos la ejecución
    if (ModelState.ErrorCount > 0)
        return BadRequest(new ValidationProblemDetails(ModelState));


    // Extraemos los IDs de los libros y los buscamos en la base de datos
    var libroIds = reposicionForCreate.ReposicionItems.Select(ri => ri.LibroId).ToList();
    var libros = await _context.Libros
        .Where(l => libroIds.Contains(l.Id))
        .ToListAsync();

    // Instanciamos la entidad Reposicion usando el constructor parametrizado
    Reposicion reposicion = new Reposicion(
        0, // ID 0, ya que EF Core lo autogenerará
        DateTime.UtcNow,
        0, // Precio total inicializado a 0, se actualizará después
        reposicionForCreate.Comentario,
        new List<ReposicionItem>(),
        metodoPago,
        user
    );

    decimal precioTotal = 0;

    foreach (var item in reposicionForCreate.ReposicionItems)
    {
        var libro = libros.FirstOrDefault(l => l.Id == item.LibroId);
        
        // Comprobamos que el libro exista
        if (libro == null)
        {
            ModelState.AddModelError("ReposicionItems", $"Error! El libro con ID '{item.LibroId}' no existe en la base de datos.");
        }
        //Solo se puede reponer libros con stock = 0
        else if (libro.Stock > 0)
        {
            ModelState.AddModelError("ReposicionItems", $"Error! El libro '{libro.Titulo}' ya tiene stock disponible ({libro.Stock} unidades) y no requiere reposición.");
        }
        // Requisito 5: La cantidad de reposición debe ser un valor mayor que 1
        else if (item.Cantidad <= 1) 
        {
            ModelState.AddModelError("ReposicionItems", $"Error! La cantidad a reponer para el libro '{libro.Titulo}' debe ser mayor a 1.");
        }
        else
        {
            // Vinculamos la relación
            reposicion.ReposicionItems.Add(new ReposicionItem(
                item.Cantidad, // cantidadReposicion
                libro.Id,      // libroId
                libro,         // libro
                reposicion,    // reposicion
                reposicion.Id  // reposicionId
            ));

            // Calculamos el precio basándonos en el dato seguro de la BD, no del cliente
            precioTotal += libro.PrecioReposicion * item.Cantidad;

            // Actualizamos el stock del libro (regla de negocio principal de una reposición)
            libro.Stock += item.Cantidad;
        }
    }

    reposicion.PrecioTotal = precioTotal;

    // Si hubo algún problema con las cantidades o libros inexistentes
    if (ModelState.ErrorCount > 0)
    {
        return BadRequest(new ValidationProblemDetails(ModelState));
    }

    _context.Add(reposicion);

    try
    {
        // Guardamos la reposición, los items y la actualización de stock en una sola transacción
        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex.Message);
        ModelState.AddModelError("Reposicion", "Error! Hubo un problema al guardar tu reposición, por favor intenta más tarde.");
        return Conflict(new ValidationProblemDetails(ModelState));
    }

    // Redirigimos al método GET que ya tienes construido, delegando en él la creación del DTO de salida
    return CreatedAtAction(nameof(GetReposicionDetails), new { id = reposicion.Id }, null);
}
}