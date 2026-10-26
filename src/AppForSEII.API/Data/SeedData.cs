namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }

            try {
                SeedGenerosEditorialesYLibros(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Books, Genres and Editorials in the Database.");
            }

            try {
                var user = dbContext.Users.OfType<ApplicationUser>().FirstOrDefault(u => u.UserName == "peter@uclm.es");

                if (user == null) 
                    logger.LogError("An error occurred seeding Reviews in the Database: user does not exist");
                else
                    SeedResenas(dbContext, user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Reviews (Resenas) in the Database.");
            }

             try {
                var user = dbContext.Users.OfType<ApplicationUser>().FirstOrDefault(u => u.UserName == "elena@uclm.es");

                if (user == null) 
                logger.LogError("An error occurred seeding Reposiciones in the Database: user does not exist");
                else
                SeedReposiciones(dbContext, user);
                }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Reposiciones in the Database.");
            }

            try {
                 var user = dbContext.Users.OfType<ApplicationUser>().FirstOrDefault(u => u.UserName == "peter@uclm.es");

                if (user == null) 
                    logger.LogError("An error occurred seeding Purchases in the Database: user does not exist");
                else
                    SeedCompras(dbContext, user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Purchases (Compras) in the Database.");
            }



        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
    if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
        ApplicationUser user = new ApplicationUser 
        {
            UserName = "elena@uclm.es",
            Email = "elena@uclm.es",
            Nombre = "Elena",
            Apellidos = "Navarro Martínez",
            Direccion = "Dirección de Elena",
            PhoneNumber = "600123456",
            EmailConfirmed = true
        };

        var resultTask = userManager.CreateAsync(user, "Password1234%");
        resultTask.Wait();

        if (resultTask.Result.Succeeded) {
            userManager.AddToRoleAsync(user, roles[0]).Wait();
        }
    }

    if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
        ApplicationUser user = new ApplicationUser 
        {
            UserName = "peter@uclm.es",
            Email = "peter@uclm.es",
            Nombre = "Peter",
            Apellidos = "Jackson",
            Direccion = "Dirección de Peter",
            PhoneNumber = "600654321",
            EmailConfirmed = true
        };

        var resultTask = userManager.CreateAsync(user, "OtherPass12$");
        resultTask.Wait();

        if (resultTask.Result.Succeeded) {
            userManager.AddToRoleAsync(user, roles[2]).Wait();
        }
    }
}

