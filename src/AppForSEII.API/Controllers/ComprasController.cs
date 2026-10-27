using AppForSEII.API.DTOs;
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

        //----------------------------------------------------------METODO DETAIL PARA COMPRAS-------------------------------------------------------------------
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
                   c.MetodoPago != null ? c.MetodoPago.GetType().Name: string.Empty, //cambie el c.MetodoPago.ToString() por c.MetodoPago.GetType().Name porque de esta manera solo me pone el nombre del metodo de pago (Ej: "Visa") y no "AppForSEII.API.Models.Visa"
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
    

    

    //-----------------------------------------------------------------METODO POST PARA COMPRAS-----------------------------------------------------------------------

    
    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(CompraDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> CreateCompra(CompraForCreateDTO compraForCreate) 
    {
        // -----------------BLOQUE 1. Validaciones previas de entrada--------------------
        if (compraForCreate.CompraItems == null || compraForCreate.CompraItems.Count == 0)
        {
            ModelState.AddModelError("CompraItems", "¡Error! Debes incluir al menos un libro para realizar la compra");
        }
        else
        {
            // Validación de la cantidad de compra (según el flujo básico, mayor que 1)
            foreach (var item in compraForCreate.CompraItems)
            {
                if (item.Cantidad <= 1)
                {
                    ModelState.AddModelError("CompraItems", $"¡Error! La cantidad para el libro '{item.Titulo}' debe ser mayor que 1");
                }
            }
        }

        // Buscar el usuario en la base de datos por su email (UsuarioUserName)
        var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == compraForCreate.UsuarioUserName);
        if (user == null)
        {
            ModelState.AddModelError("UsuarioUserName", "¡Error! El UserName no está registrado");
        }
        //busco el metodo de pago en la base de datos usando el ID que viene del DTO (Ej: "Busca en la tabla MetodosPago de la base de datos el registro que tenga el ID 3")
        var metodoPagoBd = _context.MetodosPago.Find(compraForCreate.MetodoDePago);

        if (metodoPagoBd==null)
        {
            ModelState.AddModelError("MetodoDePago", "¡Error! El método de pago seleccionado no es válido");
        }


        
        if (ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }


        //---------------BLOQUE 2: (Comprobaciones) ----------------------------------
        // Extraer los títulos de los libros del DTO para buscarlos en la base de datos

        var titulosLibros = compraForCreate.CompraItems.Select(ci => ci.Titulo).ToList();

        var libros = _context.Libros
            .Include(l=>l.Genero) //hago estos dos include para que no salga vacio genero y editorial cuando el usuario lo vea
            .Include(l=>l.Editorial)
            .Where(l => titulosLibros.Contains(l.Titulo))
            .ToList();

        //--------------BLOQUE 3. Crear la entidad principal Compra + CompraItems y validar libros------------------------
        
        // Actualizamos los datos del perfil del usuario con los introducidos en el formulario de compra
        // para que luego el GetDetailsForCompras los pueda mostrar correctamente desde c.Usuario
        
        //user aqui nunca es null, pq si lo fuera ya habría devuelto el BadRequest en el BLOQUE 1
        user.Nombre = compraForCreate.Nombre;
        user.Apellidos = compraForCreate.Apellidos;
        user.Direccion = compraForCreate.Direccion;
        user.PhoneNumber = compraForCreate.Telefono;

        Compra compra = new Compra 
        {
            Usuario = user,
            FechaCompra = DateTime.Now,
            CodigoDescuento = compraForCreate.CodigoDescuento,
            MetodoPago = metodoPagoBd, //aqui asignamos el objeto MetodoPago obtenido de la base de datos a traves del ID
            CompraItems = new List<CompraItem>()
        };

        // Validar existencia de los libros, y asignarle el id, precioCompra, genero y editorial,  y crear los items de compra
        foreach (var item in compraForCreate.CompraItems) 
        {
            var libro = libros.FirstOrDefault(l => l.Titulo == item.Titulo);
            
            if (libro == null) 
            {
                ModelState.AddModelError("CompraItems", $"¡Error! El libro titulado '{item.Titulo}' no existe en la base de datos");
            }
            else 
            {
            // Se rellenan con los datos reales de la BD: id, precio, género y editorial (título lo envía el cliente y sirve para buscar el libro)
            item.LibroID = libro.Id;
            item.PrecioCompra = (double)libro.PrecioCompra;
            item.Genero = libro.Genero != null ? libro.Genero.Nombre :string.Empty;
            item.Editorial = libro.Editorial!=null ? libro.Editorial.Nombre : string.Empty ;

                // Inicializador de objetos: el constructor exige CompraId, que aún no existe porque la compra no se ha insertado
                var compraItem = new CompraItem
                {
                    LibroId = libro.Id,
                    Libro = libro,
                    Cantidad = item.Cantidad,
                    Compra = compra
                };

                    compra.CompraItems.Add(compraItem);
            }
        }

        if (ModelState.ErrorCount > 0) 
        {
            //en vez de terminar la ejecucion antes si detecto un error, hago que cada vez que haya 
            //un error este se acumula en un contador de errores y antes de insertar algo erroneo en la base de datos en el 
            //bloque 4, lo que hago es que si hay 1 error como minimo en el contador de errores (ErrorCount) devuelve 
            //que ha habido x errores, y si no hay ninguno sigue hacia delante
            
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        // Precio total: lo calcula la propiedad PrecioTotal de CompraForCreateDTO (suma de precio × cantidad).
        // Es fiable porque PrecioCompra de cada item se ha rellenado antes con el precio de la BD
        compra.PrecioTotal = compraForCreate.PrecioTotal; 

        //-----------------------BLOQUE 4: Inserción en la Base de Datos-----------------------------------
        _context.Add(compra);

        try 
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) 
        {
        _logger.LogError(ex.Message);
            ModelState.AddModelError("Compra", $"¡Error! Hubo un problema al guardar tu compra, por favor inténtalo de nuevo más tarde");
            return Conflict("Error: " + ex.Message);
        }

        //-----------BLOQUE 5: devolver 201 + details (paso 7)--------------------
        // Construyo el DTO de respuesta para el Details usando los datos actualizados del usuario
        var compraDetail = new CompraDetailDTO(
            compra.Id,
            user.Nombre ?? string.Empty,
            user.Apellidos ?? string.Empty,
            user.Direccion ?? string.Empty,
            user.PhoneNumber ?? string.Empty,
            compra.FechaCompra,
            (decimal)compra.PrecioTotal,
            compra.MetodoPago != null ? compra.MetodoPago.GetType().Name : string.Empty, //hago asi el metodo pago para que en vez de que salga por pantalla: "AppForSEII.API.Models.Visa", salga solo "Visa"
            compra.CodigoDescuento,
            compraForCreate.CompraItems
        );

        return CreatedAtAction("GetDetailsForCompras", new { id = compra.Id }, compraDetail);
    }

}
}
