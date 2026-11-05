using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs;
using AppForSEII.API.Data;
using System.Net;
using AppForSEII.API.DTOs.LibroDTOs; // importé el namespace del DTO LibroParaComprarDTO para poder usarlo en el controlador

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")] //define la ruta URL base para acceder a los metodos de este controlador, que será "api/Libro/nombreDelControlador"
    [ApiController]
    public class LibroController : ControllerBase //hereda de ControllerBase, lo que le permite manejar solicitudes HTTP y devolver respuestas HTTP (como Ok(), BadRequest(), etc.)
    {
        private readonly ApplicationDbContext _context; //esto es una instancia de mi "ApplicationDbContext" para hacer de puente entre la aplicacion(el codigo) y la base de datos, para acceder a los datos de tablas que me interesan para hacer consultas, inserciones, actualizaciones y eliminaciones de datos en la base de datos.
        private readonly ILogger<LibroController> _logger;

        public LibroController(ApplicationDbContext context, ILogger<LibroController> logger)
        {
            _context = context;
            _logger = logger;
        }
        



        //Controlador para obtener los libros disponibles para reseñar, devolviendo un DTO con los atributos relevantes para la reseña.
 [HttpGet]  // Indica que el método responderá a peticiones HTTP GET
[Route("[action]")] // Define la ruta de la acción, que será "api/Libro/GetLibrosParaResenar"
[ProducesResponseType(typeof(IList<LibroParaResenarDTO>), (int) HttpStatusCode.OK)] // Indica que la respuesta será de tipo IList<LibroParaResenarDTO> con un código de estado HTTP 200 OK
public async Task<ActionResult> GetLibrosParaResenar(string? tipoLibro, string? genero) // Los parámetros tipoLibro y genero son opcionales y se usan para filtrar los resultados
{
    var libros = await _context.Libros // Accedemos a la tabla de libros en la base de datos
        .Include(l => l.Genero) // Incluimos la relación con la tabla de géneros para poder acceder a los nombres de los géneros
        .Include(l => l.Editorial) // Incluimos la relación con la tabla de editoriales para poder acceder a los nombres de las editoriales
        // Usamos Contains() para búsquedas parciales
        .Where(l => (string.IsNullOrEmpty(tipoLibro) || l.TipoLibro.Contains(tipoLibro)) // Filtramos por tipo de libro si se proporciona
                 && (string.IsNullOrEmpty(genero) || (l.Genero != null && l.Genero.Nombre.Contains(genero)))) // Filtramos por género si se proporciona, asegurándonos de que el género no sea nulo antes de acceder a su nombre
        .Select(l => new LibroParaResenarDTO( // Creamos un nuevo DTO para cada libro que cumpla con los filtros
            l.Id,
            l.Titulo,
            l.TipoLibro,
            l.Genero != null ? l.Genero.Nombre : string.Empty,
            l.Editorial != null ? l.Editorial.Nombre : string.Empty,
            l.Autor,
            l.CalificacionMedia,
            l.FechaLanzamiento
        ))
        .ToListAsync(); // Ejecutamos la consulta de manera asíncrona y obtenemos la lista de libros que cumplen con los criterios de filtrado

    return Ok(libros); // Devolvemos la lista de libros en la respuesta HTTP con un código de estado 200 OK
}



        /*
            1º: El cliente inserta en la interfaz en api/Libro/GetLibrosParaComprar genero y precio de manera opcional, y le da a buscar.

            2º: El controlador recibe esos parámetros y declara la variable libros, donde insertare una lista con los datos obtenidos de la base de datos, filtrando por género y precio si se proporcionan.
        
            3º: El controlador devuelve la lista de libros en un DTO (LibroParaComprarDTO) con los atributos relevantes para la compra, y el cliente recibe esa lista y la muestra en la interfaz.
        */

        //Controlador para obtener los libros disponibles para comprar (stock > 0), devolviendo un DTO con los atributos relevantes para la compra.
        //permitiendo filtrar opcionalmente por género y precio según el flujo alternativo.
    
        [HttpGet]
        [Route("[action]")] //hace la ruta para unir el swagger con el metodo GetLibrosParaComprar, para que se pueda acceder a este metodo desde la ruta "api/Libro/GetLibrosParaComprar"
        [ProducesResponseType(typeof(IList<LibroParaComprarDTO>), (int)HttpStatusCode.OK)] //tipo de respuesta esperada, en este caso una lista de objetos de tipo LibroParaComprarDTO y un código de estado HTTP 200 OK
        public async Task<ActionResult> GetLibrosParaComprar(string? genero, decimal? precio)
        {
            var libros = await _context.Libros //asigno a la variable libros el resultado de la consulta, que saca todos los libros de la base de datos que cumplen con lo que dice el where, y al final los transforma en una lista de objetos de tipo LibroParaComprarDTO, que es lo que devuelve el metodo.
                .Include(l => l.Genero) //gracias a esto puedo acceder al nombre del género del libro, por el Id que tiene en la tabla de libros, y así poder poner en vez de 1 el nombre del género en el DTO.
                .Include(l => l.Editorial)

                
                /*
                    l es una abreviatura de libro, y no se refiere a la tabla libro, 
                    sino a cada fila individual(es decir, un libro en concreto), 

                    => : expresion lambda (siempre la uso dsp de _context), la uso para consultar o modificar los datos 
                    de las columnas de ese libro en especifico(fila) que el sistema
                    esta evaluando en ese momento 

                */
                
                //Where: filtra y descarta los libros que no cumplen con las condiciones (deja los libros q si la cumplen)

                .Where(l => l.Stock > 0     // Requisito obligatorio: stock > 0 según el flujo. (condicion para filtrar los libros que tienen stock disponible para comprar)
                        && (string.IsNullOrEmpty(genero) || (l.Genero != null && l.Genero.Nombre.Contains(genero))) //Filtra por género si se proporciona, asegurándose de que el género no sea nulo antes de acceder a su nombre
                        && (!precio.HasValue || l.PrecioCompra <= precio.Value)) //!: es NO[precio= el q pongo yo, precioCompra= precio del libro]

                //Select: es como el foreach, va creando un nuevo objeto de tipo 
                // LibroParaComprarDTO para cada libro que cumple con el where anterior, 
                // y va asignando los valores de las propiedades del 
                // libro a las propiedades del DTO.
                .Select(l => new LibroParaComprarDTO(
                    l.Id,
                    l.Titulo,
                    l.PrecioCompra,
                    l.FechaLanzamiento,
                    l.Genero != null ? l.Genero.Nombre : string.Empty,
                    l.Editorial != null ? l.Editorial.Nombre : string.Empty
                ))
                .ToListAsync();

            return Ok(libros); // Devuelve la lista de libros en la respuesta HTTP con un código de estado 200 OK (posicion 0: DTO 1, Posicion 1: DTO 2, etc)
        }

    /*
    cuando hago el select, selecciono todas las filas de la tabla que cumplen 
    con la condicion del where, metiendo los datos de cada columna que me interesa en 
    un nuevo objeto de tipo LibroParaComprarDTO(con su constructor) teniendo todos esas 
    filas de libros de forma independiente, y eso hace el select solamente, 
    
    y cuando llega al .ToListAsync();, es cuando de repente todas esas filas independientes (con datos del DTO) 
    se almacenan en una lista q se crea en ese momento (List<LibroParaComprarDTO>) en orden de 
    arriba a abajo en la tabla libros, 
    
    y esa lista se almacena en la variable libros, que no le he puesto tipo porque 
    en c# no lo necesita, el tipo da por hecho que es el que se le asigna despues del = 
    */


    
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