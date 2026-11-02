using AppForSEII.API.Controllers; 
using AppForSEII.API.DTOs;        
using AppForSEII.API.Models;      
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII.UT.LibrosController_test 
{
    public class GetLibrosParaResenar_test : AppForSEII4SqliteUT 
    {
        public GetLibrosParaResenar_test() // Constructor para inicializar la base de datos con datos de prueba
        {
            var generos = new List<Genero>() {
                new Genero(1, "Fantasía", null!),
                new Genero(2, "Ciencia Ficción", null!),
                new Genero(3, "Misterio", null!)
            };

            var editoriales = new List<Editorial>() {
                new Editorial(1, "Minotauro", null!),
                new Editorial(2, "Planeta", null!)
            };

            var libros = new List<Libro>(){ // Creamos los libros con los datos de prueba creados anteriormente
                new Libro(1, "El Imperio Final", "Tapa blanda", "Brandon Sanderson", 4.8, new DateTime(2006, 7, 17), null!, null!, null!, generos[0], editoriales[0], 15.0m, 10, 10.0m),
                new Libro(2, "Dune", "Bolsillo", "Frank Herbert", 4.5, new DateTime(1965, 8, 1), null!, null!, null!, generos[1], editoriales[0], 12.0m, 5, 8.0m),
                new Libro(3, "Y no quedó ninguno", "Tapa dura", "Agatha Christie", 4.2, new DateTime(1939, 11, 6), null!, null!, null!, generos[2], editoriales[1], 18.0m, 20, 12.0m),
                new Libro(4, "Elantris", "Tapa dura", "Brandon Sanderson", 4.6, new DateTime(2005, 4, 21), null!, null!, null!, generos[0], editoriales[0], 20.0m, 15, 15.0m)
            };

            _context.AddRange(generos); // Guardamos los datos en la base de datos en memoria para que el controlador pueda consultarlos
            _context.AddRange(editoriales);
            _context.AddRange(libros);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetLibrosParaResenar_OK() // Método que devuelve los casos de prueba para el método GetLibrosParaResenar del controlador
        {
            var libroDTOs = new List<LibroParaResenarDTO>() { // Creamos la representación DTO exacta de los libros que hemos metido en la BD
                new LibroParaResenarDTO(1, "El Imperio Final", "Tapa blanda", "Fantasía", "Minotauro", "Brandon Sanderson", 4.8, new DateTime(2006, 7, 17)),
                new LibroParaResenarDTO(2, "Dune", "Bolsillo", "Ciencia Ficción", "Minotauro", "Frank Herbert", 4.5, new DateTime(1965, 8, 1)),
                new LibroParaResenarDTO(3, "Y no quedó ninguno", "Tapa dura", "Misterio", "Planeta", "Agatha Christie", 4.2, new DateTime(1939, 11, 6)),
                new LibroParaResenarDTO(4, "Elantris", "Tapa dura", "Fantasía", "Minotauro", "Brandon Sanderson", 4.6, new DateTime(2005, 4, 21))
            };

            var libroDTOsTC1 = new List<LibroParaResenarDTO>() { libroDTOs[0], libroDTOs[1], libroDTOs[2], libroDTOs[3] }; // Caso de prueba 1: todos los libros
            var libroDTOsTC2 = new List<LibroParaResenarDTO>() { libroDTOs[0], libroDTOs[2], libroDTOs[3] }; // Caso de prueba 2: devuelve tapa blanda y tapa dura
            var libroDTOsTC3 = new List<LibroParaResenarDTO>() { libroDTOs[0], libroDTOs[3] }; // Caso de prueba 3: devuelve libros 0 y 3
            var libroDTOsTC4 = new List<LibroParaResenarDTO>() { libroDTOs[1] }; // Caso de prueba 4: devuelve libro 1

            var allTests = new List<object[]> // Empaquetamos los filtros de entrada y el resultado esperado en arrays de objetos
            {               
                new object[] { null,      null,      libroDTOsTC1 }, // Orden de parámetros: { tipoLibro (filtro 1), genero (filtro 2), lista de DTOs esperada }
                new object[] { "Tapa",    null,      libroDTOsTC2 },
                new object[] { null,      "Fantasía", libroDTOsTC3 },
                new object[] { "Bolsillo","Ciencia",  libroDTOsTC4 }
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetLibrosParaResenar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetLibrosParaResenar_OK_test(string? tipoLibro, string? genero, IList<LibroParaResenarDTO> expectedLibros) // Recibe los 3 parámetros definidos en cada 'object[]' del método anterior
        {
            // Arrange: Preparamos el sujeto de prueba (el controlador) con nuestra BBDD falsa
            var controller = new LibroController(_context, null!); // Pasamos null al logger porque no lo necesitamos para este test

            // Act: Ejecutamos la acción real del controlador pasando los filtros
            var result = await controller.GetLibrosParaResenar(tipoLibro, genero);

            // Assert: Comprobamos que el resultado es exactamente el esperado
            var okResult = Assert.IsType<OkObjectResult>(result); // Verificamos que el controlador devolvió un HTTP 200 (OkObjectResult)
            var librosDTOsActual = Assert.IsType<List<LibroParaResenarDTO>>(okResult.Value); // Extraemos el 'Value' de la respuesta HTTP y verificamos que es una lista de DTOs
            
            Assert.Equal(expectedLibros, librosDTOsActual); // Comparamos la lista devuelta con la lista que esperábamos
        }
    }
}