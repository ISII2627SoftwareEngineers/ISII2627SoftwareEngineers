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
                   c.Usuario.Nombre,
                   c.Usuario.Apellidos,
                   c.Usuario.Direccion,
                   c.Usuario.PhoneNumber, 
                   c.FechaCompra,
                   (decimal)c.PrecioTotal,
                   c.MetodoPago != null ? c.MetodoPago.GetType().Name: string.Empty, //cambie el c.MetodoPago.ToString() por c.MetodoPago.GetType().Name porque de esta manera solo me pone el nombre del metodo de pago (Ej: "Visa") y no "AppForSEII.API.Models.Visa"
                   c.CodigoDescuento,
                   c.CompraItems
                       .Select(ci => new CompraItemDTO(
                           ci.LibroId,
                           ci.Libro.Titulo ,
                           ci.Libro.Genero.Nombre,
                           ci.Libro.Editorial.Nombre
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

        /*
        cuando relleno todos los campos en la API web, primero va a la clase de 
        CompraForCreateDTO y se instancia el objeto por el constructor
        , y una vez instanciado, se mete en el controlador CreateCompra 
        y empieza con el bloque 1
        */

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraDetailDTO), (int)HttpStatusCode.Created)] // Indica que este método puede devolver un estado HTTP 201 Created con un objeto de tipo CompraDetailDTO
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)] // Indica que este método puede devolver un estado HTTP 400 Bad Request con un objeto de tipo ValidationProblemDetails si hay errores de validación
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateCompra(CompraForCreateDTO compraForCreate)
        {
            // -----------------BLOQUE 1. Validaciones previas de entrada (lista de libros, usuario y método de pago)--------------------
            if (compraForCreate.CompraItems.Count == 0)
            {
                ModelState.AddModelError("CompraItems", "¡Error! Debes incluir al menos un libro para realizar la compra");
            }




            
            //----------------------------------------------------------------------------------------------------------------------
            //NUEVO CASO AÑADIDO
            if (!compraForCreate.Direccion.Contains("Calle"))
            {
                ModelState.AddModelError("Direccion", "¡Error! La dirección de envío debe contener la palabra Calle.");
            }
            //-----------------------------------------------------------------------------------------------------------------------




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

            // Extraigo solo los títulos (strings) de los libros del CompraItemDTO y los pongo en una lista (List<string>), para desp buscarlos  en la base de datos

            var titulosLibros = compraForCreate.CompraItems.Select(ci => ci.Titulo).ToList();    //al extraer un solo UN campo la lista seria de ese UNICO tipo y no del tipo del CompraItemDTO (si fuera coger int e string no se podria), O de un tipo O usando el constructor de una clase (IList<CompraItemDTO>)


            //guardo en la variable libros, los libros (fila) que contengan los títulos de la lista de string titulosLibros
            var libros = _context.Libros 
                .Include(l => l.Genero) // hago estos dos include para que no salga vacío género y editorial cuando el usuario lo vea
                .Include(l => l.Editorial)
            .Where(l => titulosLibros.Contains(l.Titulo)).ToList(); //guardo los libros que han coincidido con los titulos, en una lista de libros con todas sus columnas sin empaquetarlas aun


            // Actualizo los datos de la fila del correo(ej: peter@uclm.es), cambiando esos datos antiguos por los 
            // datos nuevos que he escrito en el post de compra, para que luego en CompraForDetailDTO (al final) los pueda mostrar 
            // correctamente los datos que he introducido nuevos (paso 5 poner nombre, apellidos, dirección, teléfono y método de pago)

            //el user con datos antiguos lo he cambiado por los datos nuevos introducidos en el post
            user.Nombre = compraForCreate.Nombre;
            user.Apellidos = compraForCreate.Apellidos;
            user.Direccion = compraForCreate.Direccion;
            user.PhoneNumber = compraForCreate.Telefono;


            //Creo un nuevo objeto Compra vacia para luego rellenar sus datos de cada compra individual (contenedor principal)
            Compra compra = new Compra(
                0,//el id 0, lo genera la BD porque es autoincremental en BD, cnd sea 1 será la primera compra, cnd sea 2 será la segunda compra, etc)
                DateTime.Now, 
                0, 
                compraForCreate.CodigoDescuento, 
                metodoPagoBd,
                new List<CompraItem>(), //lista de items de libros comprados vacía, se irá rellenando en el bucle foreach de abajo
                user
            ); //todos sus valores son por defecto (abajo los cambio)

            //--------------BLOQUE 3. Validar cada libro, rellenar el DTO, crear los CompraItems y restar stock------------------------

            foreach (var item in compraForCreate.CompraItems)
            {
             // item: es una variable temporal, en cada vuelta representa únicamente el objeto CompraItemDTO de la posición 
             // actual que se está procesando (primero item tiene los datos del objeto de la pos 0, luego la pos 1, etc)

             //CompraForCreate.CompraItems: es la lista de libros que el usuario ha seleccionado para comprar
                
                //libro: contiene la fila completa de el libro con el titulo especifico del item
                var libro = libros.FirstOrDefault(l => l.Titulo == item.Titulo); 

                // Comprueba que la cantidad del item actual (por ej el d la posicion 0, dsp el pos 1, etc.) pedida sea mayor que 1 (según flujo básico)
                if (item.Cantidad <= 1)
                {
                    ModelState.AddModelError("CompraItems", $"¡Error! La cantidad para el libro '{item.Titulo}' debe ser mayor que 1");
                }
                // Si el libro buscado por título no se encuentra en la base de datos, añado un error
                else if (libro == null)
                {
                    ModelState.AddModelError("CompraItems", $"¡Error! El libro '{item.Titulo}' no existe en el sistema.");
                }
                // Si el libro existe pero el stock disponible es menor que la cantidad que se quiere comprar, añado un error
                else if (libro.Stock < item.Cantidad)
                {
                    ModelState.AddModelError("CompraItems", $"¡Error! No hay suficiente stock para el libro '{item.Titulo}'. Stock disponible: {libro.Stock}");
                }
                else
                {
                    // Relleno el objeto tipo CompraItemDTO item actual con los datos reales de la BD: id, precio, género y editorial
                    //esto es informativo, es para que en la interfaz se vea al hacer la compra los datos asociados a ese libro especifico comprado
                    item.LibroID = libro.Id;
                    item.PrecioCompra = (double)libro.PrecioCompra;
                    item.Genero = libro.Genero.Nombre;
                    item.Editorial = libro.Editorial.Nombre;

                    
                    //Creo un nuevo objeto compraItem para despues guardar una nueva fila en la BD que relacione la Compra con el Libro a traves de CompraItem

                    var compraItem = new CompraItem( //instancio los libros individuales que van dentro de la Compra (Libro A, Libro B, etc)
                        item.Cantidad, 
                        libro.Id, 
                        0, 
                        libro, 
                        compra
                    );

                    //Añado el item a la lista del objeto compra, posteriormente lo guardaré
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