public static void SeedGenerosEditorialesYLibros(ApplicationDbContext dbContext) {
            string[] nombresGeneros = ["Fantasia", "Ciencia Ficcion", "Terror"];
            List<Genero> generos = [];
            Libro libro;

            foreach (string nombreGenero in nombresGeneros) {
                var genero = dbContext.Generos.FirstOrDefault(g => g.Nombre == nombreGenero);
                if (genero == null)
                generos.Add(new Genero { Nombre = nombreGenero });
                else
                    generos.Add(genero);
            }

            string[] nombresEditoriales = ["SM", "Planeta", "Anaya"];
            List<Editorial> editoriales = [];
            
            foreach (string nombreEditorial in nombresEditoriales) {
                var editorial = dbContext.Editoriales.FirstOrDefault(e => e.Nombre == nombreEditorial);
                if (editorial == null)
                    editoriales.Add(new Editorial { Nombre = nombreEditorial });
                else
                    editoriales.Add(editorial);
            }

if (dbContext.Libros.FirstOrDefault(l => l.Titulo == "Las maravillosas aventuras de Chipi") == null) {
    libro = new Libro {
        Titulo = "Las maravillosas aventuras de Chipi",
        Autor = "Sergio Muñon",
        TipoLibro = "Fisico",
        CalificacionMedia = 5.0,
        FechaLanzamiento = new DateTime(2023, 5, 10).ToUniversalTime(),
        PrecioCompra = 15.0m,       
        PrecioReposicion = 10.0m,   
        Stock = 20,                 
        Genero = generos[0],
        Editorial = editoriales[0]
    };
    dbContext.Libros.Add(libro);
}

if (dbContext.Libros.FirstOrDefault(l => l.Titulo == "Wigetta") == null) {
    libro = new Libro {
        Titulo = "Wigetta",
        Autor = "Willyrex",
        TipoLibro = "Bolsillo",
        CalificacionMedia = 4.8,
        FechaLanzamiento = new DateTime(1965, 8, 1).ToUniversalTime(),
        PrecioCompra = 12.5m,       
        PrecioReposicion = 8.0m,    
        Stock = 15,                 
        Genero = generos[1],
        Editorial = editoriales[0]
    };
    dbContext.Libros.Add(libro);
}

if (dbContext.Libros.FirstOrDefault(l => l.Titulo == "El libro troll") == null) {
    libro = new Libro {
        Titulo = "El libro troll",
        Autor = "elrubius",
        TipoLibro = "Tapa dura",
        CalificacionMedia = 4.9,
        FechaLanzamiento = new DateTime(1996, 8, 6).ToUniversalTime(),
        PrecioCompra = 18.0m,       
        PrecioReposicion = 12.0m,   
        Stock = 25,                 
        Genero = generos[0],
        Editorial = editoriales[1]
    };
    dbContext.Libros.Add(libro);
}

if (dbContext.Libros.FirstOrDefault(l => l.Titulo == "Los Juegos del Hambre") == null) {
    libro = new Libro {
        Titulo = "Los Juegos del Hambre",
        Autor = "Suzanne Collins",
        TipoLibro = "Digital",
        CalificacionMedia = 4.7,
        FechaLanzamiento = new DateTime(1989, 10, 1).ToUniversalTime(),
        PrecioCompra = 20.0m,       
        PrecioReposicion = 14.0m,   
        Stock = 30,                 
        Genero = generos[2],
        Editorial = editoriales[2]
    };
    dbContext.Libros.Add(libro);
}

            dbContext.SaveChanges();
            
}

