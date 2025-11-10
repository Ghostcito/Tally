using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using SoftWC.Models;

namespace SoftWC.Data
{
    public static class UserDataInitializer
    {
        public static async Task SeedData2(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            var users = new List<(string Email, string Password, string Role, Usuario User)>
            {

                ("ruth.torocahua@gmail.com", "Empleado121*@", "Empleado", new Usuario
                {
                    UserName = "RUTHWC",
                    Email = "ruth.torocahua@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "RUTH",
                    Apellido = "TOROCAHUA HUAMANVILCA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "42425474"
                }),

                ("marilda.torocahua@gmail.com", "Empleado122*@", "Empleado", new Usuario
                {
                    UserName = "MARILDAWC",
                    Email = "marilda.torocahua@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "MARILDA",
                    Apellido = "TOROCAHUA HUAMANVILCA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "41095019"
                }),

                ("martha.choque@gmail.com", "Empleado123*@", "Empleado", new Usuario
                {
                    UserName = "MARTHAWC",
                    Email = "martha.choque@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "MARTHA",
                    Apellido = "CHOQUE TRUJILLANO",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "29594503"
                }),

                ("rogelia.alhuirca@gmail.com", "Empleado124*@", "Empleado", new Usuario
                {
                    UserName = "ROGELIAWC",
                    Email = "rogelia.alhuirca@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "ROGELIA",
                    Apellido = "ALHUICRCA LLACMA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "41155258"
                }),

                ("julia.calachua@gmail.com", "Empleado125*@", "Empleado", new Usuario
                {
                    UserName = "JULIAWC",
                    Email = "julia.calachua@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "JULIA",
                    Apellido = "CALACHUA CONDORI",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "42430147"
                }),

                ("celia.huamani@gmail.com", "Empleado126*@", "Empleado", new Usuario
                {
                    UserName = "CELIAWC",
                    Email = "celia.huamani@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "CELIA",
                    Apellido = "HUAMANI VERA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "41580899"
                }),

                ("july.tapayuri@gmail.com", "Empleado127*@", "Empleado", new Usuario
                {
                    UserName = "JULYWC",
                    Email = "july.tapayuri@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "JULY",
                    Apellido = "TAPAYURI YUMBATO",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "43526049"
                }),

                ("carla.taco@gmail.com", "Empleado128*@", "Empleado", new Usuario
                {
                    UserName = "CARLAWC",
                    Email = "carla.taco@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "CARLA",
                    Apellido = "TACO CABANA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "40216288"
                }),

                ("alisson.mendoza@gmail.com", "Empleado129*@", "Empleado", new Usuario
                {
                    UserName = "ALISSONWC",
                    Email = "alisson.mendoza@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "ALISSON",
                    Apellido = "MENDOZA MAMANI",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "76772867"
                }),

                ("nancy.condori@gmail.com", "Empleado131@", "Empleado", new Usuario
                {
                    UserName = "NANCYWC",
                    Email = "nancy.condori@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "NANCY",
                    Apellido = "CONDORI ITO",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "73492235"
                }),

                ("reyna.acero@gmail.com", "Empleado132*@", "Empleado", new Usuario
                {
                    UserName = "REYNAWC",
                    Email = "reyna.acero@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "REYNA",
                    Apellido = "ACERO RAMOS",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "41821304"
                }),

                ("delia.acero@gmail.com", "Empleado133*@", "Empleado", new Usuario
                {
                    UserName = "DELIAWC",
                    Email = "delia.acero@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "DELIA",
                    Apellido = "ACERO RAMOS",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "42500840"
                }),

                ("mercedes.rojas@gmail.com", "Empleado134*@", "Empleado", new Usuario
                {
                    UserName = "MERCEDESWC",
                    Email = "mercedes.rojas@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "MERCEDES",
                    Apellido = "ROJAS SOTO",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "OO102716"
                }),

                ("shirley.nina@gmail.com", "Empleado135*@", "Empleado", new Usuario
                {
                    UserName = "SHIRLEYWC",
                    Email = "shirley.nina@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "SHIRLEY",
                    Apellido = "NINA CHAVEZ",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "60062615"
                }),

                ("marlene.santos@gmail.com", "Empleado136*@", "Empleado", new Usuario
                {
                    UserName = "MARLENEWC",
                    Email = "marlene.santos@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "MARLENE",
                    Apellido = "SANTOS CHINCHAY",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "42686571"
                }),

                ("roxana.chavez@gmail.com", "Empleado137*@", "Empleado", new Usuario
                {
                    UserName = "ROXANAWC",
                    Email = "roxana.chavez@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "ROXANA",
                    Apellido = "CHAVEZ LACASTRO LEONOR",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "46516695"
                }),

                ("maryori.montenegro@gmail.com", "Empleado138*@", "Empleado", new Usuario
                {
                    UserName = "MARYORIWC",
                    Email = "maryori.montenegro@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "MARYORI",
                    Apellido = "MONTENEGRO MENDOZA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "76819477"
                }),

                ("reyna.sanca@gmail.com", "Empleado139*@", "Empleado", new Usuario
                {
                    UserName = "REYNASCWC",
                    Email = "reyna.sanca@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "REYNA",
                    Apellido = "SANCA CRUZ",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "42531143"
                }),

                ("delia.quispe@gmail.com", "Empleado141*@", "Empleado", new Usuario
                {
                    UserName = "DELIAQUISPE",
                    Email = "delia.quispe@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "DELIA",
                    Apellido = "CONDORI QUISPE",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "44997761"
                }),

                ("victoria.mendizabal@gmail.com", "Empleado142*@", "Empleado", new Usuario
                {
                    UserName = "VICTORIAMAMANI",
                    Email = "victoria.mendizabal@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "VICTORIA",
                    Apellido = "MENDIZABAL MAMANI",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "29724067"
                }),

                ("nadia.nina@gmail.com", "Empleado143*@", "Empleado", new Usuario
                {
                    UserName = "NADIANINA",
                    Email = "nadia.nina@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "NADIA",
                    Apellido = "NINA NINA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "43507545"
                }),

                ("brigida.diaz@gmail.com", "Empleado144*@", "Empleado", new Usuario
                {
                    UserName = "BRIGIDADIAZ",
                    Email = "brigida.diaz@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "BRIGIDA",
                    Apellido = "DIAZ HUANCA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "01552993"
                }),

                ("chela.diaz@gmail.com", "Empleado145*@", "Empleado", new Usuario
                {
                    UserName = "CHELAHUANCA",
                    Email = "chela.diaz@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "CHELA",
                    Apellido = "DIAZ HUANCA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "01499534"
                }),

                ("gino.arias@gmail.com", "Empleado146*@", "Empleado", new Usuario
                {
                    UserName = "GINOARIAS",
                    Email = "gino.arias@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "GINO",
                    Apellido = "ARIAS LLERENA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "75970809"
                }),

                ("yani.huayna@gmail.com", "Empleado147*@", "Empleado", new Usuario
                {
                    UserName = "YANIHUAYNA",
                    Email = "yani.huayna@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "YANI",
                    Apellido = "HUANCA HUAYNA",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "46355025"
                }),

                ("edith.panihuara@gmail.com", "Empleado148*@", "Empleado", new Usuario
                {
                    UserName = "EDITHPANIHUARA",
                    Email = "edith.panihuara@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "EDITH",
                    Apellido = "PANIHUARA SACSI",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "84317651"
                }),

                ("reyna.sanca@gmail.com", "Empleado149*@", "Empleado", new Usuario
                {
                    UserName = "REYNASCWC",
                    Email = "reyna.sanca@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "REYNA",
                    Apellido = "SANCA CRUZ",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "42531143"
                })
            };

            foreach (var (email, password, role, user) in users)
            {
                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser == null)
                {
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Code} - {error.Description}");
                        }
                    }
                }
            }

            // Crear el usuario con rol admin si no existe
            /* string roleName = "Administrador";
             string adminEmail = "admin@gmail.com";
             string adminPassword = "Admin123*@";

             var adminUser = await userManager.FindByEmailAsync(adminEmail);
             if (adminUser == null)
             {
                 adminUser = new Usuario
                 {
                     UserName = "Administradorwc",
                     Email = adminEmail,
                     EmailConfirmed = true,
                     Nombre = "Administrador wc",
                     Apellido = "Sistema",
                     NivelAcceso = "3", // Nivel de acceso para el administrador varia en 1 - 2 - 3
                     Estado = "activo",
                     DNI = "12345678"
                 };

                 var result = await userManager.CreateAsync(adminUser, adminPassword);
                 if (result.Succeeded)
                 {
                     await userManager.AddToRoleAsync(adminUser, roleName);
                 }
                 else
                 {
                     foreach (var error in result.Errors)
                     {
                         Console.WriteLine($"Error: {error.Code} - {error.Description}");
                     }
                 }
             }*/
        }
    }
}