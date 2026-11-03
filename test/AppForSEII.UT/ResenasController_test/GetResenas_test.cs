using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppForSEII.UT.ResenasController_test
{
    public class GetResenas_test : AppForSEII4SqliteUT
    {
        public GetResenas_test()
        {
            // 1. Instanciamos los datos básicos que necesita el libro
            var generos = new List<Genero>() {
                new Genero(1, "Aventuras", null!)
            };

            var editoriales = new List<Editorial>() {
                new Editorial(1, "Planeta", null!)
            };

            var libros = new List<Libro>(){
                new Libro(1, "Las maravillosas aventuras de Chipi", "Tapa blanda", "Sergio Muñon", 5.0, new DateTime(2026, 1, 1), null!, null!, null!, generos[0], editoriales[0], 15.0m, 10, 10.0m)
            };

            // 2. Instanciamos el usuario tal y como aparece en el JSON
            ApplicationUser user = new ApplicationUser("Elena", "Navarro Martínez", "Dirección de prueba", "123456789", new List<Resena>(), new List<Compra>(), new List<Reposicion>());

            // 3. Creamos la Reseña
            var resena = new Resena(1, "stringstri", new DateTime(2026, 1, 1), user, new List<ResenaItem>());

            // 4. Creamos los ResenaItems enlazándolos a la Reseña y al Libro
            resena.ResenaItems.Add(new ResenaItem(5, "Un libro increíble, me ha encantado!", 1, libros[0], 1, resena));

            // 5. Guardamos todo en la base de datos de prueba
            _context.ApplicationUsers.Add(user);
            _context.AddRange(generos);
            _context.AddRange(editoriales);
            _context.AddRange(libros);
            _context.Add(resena);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetResena_NotFound_test()
        {
            // Arrange: Instanciamos el controlador con un logger falso
            var mock = new Mock<ILogger<ResenasController>>();
            ILogger<ResenasController> logger = mock.Object;

            var controller = new ResenasController(_context, logger);

            // Act: Buscamos un ID que sabemos que NO existe en nuestra BD falsa (ej: 0)
            var result = await controller.GetDetailsForResenas(0);

            // Assert: Comprobamos que el controlador devuelve NotFound (Código HTTP 404)
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetResena_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ResenasController>>();
            ILogger<ResenasController> logger = mock.Object;
            var controller = new ResenasController(_context, logger);

            // Recreamos exactamente el DTO que esperamos que nos devuelva el controlador
            var expectedResena = new ResenaDetailDTO(1, "Elena", "Navarro Martínez", "123456789", new DateTime(2026, 1, 1), new List<ResenaItemDTO>());

            // Añadimos el ItemDTO exacto que esperamos
            expectedResena.ResenaItems.Add(new ResenaItemDTO(1, "Las maravillosas aventuras de Chipi", "Planeta", "Tapa blanda", 5, "Un libro increíble, me ha encantado!"));

            // Act: Buscamos el ID 1, que SÍ hemos creado en el constructor
            var result = await controller.GetDetailsForResenas(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resenaDTOActual = Assert.IsType<ResenaDetailDTO>(okResult.Value);
            
            // Comprobamos que el DTO devuelto es idéntico al que construimos nosotros a mano
            Assert.Equal(expectedResena, resenaDTOActual);
        }
    }
}