public static void SeedResenas(ApplicationDbContext dbcontext, ApplicationUser user) {
            var libros = dbcontext.Libros.Take(4).ToList();

            if (libros.Count >= 4) {
                if (dbcontext.Resenas.FirstOrDefault(p => p.Titulo == "Excelente libro!") == null) {
                    var resena = new Resena(0, "Excelente libro!", new DateTime(2023, 10, 12).ToUniversalTime(), user, new List<ResenaItem>());
                    resena.ResenaItems.Add(new ResenaItem(5, "Una obra maestra indiscutible, me ha encantado.", libros[0].Id, libros[0], resena.Id, resena));
                    dbcontext.Resenas.Add(resena);
                }

                if (dbcontext.Resenas.FirstOrDefault(p => p.Titulo == "Muy recomendable") == null) {
                    var resena = new Resena(0, "Muy recomendable", new DateTime(2023, 11, 5).ToUniversalTime(), user, new List<ResenaItem>());
                    resena.ResenaItems.Add(new ResenaItem(4, "Muy entretenido y divertido para pasar el rato.", libros[1].Id, libros[1], resena.Id, resena));
                    dbcontext.Resenas.Add(resena);
                }

                if (dbcontext.Resenas.FirstOrDefault(p => p.Titulo == "No me ha gustado") == null) {
                    var resena = new Resena(0, "No me ha gustado", new DateTime(2024, 1, 20).ToUniversalTime(), user, new List<ResenaItem>());
                    resena.ResenaItems.Add(new ResenaItem(2, "Esperaba algo diferente, no conecté con la trama.", libros[2].Id, libros[2], resena.Id, resena));
                    dbcontext.Resenas.Add(resena);
                }

                if (dbcontext.Resenas.FirstOrDefault(p => p.Titulo == "Lectura aburrida") == null) {
                    var resena = new Resena(0, "Lectura aburrida", new DateTime(2024, 2, 14).ToUniversalTime(), user, new List<ResenaItem>());
                    resena.ResenaItems.Add(new ResenaItem(1, "Se me hizo muy pesado de leer desde el principio.", libros[3].Id, libros[3], resena.Id, resena));
                    dbcontext.Resenas.Add(resena);
                }

                dbcontext.SaveChanges();
            }
        }


    //SeedCompras: este metodo añade datos de prueba para la entidad Compra y CompraItem.
    //Se asume que ya existen libros en la base de datos y un usuario con el que se asociarán las compras.
    
    public static void SeedCompras(ApplicationDbContext dbContext, ApplicationUser user) {
    var libros = dbContext.Libros.Take(4).ToList();

    if (libros.Count >= 4) {
        // 1. Insertar 4 filas/tuplas de Compra con sus respectivos CompraItem
        if (!dbContext.Compras.Any()) {
            

            // cargo los metodos de pago que ya existen en la BD por su Id
            // 1 = GooglePay, 2 = PayPal, 3 = Visa

            // 1. Buscamos en la base de datos el primer registro guardado de cada tipo de pago (GooglePay, PayPal y Visa)
            var metodoPago1 = dbContext.Set<MetodoPago>().OfType<GooglePay>().FirstOrDefault();
            var metodoPago2 = dbContext.Set<MetodoPago>().OfType<PayPal>().FirstOrDefault();   
            var metodoPago3 = dbContext.Set<MetodoPago>().OfType<Visa>().FirstOrDefault();

            // 2. Comprobamos que existan los tres; si falta cualquiera de ellos, lanzamos una excepción para avisar del error
            if (metodoPago1 == null || metodoPago2 == null || metodoPago3 == null)
                throw new InvalidOperationException("Los metodos de pago 1, 2 y 3 no existen todavia.");


            //Compras asociadas a los metodos de pago
            var compra1 = new Compra {
                FechaCompra = new DateTime(2023, 10, 15).ToUniversalTime(),
                PrecioTotal = 30.0,
                CodigoDescuento = "DESC10", // Entre 5 y 10 caracteres
                MetodoPago = metodoPago1, //GooglePay 
                Usuario = user,
                CompraItems = new List<CompraItem>()
            };
            compra1.CompraItems.Add(new CompraItem { Cantidad = 2, LibroId = libros[0].Id, Libro = libros[0], Compra = compra1 });
            dbContext.Compras.Add(compra1);

            var compra2 = new Compra {
                FechaCompra = new DateTime(2023, 11, 20).ToUniversalTime(),
                PrecioTotal = 45.0,
                CodigoDescuento = "PROMO5",
                MetodoPago = metodoPago2, //Paypal 
                Usuario = user,
                CompraItems = new List<CompraItem>()
            };
            compra2.CompraItems.Add(new CompraItem { Cantidad = 3, LibroId = libros[1].Id, Libro = libros[1], Compra = compra2 });
            dbContext.Compras.Add(compra2);

            var compra3 = new Compra {
                FechaCompra = new DateTime(2024, 1, 10).ToUniversalTime(),
                PrecioTotal = 20.0,
                CodigoDescuento = "OFERTA",
                MetodoPago = metodoPago3, //Visa 
                Usuario = user,
                CompraItems = new List<CompraItem>()
            };
            compra3.CompraItems.Add(new CompraItem { Cantidad = 2, LibroId = libros[2].Id, Libro = libros[2], Compra = compra3 });
            dbContext.Compras.Add(compra3);

            var compra4 = new Compra {
                FechaCompra = new DateTime(2024, 2, 5).ToUniversalTime(),
                PrecioTotal = 60.0,
                CodigoDescuento = "VERANO",
                MetodoPago = metodoPago1, //GooglePay
                Usuario = user,
                CompraItems = new List<CompraItem>()
            };
            compra4.CompraItems.Add(new CompraItem { Cantidad = 4, LibroId = libros[3].Id, Libro = libros[3], Compra = compra4 });
            dbContext.Compras.Add(compra4);

            dbContext.SaveChanges();
        }
    }
}


