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


            var compra = await _context.Compras //creo un objeto compra, y voy a asignarle una compra, accediendo a la base de datos por _context.Compras  
               .Where(c => c.Id == id) //filtro con where para que solo devuelva la compra con el id especificado


                // 2. Primeros saltos directos (Include) desde Compras hacia sus relaciones directas

               .Include(c => c.Usuario) // Trae los datos del usuario que realizó la compra a traves de UsuarioId (FK) de la tabla Compras

                // 3. Primer camino: Viajamos por CompraItems y Libro para llegar al Género de id especificado
               .Include(c => c.CompraItems) 
                   .ThenInclude(ci => ci.Libro) //paso por la tabla libros...
                       .ThenInclude(l => l.Genero) //...para quedarme con el genero del libro con el id especificado 

                // 4. Segundo camino: Viajamos por CompraItems y Libro para llegar a la Editorial de id especificado
               .Include(c => c.CompraItems) 
                   .ThenInclude(ci => ci.Libro) 
                       .ThenInclude(l => l.Editorial) 


                //si no hago el select, me devuelve un objeto gigante con todos los datos de todas las tablas(por ello hago los DTOs para filtrar y empaquetar solo lo que necesito)
               .Select(c => new CompraDetailDTO(
                   c.Id,
                   // Validamos que el usuario no sea nulo antes de coger sus datos (evita warnings y errores)
                   c.Usuario != null ? c.Usuario.Nombre : string.Empty, //Si el usuario existe, coge su nombre; si no, lo dejo vacío (Condición ? Resultado si es Verdadero : Resultado si es Falso)
                   c.Usuario != null ? c.Usuario.Apellidos : string.Empty,
                   c.Usuario != null ? c.Usuario.Direccion : string.Empty,
                   c.Usuario != null ? c.Usuario.PhoneNumber ?? string.Empty : string.Empty, //si el PhoneNumber es null, lo dejo vacío (?? string.Empty)
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
                       )).ToList<CompraItemDTO>() //une todas las instacias de CompraItemDTO en una lista para que el CompraDetailDTO final tenga la lista de libros comprados
               ))
               .FirstOrDefaultAsync(); // Devuelve el objeto encontrado (que contiene una lista de compraItems (libros))) o null si no hay compras con el id especificado

                if (compra == null) // Si no se encuentra la compra con el id especificado, devolvemos un 404 Not Found
                {
                    _logger.LogError($"Error: Compra with id {id} does not exist");
                    return NotFound();
                }

            return Ok(compra); // Si se encuentra la compra, devolvemos un 200 OK con el objeto CompraDetailDTO
        }
    
        //Explicacion del .select
        /*
        A partir de la fila gigante obtenida al unir las tablas en la base de datos (haciendo .Include() y .ThenInclude()), 
        utilizamos el .Select() para filtrar y empaquetar los datos necesarios según 
        el flujo de detalles(lo que pide en el parrafo del detail), instanciando 
        un objeto CompraDetailDTO que contendrá la información del cliente, la fecha, 
        el precio total, el método de pago y el código de descuento.

        Y anidando a su vez 
        una lista de CompraItemDTO construida mediante otro .Select() interno dentro del constructor de CompraDetailDTO,
        que recorre cada línea de compra para extraer el ID, el título, el género y la editorial de cada 
        libro, devolviendo finalmente el recurso completo y limpio al usuario.
        */

    

        //-----------------------------------------------------------------METODO POST PARA COMPRAS-----------------------------------------------------------------------

        

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateCompra(CompraForCreateDTO compraForCreate)
        {
            // -----------------BLOQUE 1. Validaciones previas de entrada (lista de libros, usuario y método de pago)--------------------
            if (compraForCreate.CompraItems == null || compraForCreate.CompraItems.Count == 0)
            {
                ModelState.AddModelError("CompraItems", "¡Error! Debes incluir al menos un libro para realizar la compra");
            }

            // Buscar el usuario en la base de datos por su email (UsuarioUserName)
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == compraForCreate.UsuarioUserName);
            if (user == null)
            {
                ModelState.AddModelError("UsuarioUserName", "¡Error! El UserName no está registrado");
            }

            // Busco el método de pago en la base de datos usando el ID que viene del DTO (Ej: "Busca en la tabla MetodosPago el registro con ID 3")
            var metodoPagoBd = _context.MetodosPago.Find(compraForCreate.MetodoDePago);
            if (metodoPagoBd == null)
            {
                ModelState.AddModelError("MetodoDePago", "¡Error! El método de pago seleccionado no es válido");
            }

            // Si algo de lo anterior falla, no seguimos (así user y metodoPagoBd no son null a partir de aquí)
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            //---------------BLOQUE 2: (Comprobaciones) Consulta de libros, actualización del usuario y creación de la compra ----------------------------------
            // Extraer los títulos de los libros del DTO para buscarlos de una sola vez en la base de datos
            var titulosLibros = compraForCreate.CompraItems.Select(ci => ci.Titulo).ToList();

            var libros = _context.Libros
                .Include(l => l.Genero) // hago estos dos include para que no salga vacío género y editorial cuando el usuario lo vea
                .Include(l => l.Editorial)
            .Where(l => titulosLibros.Contains(l.Titulo)).ToList();

            // Actualizamos los datos del perfil del usuario con los introducidos en el formulario de compra
            // para que luego el GetDetailsForCompras los pueda mostrar correctamente desde c.Usuario
            user.Nombre = compraForCreate.Nombre;
            user.Apellidos = compraForCreate.Apellidos;
            user.Direccion = compraForCreate.Direccion;
            user.PhoneNumber = compraForCreate.Telefono;

            // Constructor de Compra: id (0, lo genera la BD), fecha, precio total (0 de momento, se calcula tras recorrer los items),
            // código de descuento, método de pago obtenido de la BD a través del ID, lista de items vacía y usuario
            Compra compra = new Compra(
                0,
                DateTime.Now, 
                0, 
                compraForCreate.CodigoDescuento, 
                metodoPagoBd!,
                new List<CompraItem>(), 
                user!
            );

            //--------------BLOQUE 3. Validar cada libro, rellenar el DTO, crear los CompraItems y restar stock------------------------
            foreach (var item in compraForCreate.CompraItems)
            {
                var libro = libros.FirstOrDefault(l => l.Titulo == item.Titulo);

                // Comprueba que la cantidad pedida sea mayor que 1 (según flujo básico)
                if (item.Cantidad <= 1)
                {
                    ModelState.AddModelError("CompraItems", $"¡Error! La cantidad para el libro '{item.Titulo}' debe ser mayor que 1");
                }
                // Si el libro no se encuentra en la base de datos, añade un error
                else if (libro == null)
                {
                    ModelState.AddModelError("CompraItems", $"¡Error! El libro '{item.Titulo}' no existe en el sistema.");
                }
                // Si el libro existe pero el stock disponible es menor que la cantidad que se quiere comprar, añade un error
                else if (libro.Stock < item.Cantidad)
                {
                    ModelState.AddModelError("CompraItems", $"¡Error! No hay suficiente stock para el libro '{item.Titulo}'. Stock disponible: {libro.Stock}");
                }
                else
                {
                    // Se rellenan con los datos reales de la BD: id, precio, género y editorial (el título lo envía el cliente y sirve para buscar el libro)
                    item.LibroID = libro.Id;
                    item.PrecioCompra = (double)libro.PrecioCompra;
                    item.Genero = libro.Genero.Nombre;
                    item.Editorial = libro.Editorial.Nombre;

                    // Constructor de CompraItem: cantidad, id del libro, id de la compra (0, aún no existe porque la compra no se ha insertado:
                    // EF Core lo rellena solo al guardar gracias a la referencia a la compra), libro y compra
                    var compraItem = new CompraItem(
                        item.Cantidad, 
                        libro.Id, 
                        0, 
                        libro, 
                        compra
                    );

                    compra.CompraItems.Add(compraItem);

                    libro.Stock = libro.Stock - item.Cantidad;
                }
            }

            // Si ha habido problemas de cantidad, libros inexistentes o stock, no guardamos la compra
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Precio total: lo calcula la propiedad PrecioTotal de CompraForCreateDTO (suma de precio × cantidad).
            // Es fiable porque PrecioCompra de cada item se ha rellenado antes con el precio de la BD
            compra.PrecioTotal = compraForCreate.PrecioTotal;

            //-----------------------BLOQUE 4: Inserción en la Base de Datos-----------------------------------
            _context.Add(compra); // no guarda todavía: solo marca la compra para insertarla en el SaveChangesAsync

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
            // Construyo el DTO de respuesta para el Details usando los datos actualizados del usuario PARA ENSEÑARSELO AL USUARIO QUE LO ACABA DE HACER
            var compraDetail = new CompraDetailDTO(
                compra.Id,
                user.Nombre,
                user.Apellidos,
                user.Direccion,
                user.PhoneNumber,
                compra.FechaCompra,
                (decimal)compra.PrecioTotal,
                compra.MetodoPago.GetType().Name, // para que salga solo "Visa" y no "AppForSEII.API.Models.Visa"
                compra.CodigoDescuento,
                compraForCreate.CompraItems
            );

            return CreatedAtAction("GetDetailsForCompras", new { id = compra.Id }, compraDetail);
        }
    


    }
}