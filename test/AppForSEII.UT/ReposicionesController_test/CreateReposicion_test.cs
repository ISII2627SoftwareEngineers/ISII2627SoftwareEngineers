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
        
        private const string _libroConStockTitulo = "El Archivo de las Tormentas"; 
        
        private const string _libroGenero = "Fantasía";
        private const string _libroEditorial = "Minotauro";

        // 2. Constructor: Preparación de la base de datos en memoria
        public CreateReposicion_test() 
        {
            var genero = new Genero(1, _libroGenero, null!);
            var editorial = new Editorial(1, _libroEditorial, null!);
            
            // Libro 1: Stock 0 (Válido para reponer)
            var libro = new Libro(1, _libroTitulo, "Tapa blanda", "Brandon Sanderson", 4.8, new DateTime(2006, 7, 17), null!, null!, null!, genero, editorial, 15.0m, 0, 10.0m);
            
            // Libro 2 con Stock 5 (Inválido para reponer)
            var libroConStock = new Libro(2, _libroConStockTitulo, "Tapa dura", "Brandon Sanderson", 4.9, new DateTime(2010, 8, 31), null!, null!, null!, genero, editorial, 20.0m, 5, 12.0m);

           // Vinculamos las propiedades a través del constructor parametrizado
            var user = new ApplicationUser(
                _customerName, 
                _customerSurname, 
                "Dirección de prueba",
                _phone, 
                new List<Resena>(), 
                new List<Compra>(), 
                new List<Reposicion>()
            );
            user.Id = "1";
            user.UserName = _userName;

            var visa1 = new Visa("12345678", new DateTime(2030, 12, 31)); // Obtendrá el Id = 1 (Válido)
            var visa2 = new Visa("11111111", new DateTime(2030, 12, 31)); // Obtendrá el Id = 2 (Válido)
            var visa3 = new Visa("22222222", new DateTime(2030, 12, 31)); // Obtendrá el Id = 3 (Válido)
            var visa4 = new Visa("00000000", new DateTime(2030, 12, 31)); // Obtendrá el Id = 4 (Inválido para tu lógica)
            
            _context.Generos.Add(genero);
            _context.Editoriales.Add(editorial);
            _context.Libros.Add(libro);
            _context.Libros.Add(libroConStock);
            _context.Users.Add(user);
            _context.Set<MetodoPago>().AddRange(new List<MetodoPago> { visa1, visa2, visa3, visa4 });
            _context.SaveChanges();
        }

        // 3. Casos de prueba para los errores (Mapeo DTO -> Error esperado)
        public static IEnumerable<object[]> TestCasesFor_CreateReposicion() 
        {
            var itemCorrecto = new ReposicionItemDTO(1, _libroTitulo, "Brandon Sanderson", 10);
            var itemCantidadInvalida = new ReposicionItemDTO(1, _libroTitulo, "Brandon Sanderson", 1);
            var itemLibroFalso = new ReposicionItemDTO(99, "Libro Inventado", "Autor Falso", 10);
            
            // Ítem apuntando al ID 2 (El libro que ya tiene 5 unidades de stock)
            var itemConStock = new ReposicionItemDTO(2, _libroConStockTitulo, "Brandon Sanderson", 10); 

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

            // Caso 7 (Flujo 0) - Reponer un libro que ya tiene stock
            var repoLibroConStock = new ReposicionForCreateDTO(_userName, _customerName, _customerSurname, _phone, 1, "Comentario", new List<ReposicionItemDTO> { itemConStock });

            return new List<object[]> 
            {
                new object[] { repoSinItems, "Error! Debes incluir al menos un libro para reponer." },
                new object[] { repoUserFalso, "Error! El usuario especificado no está registrado." },
                new object[] { repoMetodoNoExiste, "Error! El método de pago especificado no existe." },
                new object[] { repoMetodoInvalido, "Error! El método de pago debe ser Visa, GooglePay o PayPal." },
                new object[] { repoLibroNoExiste, "Error! El libro con ID '99' no existe en la base de datos." },
                new object[] { repoCantidadInvalida, $"Error! La cantidad a reponer para el libro '{_libroTitulo}' debe ser mayor a 1." },
                new object[] { repoLibroConStock, $"Error! El libro '{_libroConStockTitulo}' ya tiene stock disponible (5 unidades) y no requiere reposición." },
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
            
            Assert.Equal("GetReposicionDetails", createdResult.ActionName);
            Assert.True(createdResult.RouteValues!.ContainsKey("id"));
            Assert.Null(createdResult.Value);
        }
    }
}