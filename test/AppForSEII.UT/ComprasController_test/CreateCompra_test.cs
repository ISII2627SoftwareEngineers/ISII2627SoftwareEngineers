using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.ComprasController_test
{
    // Pruebas de unidad del método de acción CreateCompra (Post del caso de uso "Comprar libros").
    public class CreateCompra_test : AppForSEII4SqliteUT
    {
        // 1. Definimos los datos que vamos a usar repetidamente en los tests

        // Datos del formulario de compra (lo que escribe el cliente)
        private const string _userName = "elena.navarro@uclm.es";
        private const string _nombre = "Elena";
        private const string _apellidos = "Navarro Martínez";
        private const string _direccion = "Avda. España s/n, Albacete 02071";
        private const string _telefono = "600123456";
        private const string _codigoDescuento = "DESC12345"; //opcional segun el parrafo 5 del flujo basico

        // Libro con stock suficiente (stock 10): es el que se compra en el caso de exito
        private const string _libro1Titulo = "Don Quijote";
        private const string _libro1Genero = "Novela";
        private const string _libro1Editorial = "Planeta";

        // Libro con stock limitado (stock 3), para probar el error de stock insuficiente
        private const string _libro2Titulo = "El Hobbit";

        // Id del método de pago (Visa) que se inserta en la base de datos de prueba,
        // y un id que no existe para probar el error de método de pago no válido
        private const int _metodoPagoValido = 1;
        private const int _metodoPagoInvalido = 99;

        // 2. Creamos el constructor de la clase de test, que se ejecutará antes de cada test

        // El constructor inserta en la base de datos de prueba los datos que necesita CreateCompra 
        public CreateCompra_test()
        {
            //1º creo los géneros y editoriales que necesito para crear los libros
            var genero = new Genero(1, _libro1Genero, new List<Libro>());
            var editorial = new Editorial(1, _libro1Editorial, new List<Libro>());

            //2º creo los libros que se van a comprar en los tests, con stock suficiente y limitado
            var libros = new List<Libro>()
            {
                new Libro(1, _libro1Titulo, "Tapa dura", "Cervantes", 4.5, new DateTime(1605, 1, 16),
                    new List<CompraItem>(), new List<ResenaItem>(), new List<ReposicionItem>(),
                    genero, editorial, 15.0m, 10, 8.0m),
                new Libro(2, _libro2Titulo, "Tapa blanda", "Tolkien", 4.8, new DateTime(1937, 9, 21),
                    new List<CompraItem>(), new List<ResenaItem>(), new List<ReposicionItem>(),
                    genero, editorial, 12.0m, 3, 6.0m)
            };
            //3º creo el usuario que va a realizar la compra, con datos de perfil distintos a los del formulario

            // El usuario tiene datos de perfil distintos a los del formulario,
            // así comprobamos que el post los actualiza
            var usuario = new ApplicationUser("Pepe", "Pérez", "Calle Falsa 1", "611111111",
                new List<Resena>(), new List<Compra>(), new List<Reposicion>());
            usuario.UserName = _userName; // el constructor de ApplicationUser no asigna el UserName

            //4º creo un método de pago (Visa) que se va a usar en los tests. Al insertarse recibe el id 1

            var visa = new Visa("1234567812345678", new DateTime(2030, 12, 31));

            //5º añado todos los datos en la base de datos de prueba
            _context.AddRange(libros); //añade tmb los géneros y editoriales de los libros de forma implícita, ya que son referencias de los libros
            _context.Add(usuario);
            _context.Add(visa);

            //6º guardo los cambios en la base de datos de prueba
            _context.SaveChanges();
        }

        // 3. Creamos un método que nos devuelva los casos de prueba para el test de creación de compras

        // Casos de prueba de error: cada uno provoca un solo error (el DTO y el mensaje de error esperado)
        public static IEnumerable<object[]> TestCasesFor_CreateCompra()
        {
            // Item válido (libro con stock suficiente y cantidad 2). Se reutiliza en los casos
            // en los que el fallo es otro (usuario o método de pago)
            var itemValido = new List<CompraItemDTO>()
                { new CompraItemDTO(0, _libro1Titulo, string.Empty, string.Empty, 2, 0.0) };
            
            // Caso 1: compra sin ningún libro
            var compraSinLibros = new CompraForCreateDTO(_userName, _nombre, _apellidos, _direccion,
                _telefono, _metodoPagoValido, _codigoDescuento, new List<CompraItemDTO>());

            // Caso 2: cantidad menor que 2 (el flujo básico exige una cantidad mayor que 1)
            var compraCantidadMenorQue2 = new CompraForCreateDTO(_userName, _nombre, _apellidos, _direccion,
                _telefono, _metodoPagoValido, _codigoDescuento,
                new List<CompraItemDTO>() { new CompraItemDTO(0, _libro1Titulo, string.Empty, string.Empty, 1, 0.0) });

            // Caso 3: libro que no existe en la base de datos
            var compraLibroInexistente = new CompraForCreateDTO(_userName, _nombre, _apellidos, _direccion,
                _telefono, _metodoPagoValido, _codigoDescuento,
                new List<CompraItemDTO>() { new CompraItemDTO(0, "Libro que no existe", string.Empty, string.Empty, 2, 0.0) });

            // Caso 4: se piden 5 unidades de un libro (cantidad=5)que solo tiene 3 en stock (cantidad>stock)
            var compraStockInsuficiente = new CompraForCreateDTO(_userName, _nombre, _apellidos, _direccion,
                _telefono, _metodoPagoValido, _codigoDescuento,
                new List<CompraItemDTO>() { new CompraItemDTO(0, _libro2Titulo, string.Empty, string.Empty, 5, 0.0) });

            // Caso 5: usuario no registrado en la base de datos
            var compraUsuarioNoRegistrado = new CompraForCreateDTO("victor.lopez@uclm.es", _nombre, _apellidos,
                _direccion, _telefono, _metodoPagoValido, _codigoDescuento, itemValido);

            // Caso 6: método de pago no válido (no existe en la base de datos)
            var compraMetodoPagoInvalido = new CompraForCreateDTO(_userName, _nombre, _apellidos, _direccion,
                _telefono, _metodoPagoInvalido, _codigoDescuento, itemValido);

            // Empaquetamos todos los casos de prueba en un IEnumerable<object[]>
            var allTests = new List<object[]>
            {   // entrada de CreateCompra - mensaje de error esperado
                new object[] { compraSinLibros, "¡Error! Debes incluir al menos un libro para realizar la compra" },
                new object[] { compraCantidadMenorQue2, "¡Error! La cantidad para el libro 'Don Quijote' debe ser mayor que 1" },
                new object[] { compraLibroInexistente, "¡Error! El libro 'Libro que no existe' no existe en el sistema." },
                new object[] { compraStockInsuficiente, "¡Error! No hay suficiente stock para el libro 'El Hobbit'. Stock disponible: 3" },
                new object[] { compraUsuarioNoRegistrado, "¡Error! El UserName no está registrado" },
                new object[] { compraMetodoPagoInvalido, "¡Error! El método de pago seleccionado no es válido" },
            };

            return allTests;
        }

        // 4. Creamos los tests de unidad para el método CreateCompra

        // Caso de prueba: cada entrada inválida debe devolver un 400 BadRequest con su mensaje de error.
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateCompra))]
        public async Task CreateCompra_Error_test(CompraForCreateDTO compraDTO, string errorExpected)
        {
            // Arrange: simulamos el logger del controlador con Moq, porque el controlador lo usa dentro del metodo probado(CreateCompra) y no queremos que falle por no tenerlo
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;
            var controller = new ComprasController(_context, logger);

            // Act: ejecutamos el método CreateCompra con la entrada de prueba (compraDTO)
            var result = await controller.CreateCompra(compraDTO);

            // Assert
            // Comprobamos que la respuesta es BadRequest y obtenemos el error devuelto (sus detalles)
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            // Obtenemos el primer mensaje de error del diccionario de errores (solo hay uno por cada caso de prueba)
            var errorActual = problemDetails.Errors.First().Value[0];

            // Comprobamos que el mensaje de error esperado y el real son el mismo
            Assert.StartsWith(errorExpected, errorActual);
        }

        //5. Caso de prueba: compra correcta, se espera un 201 Created con los detalles de la compra.

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateCompra_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;
            var controller = new ComprasController(_context, logger);

            // Simulamos lo que enviaría el cliente: solo el título y la cantidad de cada libro.
            // El controlador rellena el resto (id, precio, género, editorial) con los datos de la BD
            var compraForCreate = new CompraForCreateDTO(_userName, _nombre, _apellidos, _direccion,
                _telefono, _metodoPagoValido, _codigoDescuento,
                new List<CompraItemDTO>() { new CompraItemDTO(0, _libro1Titulo, string.Empty, string.Empty, 2, 0.0) });



            // Act
            // Ejecutamos la creación de la compra con el DTO válido
            var result = await controller.CreateCompra(compraForCreate);

            // Assert
            // Comprobamos que la respuesta es CreatedAtAction (201) y obtenemos la compra creada
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var compraActual = Assert.IsType<CompraDetailDTO>(createdResult.Value);

            // DTO esperado: id 1 (primera compra), precio total 15.0 x 2 = 30.0 y método de pago "Visa".
            // La fecha la asigna el controlador con DateTime.Now, así que no se puede saber de antemano:
            // se toma la del resultado real y se comprueba el resto de campos
            var expectedCompra = new CompraDetailDTO(1, _nombre, _apellidos, _direccion, _telefono,
            compraActual.Fecha, 30.0m, "Visa", _codigoDescuento,
            new List<CompraItemDTO>() { new CompraItemDTO(1, _libro1Titulo, _libro1Genero, _libro1Editorial, 2, 15.0) });

            // Comprobamos que el DTO esperado y el real son iguales (usa el Equals del DTO)
            Assert.Equal(expectedCompra, compraActual);
        }
    }
}