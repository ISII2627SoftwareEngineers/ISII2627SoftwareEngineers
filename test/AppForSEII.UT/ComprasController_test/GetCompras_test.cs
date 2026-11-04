using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.ComprasController_test
{
    // Pruebas de unidad del método de acción GetDetailsForCompras (Details del caso de uso "Comprar libros").
    // Hereda de la clase base que configura la base de datos SQLite en memoria (_context) para las pruebas.
    public class GetCompras_test : AppForSEII4SqliteUT
    {
        // El constructor inserta en la base de datos de prueba los datos que necesita el método a probar(GetDetailsForCompras) para devolver un resultado correcto.
        public GetCompras_test()
        {
            // Creo el Género y editorial del libro comprado, lo creo antes de crear 
            // el libro porque necesitan ser creados antes de crear el libro 
            // (el libro necesita que exista genero y editorial, debido a la foreign key).
            var genero = new Genero(1, "Novela", new List<Libro>());
            var editorial = new Editorial(1, "Planeta", new List<Libro>());

            // Creo el libro en la base de datos de prueba, con 
            // el género y editorial creados anteriormente.
            var libro = new Libro(1, "Don Quijote", "Tapa dura", "Cervantes", 4.5,
                new DateTime(1605, 1, 16), new List<CompraItem>(), new List<ResenaItem>(),
                new List<ReposicionItem>(), genero, editorial, 15.0m, 10, 8.0m);

            // Creo el cliente que realiza la compra del libro, lo creo antes de crear la 
            // compra (de aquí salen nombre, apellidos, dirección y teléfono)
            var usuario = new ApplicationUser("Elena", "Navarro Martínez",
                "Avda. España s/n, Albacete 02071", "600123456",
                new List<Resena>(), new List<Compra>(), new List<Reposicion>());

            // Método de pago de la compra (el controlador devolverá su nombre de clase: "Visa")
            var visa = new Visa("1234567812345678", new DateTime(2030, 12, 31));

            // Compra con código de descuento (5-10 caracteres) y lista de items vacía de momento
            // (ya que el item necesita la referencia a la compra para crearse, y la compra necesita 
            // el item para crearse, y el libro necesita el item para crearse, por eso se crean en 
            // este orden y se añaden después)
            var compra = new Compra(1, new DateTime(2026, 11, 1), 25.5, "DESC12345",
                visa, new List<CompraItem>(), usuario);

            // El item se añade después porque necesita la referencia a la compra ya creada (une la compra con el libro comprado)
            compra.CompraItems.Add(new CompraItem(2, 1, 1, libro, compra));

            // Añadimos al final la compra y todos sus objetos relacionados 
            // (que son los anteriores creados) a la base de datos de prueba 
            // (SQLite en memoria) para que el controlador GetDetailsForCompras 
            // pueda consultarlo (usuario, método de pago, libro, género y editorial)

            _context.Add(compra); //aqui escribo los datos en memoria RAM, como si escribo en un bloc de notas
            _context.SaveChanges();//y aqui si que guardo los datos en la base de datos de prueba (SQLite en memoria)
        }

        // Caso de prueba: el id no existe en la base de datos, por lo que se espera un 404 NotFound.
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetCompra_NotFound_test()
        {
            // Arrange: aqui si usamos el logger, porque el 
            // controlador lo necesita, pero no nos interesa 
            // su funcionalidad, por eso usamos un mock 
            // (que es un logger falso que no hace nada, 
            // solo sirve para pasarle al controlador)
            
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;
            var controller = new ComprasController(_context, logger);

            // Act: aqui llamamos al método que queremos probar, 
            // pasándole un id que sabemos que no existe en la 
            // base de datos de prueba (ej: 0)

            // El id 0 no existe en la base de datos de prueba
            var result = await controller.GetDetailsForCompras(0);

            // Assert: Comprobamos que la respuesta es de tipo NotFound (404)

            Assert.IsType<NotFoundResult>(result);
        }

        // Caso de prueba: el id existe, por lo que se espera un 200 OK con los detalles de la compra.
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetCompra_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;
            var controller = new ComprasController(_context, logger);

            // DTO que esperamos que devuelva el método, con los mismos datos insertados en el constructor
            var expectedCompra = new CompraDetailDTO(1, "Elena", "Navarro Martínez",
                "Avda. España s/n, Albacete 02071", "600123456",
                new DateTime(2026, 11, 1), 25.5m, "Visa", "DESC12345",
                new List<CompraItemDTO>());
            expectedCompra.CompraItems.Add(new CompraItemDTO(1, "Don Quijote", "Novela", "Planeta"));

            // Act
            // El id 1 es el de la compra insertada en el constructor
            var result = await controller.GetDetailsForCompras(1);

            // Assert
            // Comprobamos que la respuesta es OK y obtenemos el DTO devuelto
            var okResult = Assert.IsType<OkObjectResult>(result);
            var compraActual = Assert.IsType<CompraDetailDTO>(okResult.Value);

            // Comprobamos que el DTO esperado y el real son iguales (usa el Equals del DTO)
            Assert.Equal(expectedCompra, compraActual);
        }
    }
}