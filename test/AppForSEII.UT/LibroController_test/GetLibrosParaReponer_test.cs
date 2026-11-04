using Microsoft.AspNetCore.Mvc;
using AppForSEII.API.Controllers; 
using AppForSEII.API.DTOs.LibroDTOs;        
using AppForSEII.API.Models; // Ajusta este namespace al que contiene tus modelos (Libro, Genero, Editorial)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII.UT.LibrosController_test 
{
    public class GetLibrosParaReponer_test : AppForSEII4SqliteUT 
    {
        public GetLibrosParaReponer_test() 
        {
            var generos = new List<Genero>() {
                new Genero(1, "Fantasía", null!),
                new Genero(2, "Ciencia Ficción", null!)
            };

            var editoriales = new List<Editorial>() {
                new Editorial(1, "Minotauro", null!)
            };

            var libros = new List<Libro>(){ 
                // Libro 1: Stock 0 (Debe aparecer en reposición)
                new Libro(1, "El Imperio Final", "Tapa blanda", "Brandon Sanderson", 4.8, new DateTime(2006, 7, 17), null!, null!, null!, generos[0], editoriales[0], 15.0m, 0, 10.0m),
                // Libro 2: Stock 10 (NO debe aparecer, su stock es > 0)
                new Libro(2, "Dune", "Bolsillo", "Frank Herbert", 4.5, new DateTime(1965, 8, 1), null!, null!, null!, generos[1], editoriales[0], 12.0m, 10, 8.0m),
                // Libro 3: Stock 0 (Debe aparecer en reposición)
                new Libro(3, "Elantris", "Tapa dura", "Brandon Sanderson", 4.6, new DateTime(2005, 4, 21), null!, null!, null!, generos[0], editoriales[0], 20.0m, 0, 15.0m)
            };

            _context.AddRange(generos); 
            _context.AddRange(editoriales);
            _context.AddRange(libros);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetLibrosParaReponer_OK() 
        {
            // Instanciamos usando 'm' para cumplir con el parámetro 'decimal precioReposicion' de tu DTO
            var libroDTOs = new List<LibroParaReponerDTO>() { 
                new LibroParaReponerDTO(1, "El Imperio Final", "Fantasía", "Brandon Sanderson", new DateTime(2006, 7, 17), 10.0m),
                new LibroParaReponerDTO(3, "Elantris", "Fantasía", "Brandon Sanderson", new DateTime(2005, 4, 21), 15.0m)
            };

            var TC1_TodosSinStock = new List<LibroParaReponerDTO>() { libroDTOs[0], libroDTOs[1] }; 
            var TC2_FiltroGenero = new List<LibroParaReponerDTO>() { libroDTOs[0], libroDTOs[1] }; 
            var TC3_FiltroAutorParcial = new List<LibroParaReponerDTO>() { libroDTOs[0], libroDTOs[1] }; 
            var TC4_FiltroSinResultados = new List<LibroParaReponerDTO>(); 

            var allTests = new List<object[]> 
            {               
                // Orden: { genero, autor, lista esperada }
                new object[] { null,       null,     TC1_TodosSinStock }, 
                new object[] { "Fantasía", null,     TC2_FiltroGenero },
                new object[] { null,       "Brand",  TC3_FiltroAutorParcial },
                new object[] { "Terror",   null,     TC4_FiltroSinResultados } 
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetLibrosParaReponer_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetLibrosParaReponer_OK_test(string? genero, string? autor, IList<LibroParaReponerDTO> expectedLibros) 
        {
            var controller = new LibroController(_context, null!); 

            var result = await controller.GetLibrosParaReponer(genero, autor);

            var okResult = Assert.IsType<OkObjectResult>(result); 
            var librosDTOsActual = Assert.IsType<List<LibroParaReponerDTO>>(okResult.Value); 
            
            Assert.Equal(expectedLibros, librosDTOsActual); 
        }
    }
}