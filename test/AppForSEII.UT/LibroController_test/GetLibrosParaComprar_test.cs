using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.LibroDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppForSEII.UT.LibroController_test
{
    public class GetLibrosParaComprar_test : AppForSEII4SqliteUT
    {
        // Constructor: inserta en la BD (SQLite) los datos necesarios para las pruebas, es como una mini base de datos  
        public GetLibrosParaComprar_test()
        {
            //inserto los 3 géneros, 2 editoriales ya que para instanciar/crear 
            //un objeto tipo Libro necesito un género y una editorial, y 5 libros de prueba
            
            var generos = new List<Genero>() {
                new Genero(1, "Fantasía", null!),
                new Genero(2, "Ciencia Ficción", null!),
                new Genero(3, "Misterio", null!)
            };

            var editoriales = new List<Editorial>() {
                new Editorial(1, "Minotauro", null!),
                new Editorial(2, "Planeta", null!)
            };


            // Orden de parámetros del constructor de Libro: id, titulo, tipoLibro, autor, calificacionMedia, fechaLanzamiento,
            // compraItems, resenaItems, reposicionItems, genero, editorial, precioCompra, stock, precioReposicion
            var libros = new List<Libro>() {
                new Libro(1, "El Imperio Final", "Tapa blanda", "Brandon Sanderson", 4.8, new DateTime(2006, 7, 17), null!, null!, null!, generos[0], editoriales[0], 15.0m, 10, 10.0m),
                new Libro(2, "Dune", "Bolsillo", "Frank Herbert", 4.5, new DateTime(1965, 8, 1), null!, null!, null!, generos[1], editoriales[0], 12.0m, 5, 8.0m),
                new Libro(3, "Y no quedó ninguno", "Tapa dura", "Agatha Christie", 4.2, new DateTime(1939, 11, 6), null!, null!, null!, generos[2], editoriales[1], 18.0m, 20, 12.0m),
                // Este libro tiene stock = 0, por lo que NO debe devolverse nunca en GetLibrosParaComprar
                new Libro(4, "Elantris", "Tapa dura", "Brandon Sanderson", 4.6, new DateTime(2005, 4, 21), null!, null!, null!, generos[0], editoriales[0], 20.0m, 0, 15.0m),
                new Libro(5, "Neuromante", "Tapa blanda", "William Gibson", 4.1, new DateTime(1984, 7, 1), null!, null!, null!, generos[1], editoriales[1], 25.0m, 3, 18.0m)
            };


            //añado a mi nueva mini base de datos los géneros, editoriales y 
            //al final los libros de prueba ya que si fuera al revés me daría 
            //error de integridad referencial (no puedo insertar un libro con 
            //un género o editorial que no existe)

            //Esto lo hago para que el controlador pueda consultar los datos de prueba y 
            // devolverlos en la respuesta de la API
            _context.AddRange(generos); 
            _context.AddRange(editoriales);
            _context.AddRange(libros);

            //Y finalmente guardo los cambios en la base de datos de prueba (SQLite)
            _context.SaveChanges();
        }

        //Ahora voy a definir 6 casos de prueba para el 
        // select GetLibrosParaComprar, con distintos 
        // filtros de género y precio, y la lista de DTOs esperada 
        // para cada caso.


        // Casos de prueba: { genero, precio, lista de DTOs esperada }
        public static IEnumerable<object?[]> TestCasesFor_GetLibrosParaComprar_OK()
        {
            //A continuación creo la lista de DTOs que representa exactamente los libros 
            // que he metido en la BD de prueba

            // (Formato del DTO: id, titulo, precioCompra, fechaLanzamiento, genero, editorial)
            var libroDTOs = new List<LibroParaComprarDTO>() {
                new LibroParaComprarDTO(1, "El Imperio Final", 15.0m, new DateTime(2006, 7, 17), "Fantasía", "Minotauro"),
                new LibroParaComprarDTO(2, "Dune", 12.0m, new DateTime(1965, 8, 1), "Ciencia Ficción", "Minotauro"),
                new LibroParaComprarDTO(3, "Y no quedó ninguno", 18.0m, new DateTime(1939, 11, 6), "Misterio", "Planeta"),
                new LibroParaComprarDTO(5, "Neuromante", 25.0m, new DateTime(1984, 7, 1), "Ciencia Ficción", "Planeta")
            };

            // Caso de prueba 1: sin filtros -> todos con stock > 0 (el libro 4 con stock 0 no aparece)
            var libroDTOsTC1 = new List<LibroParaComprarDTO>() { libroDTOs[0], libroDTOs[1], libroDTOs[2], libroDTOs[3] };

            // Caso de prueba 2: solo género "Fantasía" -> Se espera el libro 1 (el 4 también es Fantasía pero tiene stock 0)
            var libroDTOsTC2 = new List<LibroParaComprarDTO>() { libroDTOs[0] };

            // Caso de prueba 3: solo precio <= 15 -> Se espera los libros 1 (15) y 2 (12) [tienen que cumplir la condición de <=15]
            var libroDTOsTC3 = new List<LibroParaComprarDTO>() { libroDTOs[0], libroDTOs[1] };

            // Caso de prueba 4: género "Ciencia" y precio <= 20 -> Se espera el libro 2 (el 5 cuesta 25) [que cumpla genero "Ciencia" y precio <= 20]
            var libroDTOsTC4 = new List<LibroParaComprarDTO>() { libroDTOs[1] };

            // Caso de prueba 5: género que no existe -> Se espera lista vacía [porque no hay libros con género "Terror"] 
            var libroDTOsTC5 = new List<LibroParaComprarDTO>();

            // Caso de prueba 6: precio demasiado bajo -> Se espera lista vacía [porque no hay libros con precio <= 5]
            var libroDTOsTC6 = new List<LibroParaComprarDTO>();


            //Y FINALMENTE EMPAQUETO LOS CASOS DE PRUEBA EN UN ARRAY DE OBJETOS,
            // DONDE CADA ELEMENTO DEL ARRAY ES UN ARRAY DE OBJETOS CON LOS DATOS 
            // DE CADA CASO DE PRUEBA: { genero, precio, lista de DTOs esperada }
            var allTests = new List<object?[]>
            {
                new object?[] { null,       null,   libroDTOsTC1 },
                new object?[] { "Fantasía", null,   libroDTOsTC2 },
                new object?[] { null,       15.0m,  libroDTOsTC3 },
                new object?[] { "Ciencia",  20.0m,  libroDTOsTC4 },
                new object?[] { "Terror",   null,   libroDTOsTC5 },
                new object?[] { null,       5.0m,   libroDTOsTC6 },
            };

            return allTests;
        }




        [Theory]
        [MemberData(nameof(TestCasesFor_GetLibrosParaComprar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetLibrosParaComprar_OK_test(string? genero, decimal? precio, IList<LibroParaComprarDTO> expectedLibros) // Recibe los 3 parámetros definidos en cada 'object[]' del método anterior
        {
            // Arrange(preparacion): preparo el controlador con la base de datos de prueba (SQLite), que es el _context que he creado en el constructor
            var controller = new LibroController(_context, null!);//le paso null al logger porque no lo voy a usar en este test

            // Act (accion): llamo al método del controlador que quiero probar(que es GetLibrosParaComprar), pasando los filtros de género y precio
            var result = await controller.GetLibrosParaComprar(genero, precio); //devuelve un IActionResult, que puede ser OkObjectResult o NotFoundResult

            //[pongo await porque es un método asincrono, y tengo que esperar a que termine para poder comprobar el resultado]

            // Assert(Comprobacion): compruebo que el resultado es exactamente 
            // el esperado, es decir, que es un OkObjectResult 
            // y que contiene la lista de DTOs esperada
            var okResult = Assert.IsType<OkObjectResult>(result); //compruebo que el resultado es un OkObjectResult (HTTP 200)
            var librosDTOsActual = Assert.IsType<List<LibroParaComprarDTO>>(okResult.Value);//extraigo el Value del OkObjectResult y compruebo que es una lista de DTOs

            Assert.Equal(expectedLibros, librosDTOsActual); //compruebo que la lista de DTOs devuelta es exactamente la esperada (usando el metodo Equals de la clase LibroParaComprarDTO, que compara todos los campos)
        }
    }
}