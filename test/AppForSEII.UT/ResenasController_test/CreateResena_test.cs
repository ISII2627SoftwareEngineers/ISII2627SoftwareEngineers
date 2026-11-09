using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs;
using AppForSEII.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII.UT.ResenasController_test {
    public class CreateResena_test : AppForSEII4SqliteUT {
        // 1. Definimos los datos que vamos a usar repetidamente en los tests
        private const string _userName = "elena.navarro@uclm.es";
        private const string _customerName = "Elena";
        private const string _customerSurname = "Navarro Martínez";
        private const string _deliveryAddress = "Dirección de prueba";
        private const string _phone = "123456789";

        private const string _libroTitulo = "Las maravillosas aventuras de Chipi";
        private const string _libroGenero = "Aventuras";
        private const string _libroEditorial = "Planeta";

        // 2. Creamos el constructor de la clase de test, que se ejecutará antes de cada test
        public CreateResena_test() {
            var generos = new List<Genero>() {
                new Genero(1, _libroGenero, null!)
            };

            var editoriales = new List<Editorial>() {
                new Editorial(1, _libroEditorial, null!)
            };

            var libros = new List<Libro>(){
                new Libro(1, _libroTitulo, "Tapa blanda", "Sergio Muñon", 5.0, new DateTime(2026, 1, 1), null!, null!, null!, generos[0], editoriales[0], 15.0m, 10, 10.0m)
            };

            ApplicationUser user = new ApplicationUser(_customerName, _customerSurname, _deliveryAddress, _phone, new List<Resena>(), new List<Compra>(), new List<Reposicion>());
          
            user.UserName = _userName; // Asignamos el UserName al usuario para que coincida con el que se usará en los tests

            var resena = new Resena(1, "Un libro magnífica", new DateTime(2026, 1, 1), user, new List<ResenaItem>()); // Creamos la reseña con el usuario y una lista vacía de ResenaItems
            resena.ResenaItems.Add(new ResenaItem(5, "Un libro increíble, me ha encantado!", 1, libros[0], 1, resena)); // Creamos un ResenaItem y lo añadimos a la lista de ResenaItems de la reseña

            _context.ApplicationUsers.Add(user); // Guardamos los datos en la base de datos de prueba
            _context.AddRange(generos);
            _context.AddRange(editoriales);
            _context.AddRange(libros);
            _context.Add(resena);
            _context.SaveChanges();
        }

        // 3. Creamos un método que nos devuelva los casos de prueba para el test de creación de reseñas
        public static IEnumerable<object[]> TestCasesFor_CreateResena() {
    var resenaNoItem = new ResenaForCreateDTO(_userName, _customerName, _customerSurname, _deliveryAddress, _phone, "Reseña: novelas de aventuras", new List<ResenaItemDTO>()); // Caso 1:Intentar crear una reseña sin añadir ningún libro

    var resenaItems = new List<ResenaItemDTO>() { new ResenaItemDTO(1, _libroTitulo, _libroEditorial, "Tapa blanda", 5, "Un libro increíble, me ha encantado!") }; // Caso 1

    var resenaUsuarioNoRegistrado = new ResenaForCreateDTO("ruben.sanchez@uclm.es", _customerName, _customerSurname, _deliveryAddress, _phone, "Reseña: Mis lecturas del mes", resenaItems); // Caso 2: Intentar crear una reseña con un UserName que no está registrado en la base de datos

    var resenaLibroNoExiste = new ResenaForCreateDTO(_userName, _customerName, _customerSurname, _deliveryAddress, _phone, "Reseña: Excelente selección de libros", new List<ResenaItemDTO>() { new ResenaItemDTO(1, "Libro Inventado", "Editorial Falsa", "Tapa blanda", 5, "Test") }); // Caso 3: Intentar crear una reseña con un libro que no existe en la base de datos

    var resenaTituloErroneo = new ResenaForCreateDTO(_userName, _customerName, _customerSurname, _deliveryAddress, _phone, "Me ha encantado!", resenaItems);

    var allTests = new List<object[]> // Emparejamos cada caso de prueba con el mensaje de error esperado que debería devolver el controlador
    {
        new object[] { resenaNoItem, "¡Error! Debes incluir al menos un libro para reseñar" },
        new object[] { resenaUsuarioNoRegistrado, "¡Error! El UserName no está registrado" },
        new object[] { resenaLibroNoExiste, "¡Error! El libro titulado 'Libro Inventado' no existe en la base de datos" },
        new object[] { resenaTituloErroneo, "¡Error! El título de la reseña debe empezar por Reseña:"}
    };

    return allTests;
}

        // 4. Creamos el test de creación de reseñas, que usará los casos de prueba definidos en el método anterior
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateResena))]
        public async Task CreateResena_Error_test(ResenaForCreateDTO resenaDTO, string errorExpected) {
            // Arrange
            var mock = new Mock<ILogger<ResenasController>>(); // Creamos un mock del logger para poder pasarlo al controlador
            ILogger<ResenasController> logger = mock.Object;

            var controller = new ResenasController(_context, logger);

            // Act
            var result = await controller.CreateResena(resenaDTO); // Ejecutamos el método del controlador pasándole el DTO erróneo

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); // Verificamos que el resultado sea un BadRequest
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value); // Extraemos los detalles del problema del resultado

            var errorActual = problemDetails.Errors.First().Value[0]; // Sacamos el mensaje de error real del resultado

            Assert.StartsWith(errorExpected, errorActual); // Verificamos que el error devuelto por el controlador coincide con el error esperado
        }

        // 5. Creamos un test de creación de reseñas que debería funcionar correctamente
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateResena_Success_test() {
            // Arrange
            var mock = new Mock<ILogger<ResenasController>>(); // Preparamos el entorno para ejecutar el método del controlador
            ILogger<ResenasController> logger = mock.Object;

            var controller = new ResenasController(_context, logger);

            var resenaDTO = new ResenaForCreateDTO(_userName, _customerName, _customerSurname, // Creamos un DTO válido para crear una reseña
                _deliveryAddress, _phone, "Reseña: Mis lecturas del mes", new List<ResenaItemDTO>()
                { new ResenaItemDTO(1, _libroTitulo, _libroEditorial, "Tapa blanda", 5, "Un libro increíble, me ha encantado!") });

            // Act
            var result = await controller.CreateResena(resenaDTO); // Ejecutamos la creación de la reseña con el DTO válido

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result); //Comprobamos que todo ha ido bien, y que el resultado es un CreatedAtActionResult, que es lo que devuelve el controlador cuando todo va bien
            var actualResenaDetailDTO = Assert.IsType<ResenaDetailDTO>(createdResult.Value); // Comprobamos que lo que devuelve el controlador es un ResenaDetailDTO

            var expectedResenaDetailDTO = new ResenaDetailDTO(2, _customerName, _customerSurname, // Construimos cómo debería ser el DTO esperado
                _phone, actualResenaDetailDTO.FechaResena, new List<ResenaItemDTO>()
                { new ResenaItemDTO(1, _libroTitulo, _libroEditorial, "Tapa blanda", 5, "Un libro increíble, me ha encantado!") });

            Assert.Equal(expectedResenaDetailDTO, actualResenaDetailDTO); // Comprobamos que el DTO devuelto por el controlador coincide con el DTO esperado
        }
    }
}