using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ReposicionDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII.UT.ReposicionesController_test 
{
    public class CreateReposicion_test : AppForSEII4SqliteUT 
    {
        // 1. Definimos los datos que vamos a usar repetidamente en los tests
        private const string _userName = "admin@uclm.es";
        private const string _customerName = "Elena";
        private const string _customerSurname = "Navarro Martínez";
        private const string _phone = "123456789";

        private const string _libroTitulo = "El Imperio Final";
        private const string _libroGenero = "Fantasía";
        private const string _libroEditorial = "Minotauro";

        // 2. Constructor: Preparación de la base de datos en memoria
        public CreateReposicion_test() 
        {
            var genero = new Genero(1, _libroGenero, null!);
            var editorial = new Editorial(1, _libroEditorial, null!);
            
            var libro = new Libro(1, _libroTitulo, "Tapa blanda", "Brandon Sanderson", 4.8, new DateTime(2006, 7, 17), null!, null!, null!, genero, editorial, 15.0m, 0, 10.0m);

            var user = new ApplicationUser 
            { 
                Id = "1", 
                Nombre = _customerName, 
                Apellidos = _customerSurname, 
                UserName = _userName, 
                PhoneNumber = _phone 
            };

            var metodoPagoValido = new Visa { Id = 1, NumeroTarjeta = "12345678" };
            var metodoPagoInvalido = new Visa { Id = 4, NumeroTarjeta = "00000000" }; // ID 4 para forzar el error de validación (1, 2 o 3)

            _context.Generos.Add(genero);
            _context.Editoriales.Add(editorial);
            _context.Libros.Add(libro);
            _context.Users.Add(user);
            _context.Set<MetodoPago>().AddRange(new List<MetodoPago> { metodoPagoValido, metodoPagoInvalido });
            _context.SaveChanges();
        }

        // 3. Casos de prueba para los errores (Mapeo DTO -> Error esperado)
        public static IEnumerable<object[]> TestCasesFor_CreateReposicion() 
        {
            var itemCorrecto = new ReposicionItemDTO(1, _libroTitulo, "Brandon Sanderson", 10);
            var itemCantidadInvalida = new ReposicionItemDTO(1, _libroTitulo, "Brandon Sanderson", 1); // Cantidad <= 1
            var itemLibroFalso = new ReposicionItemDTO(99, "Libro Inventado", "Autor Falso", 10); // Libro 99 no existe

            // Caso 1: Sin ítems en la lista
            var repoSinItems = new ReposicionForCreateDTO(_userName, _customerName, _customerSurname, _phone, 1, "Comentario", new List<ReposicionItemDTO>());
            
            // Caso 2: Usuario no existe en BD
            var repoUserFalso = new ReposicionForCreateDTO("falso@uclm.es", _customerName, _customerSurname, _phone, 1, "Comentario", new List<ReposicionItemDTO> { itemCorrecto });
            
            // Caso 3: Método de pago no existe en BD (ID 99)
            var repoMetodoNoExiste = new ReposicionForCreateDTO(_userName, _customerName, _customerSurname, _phone, 99, "Comentario", new List<ReposicionItemDTO> { itemCorrecto });
            
            // Caso 4: Método de pago existe pero su ID no es 1, 2 ni 3 (ID 4)
            var repoMetodoInvalido = new ReposicionForCreateDTO(_userName, _customerName, _customerSurname, _phone, 4, "Comentario", new List<ReposicionItemDTO> { itemCorrecto });
            
            // Caso 5: El libro solicitado no existe
            var repoLibroNoExiste = new ReposicionForCreateDTO(_userName, _customerName, _customerSurname, _phone, 1, "Comentario", new List<ReposicionItemDTO> { itemLibroFalso });
            
            // Caso 6: La cantidad de reposición es menor o igual a 1
            var repoCantidadInvalida = new ReposicionForCreateDTO(_userName, _customerName, _customerSurname, _phone, 1, "Comentario", new List<ReposicionItemDTO> { itemCantidadInvalida });

            return new List<object[]> 
            {
                new object[] { repoSinItems, "Error! Debes incluir al menos un libro para reponer." },
                new object[] { repoUserFalso, "Error! El usuario especificado no está registrado." },
                new object[] { repoMetodoNoExiste, "Error! El método de pago especificado no existe." },
                new object[] { repoMetodoInvalido, "Error! El método de pago debe ser Visa, GooglePay o PayPal." },
                new object[] { repoLibroNoExiste, "Error! El libro con ID '99' no existe en la base de datos." },
                new object[] { repoCantidadInvalida, $"Error! La cantidad a reponer para el libro '{_libroTitulo}' debe ser mayor a 1." },
            };
        }

        // 4. Test para evaluar los escenarios de BadRequest
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReposicion))]
        public async Task CreateReposicion_Error_test(ReposicionForCreateDTO reposicionDTO, string errorExpected) 
        {
            // Arrange
            var mock = new Mock<ILogger<ReposicionesController>>();
            var controller = new ReposicionesController(_context, mock.Object);

            // Act
            var result = await controller.CreateReposicion(reposicionDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];
            Assert.StartsWith(errorExpected, errorActual);
        }

        // 5. Test para evaluar la creación exitosa
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReposicion_Success_test() 
        {
            // Arrange
            var mock = new Mock<ILogger<ReposicionesController>>();
            var controller = new ReposicionesController(_context, mock.Object);

            var reposicionDTO = new ReposicionForCreateDTO(
                _userName, _customerName, _customerSurname, _phone, 1, "Reposición mensual", 
                new List<ReposicionItemDTO> { new ReposicionItemDTO(1, _libroTitulo, "Brandon Sanderson", 10) }
            );

            // Act
            var result = await controller.CreateReposicion(reposicionDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            // Validamos que el controlador redirige al método GetReposicionDetails con el ID recién creado
            Assert.Equal("GetReposicionDetails", createdResult.ActionName);
            Assert.True(createdResult.RouteValues!.ContainsKey("id"));
            
            // Siendo críticos con tu código del controlador: 
            // Tu método devuelve CreatedAtAction(nameof(...), new { id }, NULL);
            // Al devolver null explícitamente, no podemos asertar el DTO en el Value, por lo que asertamos que es nulo.
            Assert.Null(createdResult.Value);
        }
    }
}