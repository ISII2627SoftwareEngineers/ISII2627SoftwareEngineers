using AppForSEII.API.Data;
using AppForSEII.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")] // Define la ruta base del controlador, que será "api/Resenas"
    [ApiController] // Indica que este controlador es un controlador de API, lo que habilita ciertas características 
    public class ResenasController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Referencia al contexto de la base de datos para acceder a las entidades
        private readonly ILogger<ResenasController> _logger; // Referencia al logger para registrar información y errores durante la ejecución del controlador

        public ResenasController(ApplicationDbContext context, ILogger<ResenasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet] // Define que este método responde a solicitudes HTTP GET
        [Route("[action]")] // Define la ruta específica para este método, que será "api/Resenas/GetDetailsForResenas"
        [ProducesResponseType(typeof(ResenaDetailDTO), (int)HttpStatusCode.OK)] // Indica que este método puede devolver un estado HTTP 200 OK con un objeto de tipo ResenaDetailDTO
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Indica que este método puede devolver un estado HTTP 404 Not Found si no se encuentra la reseña
        public async Task<ActionResult> GetDetailsForResenas(int id)
        {
            var resena = await _context.Resenas // Accede a la tabla de reseñas en la base de datos
                .Where(r => r.Id == id) // Filtra la reseña por el ID proporcionado
                .Include(r => r.Usuario) // Join a la tabla de ApplicationUser para nombre, apellidos y teléfono
                .Include(r => r.ResenaItems) // Join a la tabla intermedia
                    .ThenInclude(ri => ri.Libro) // Join a la tabla Libro para el título y tipo
                        .ThenInclude(l => l.Editorial) // Join a la tabla Editorial para el nombre de la editorial
                .Select(r => new ResenaDetailDTO( // Proyección a un DTO para devolver solo la información necesaria
                    r.Id,
                    r.Usuario != null ? (r.Usuario.Nombre ?? string.Empty) : string.Empty, // ? es como un if, si Usuario es null, devuelve string.Empty, si no, devuelve el nombre del usuario o string.Empty si es null
                    r.Usuario != null ? (r.Usuario.Apellidos ?? string.Empty) : string.Empty,
                    r.Usuario != null ? (r.Usuario.PhoneNumber ?? string.Empty) : string.Empty,
                    r.FechaResena,
                    r.ResenaItems.Select(ri => new ResenaItemDTO( // Proyección de los items de la reseña a un DTO
                     ri.LibroId,
                     ri.Libro != null ? ri.Libro.Titulo : string.Empty, // ? es como un if, si Libro es null, devuelve string.Empty, si no, devuelve el título del libro
                     ri.Libro != null && ri.Libro.Editorial != null ? ri.Libro.Editorial.Nombre : string.Empty, // Si Libro o Editorial es null, devuelve string.Empty, si no, devuelve el nombre de la editorial
                     ri.Libro != null ? ri.Libro.TipoLibro : string.Empty,
                     ri.Calificacion,
                     ri.Descripcion ?? string.Empty
                )).ToList() // Convierte la colección de ResenaItemDTO a una lista
                ))
                .FirstOrDefaultAsync(); // Devuelve el primer elemento de la colección o null si no hay elementos

            if (resena == null) // Si no se encuentra la reseña con el ID proporcionado, devuelve un estado HTTP 404 Not Found
            {
                _logger.LogError($"Error: La reseña con id {id} no existe"); // Registra un error en el logger indicando que la reseña no existe
                return NotFound(); // Devuelve un estado HTTP 404 Not Found
            }

            return Ok(resena); // Devuelve un estado HTTP 200 OK con el objeto ResenaDetailDTO
        }

        [HttpPost]
[Route("[action]")]
[ProducesResponseType(typeof(ResenaDetailDTO), (int)HttpStatusCode.Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
public async Task<ActionResult> CreateResena(ResenaForCreateDTO resenaForCreate) 
{
    // 1. Validaciones previas
    if (resenaForCreate.ResenaItems == null || resenaForCreate.ResenaItems.Count == 0)
        ModelState.AddModelError("ResenaItems", "¡Error! Debes incluir al menos un libro para reseñar");

    // Buscar el usuario en la base de datos
    var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == resenaForCreate.UsuarioUserName);
    if (user == null)
        ModelState.AddModelError("Usuario", "¡Error! El UserName no está registrado");

    if (ModelState.ErrorCount > 0)
        return BadRequest(new ValidationProblemDetails(ModelState));

    // 2. Extraer los títulos de los libros del DTO para buscarlos en la BD
    var titulosLibros = resenaForCreate.ResenaItems.Select(ri => ri.Titulo).ToList();

    var libros = _context.Libros
        .Where(l => titulosLibros.Contains(l.Titulo))
        .ToList();

    // 3. Crear la entidad principal Resena
    Resena resena = new Resena 
    {
        Usuario = user,
        FechaResena = DateTime.Now,
        Titulo = resenaForCreate.TituloResena,
        ResenaItems = new List<ResenaItem>()
    };

    // 4. Validar disponibilidad de los libros y crear los items usando el constructor
    foreach (var item in resenaForCreate.ResenaItems) 
    {
        var libro = libros.FirstOrDefault(l => l.Titulo == item.Titulo);
        
        if (libro == null) 
        {
            ModelState.AddModelError("ResenaItems", $"¡Error! El libro titulado '{item.Titulo}' no existe en la base de datos");
        }
        else 
        {
            resena.ResenaItems.Add(new ResenaItem(
                item.Calificacion, 
                item.Descripcion, 
                libro.Id, 
                libro, 
                0, // Le pasamos 0 porque la reseña aún no se ha guardado en la base de datos
                resena
            ));
            
            item.LibroId = libro.Id; // Actualizamos el DTO con el ID real
        }
    }

    if (ModelState.ErrorCount > 0) 
    {
        return BadRequest(new ValidationProblemDetails(ModelState));
    }

    // 5. Guardado en Base de Datos
    _context.Add(resena);

    try 
    {
        await _context.SaveChangesAsync();
    }
    catch (Exception ex) 
    {
        _logger.LogError(ex.Message);
        ModelState.AddModelError("Resena", $"¡Error! Hubo un problema al guardar tu reseña, por favor inténtalo de nuevo más tarde");
        return Conflict("Error: " + ex.Message);
    }

    // 6. Construir el DTO de respuesta (Details)
    var resenaDetail = new ResenaDetailDTO(
        resena.Id, 
        user.Nombre ?? string.Empty, 
        user.Apellidos ?? string.Empty, 
        user.PhoneNumber ?? string.Empty, 
        resena.FechaResena, 
        resenaForCreate.ResenaItems
    );

    return CreatedAtAction("GetDetailsForResenas", new { id = resena.Id }, resenaDetail);
}
    }
}