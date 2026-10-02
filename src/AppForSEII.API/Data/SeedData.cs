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
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }


            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
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
        Genero = generos[0],
        Editorial = editoriales[0]
    };
    dbContext.Libros.Add(libro);
}

if (dbContext.Libros.FirstOrDefault(l => l.Titulo == "Dune") == null) {
    libro = new Libro {
        Titulo = "Dune",
        Autor = "Frank Herbert",
        TipoLibro = "Bolsillo",
        CalificacionMedia = 4.8,
        FechaLanzamiento = new DateTime(1965, 8, 1).ToUniversalTime(),
        Genero = generos[1],
        Editorial = editoriales[0]
    };
    dbContext.Libros.Add(libro);
}

if (dbContext.Libros.FirstOrDefault(l => l.Titulo == "Juego de Tronos") == null) {
    libro = new Libro {
        Titulo = "Juego de Tronos",
        Autor = "George R. R. Martin",
        TipoLibro = "Tapa dura",
        CalificacionMedia = 4.9,
        FechaLanzamiento = new DateTime(1996, 8, 6).ToUniversalTime(),
        Genero = generos[0],
        Editorial = editoriales[1]
    };
    dbContext.Libros.Add(libro);
}

if (dbContext.Libros.FirstOrDefault(l => l.Titulo == "Los Pilares de la Tierra") == null) {
    libro = new Libro {
        Titulo = "Los Pilares de la Tierra",
        Autor = "Ken Follett",
        TipoLibro = "Digital",
        CalificacionMedia = 4.7,
        FechaLanzamiento = new DateTime(1989, 10, 1).ToUniversalTime(),
        Genero = generos[2],
        Editorial = editoriales[2]
    };
    dbContext.Libros.Add(libro);
}

            dbContext.SaveChanges();
            
}

    }
}