public static void SeedReposiciones(ApplicationDbContext dbContext, ApplicationUser user)
{
    var libros = dbContext.Libros.Take(4).ToList();

    if (libros.Count >= 4)
    {
        if (!dbContext.Reposiciones.Any())
        {
            // --- Métodos de pago ---
            var metodoPagoVisa = new Visa
            {
                NumeroTarjeta = "12345678",
                FechaCaducidad = new DateTime(2028, 12, 31).ToUniversalTime()
            };

            var metodoPagoPaypal = new PayPal
            {
                NumeroTelefono = "655322368"
            };

            var metodoPagoGooglePay = new GooglePay
            {
                Email = "chipi@uclm.es"
            };

            var metodoPagoVisa2 = new Visa
            {
                NumeroTarjeta = "87654321",
                FechaCaducidad = new DateTime(2027, 6, 30).ToUniversalTime()
            };

            // --- Ejemplo 1: Reposición con Visa ---
            var reposicion1 = new Reposicion
            {
                FechaReposicion = new DateTime(2024, 3, 1).ToUniversalTime(),
                PrecioTotal = 150.0m,
                Usuario = user,
                MetodosPago = new List<MetodoPago> { metodoPagoVisa },
                ReposicionItems = new List<ReposicionItem>()
            };

            reposicion1.ReposicionItems.Add(new ReposicionItem
            {
                CantidadReposicion = 10,
                LibroId = libros[0].Id,
                Libro = libros[0],
                Reposicion = reposicion1
            });

            reposicion1.ReposicionItems.Add(new ReposicionItem
            {
                CantidadReposicion = 5,
                LibroId = libros[1].Id,
                Libro = libros[1],
                Reposicion = reposicion1
            });

            dbContext.Reposiciones.Add(reposicion1);

            // --- Ejemplo 2: Reposición con PayPal ---
            var reposicion2 = new Reposicion
            {
                FechaReposicion = new DateTime(2024, 3, 15).ToUniversalTime(),
                PrecioTotal = 80.0m,
                Usuario = user,
                MetodosPago = new List<MetodoPago> { metodoPagoPaypal },
                ReposicionItems = new List<ReposicionItem>()
            };

            reposicion2.ReposicionItems.Add(new ReposicionItem
            {
                CantidadReposicion = 8,
                LibroId = libros[0].Id,
                Libro = libros[0],
                Reposicion = reposicion2
            });

            dbContext.Reposiciones.Add(reposicion2);

            // --- Ejemplo 3: Reposición con GooglePay ---
            var reposicion3 = new Reposicion
            {
                FechaReposicion = new DateTime(2024, 4, 2).ToUniversalTime(),
                PrecioTotal = 120.50m,
                Usuario = user,
                MetodosPago = new List<MetodoPago> { metodoPagoGooglePay },
                ReposicionItems = new List<ReposicionItem>()
            };

            reposicion3.ReposicionItems.Add(new ReposicionItem
            {
                CantidadReposicion = 12,
                LibroId = libros[2].Id,
                Libro = libros[2],
                Reposicion = reposicion3
            });

            dbContext.Reposiciones.Add(reposicion3);

            // --- Ejemplo 4: Reposición variada con múltiples ítems (Visa) ---
            var reposicion4 = new Reposicion
            {
                FechaReposicion = new DateTime(2024, 4, 18).ToUniversalTime(),
                PrecioTotal = 210.0m,
                Usuario = user,
                MetodosPago = new List<MetodoPago> { metodoPagoVisa2 },
                ReposicionItems = new List<ReposicionItem>()
            };

            reposicion4.ReposicionItems.Add(new ReposicionItem
            {
                CantidadReposicion = 15,
                LibroId = libros[1].Id,
                Libro = libros[1],
                Reposicion = reposicion4
            });

            reposicion4.ReposicionItems.Add(new ReposicionItem
            {
                CantidadReposicion = 6,
                LibroId = libros[3].Id,
                Libro = libros[3],
                Reposicion = reposicion4
            });

            dbContext.Reposiciones.Add(reposicion4);

            dbContext.SaveChanges();
        }
    }
}
    }
}