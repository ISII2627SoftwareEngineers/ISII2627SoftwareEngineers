using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ReposicionDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII.UT.ReposicionesController_test
{
    public class GetReposicionDetails_test : AppForSEII4SqliteUT
    {
        public GetReposicionDetails_test()
        {
            var genero = new Genero(1, "Fantasía", null!);
            var editorial = new Editorial(1, "Minotauro", null!);
            
            var libro = new Libro(1, "El Imperio Final", "Tapa blanda", "Brandon Sanderson", 4.8, new DateTime(2006, 7, 17), null!, null!, null!, genero, editorial, 15.0m, 10, 10.0m);

            var user = new ApplicationUser 
            { 
                Id = "1", 
                Nombre = "Elena", 
                Apellidos = "Navarro Martínez", 
                UserName = "elena@uclm.es", 
                PhoneNumber = "600123456" 
            };

            var metodoPago = new Visa { Id = 1, NumeroTarjeta = "12345678" };

            var reposicion = new Reposicion(
                1, 
                DateTime.Today, 
                150.0m, 
                "Comentario de prueba", 
                new List<ReposicionItem>(), 
                metodoPago, 
                user
            );

            reposicion.ReposicionItems.Add(new ReposicionItem(10, libro.Id, libro, reposicion, reposicion.Id));

            _context.Generos.Add(genero);
            _context.Editoriales.Add(editorial);
            _context.Libros.Add(libro);
            _context.Users.Add(user);
            _context.Set<MetodoPago>().Add(metodoPago);
            _context.Reposiciones.Add(reposicion);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReposicionDetails_NotFound_test()
        {
            var mock = new Mock<ILogger<ReposicionesController>>();
            ILogger<ReposicionesController> logger = mock.Object;
            var controller = new ReposicionesController(_context, logger);

            var result = await controller.GetReposicionDetails(0);

            Assert.IsType<NotFoundResult>(result.Result); 
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReposicionDetails_Found_test()
        {
            var mock = new Mock<ILogger<ReposicionesController>>();
            ILogger<ReposicionesController> logger = mock.Object;
            var controller = new ReposicionesController(_context, logger);

            var expectedReposicion = new ReposicionDetailsDTO
            {
                Id = 1,
                FechaReposicion = DateTime.Today,
                PrecioTotal = 150.0m,
                NombreUsuario = "Elena",
                Apellidos = "Navarro Martínez",
                TelefonoUsuario = "600123456",
                MetodoPago = "Visa" 
            };

            expectedReposicion.ReposicionItems.Add(
                new ReposicionItemDTO(1, "El Imperio Final", "Brandon Sanderson", 10)
            );

            var result = await controller.GetReposicionDetails(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reposicionDTOActual = Assert.IsType<ReposicionDetailsDTO>(okResult.Value);
            
            Assert.Equal(expectedReposicion, reposicionDTOActual);
        }
    }
}