using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using SoftWC.Models;

namespace SoftWC.Data
{
    public static class newRolesDataInitializer
    {
        public static async Task SeedData4(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = new[] { "Administrador", "Supervisor", "Empleado", "Contador", "Controltotal" };

            // Crear los roles si no existen
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var users = new List<(string Email, string Password, string Role, Usuario User)>
            {
                // ("admin@gmail.com", "Admin123*@", "Administrador", new Usuario
                // {
                //     UserName = "Administradorwc",
                //     Email = "admin@gmail.com",
                //     EmailConfirmed = true,
                //     Nombre = "Administrador",
                //     Apellido = "NaN",
                //     NivelAcceso = "3",
                //     Estado = "activo",
                //     DNI = "12345678"
                // }),
                // ("supervisor@gmail.com", "Supervisor123*@", "Supervisor", new Usuario
                // {
                //     UserName = "Supervisorwc",
                //     Email = "supervisor@gmail.com",
                //     EmailConfirmed = true,
                //     Nombre = "Supervisor",
                //     Apellido = "NaN",
                //     NivelAcceso = "2",
                //     Estado = "activo",
                //     DNI = "87654321"
                // }),
                // ("empleado@gmail.com", "Empleado123*@", "Empleado", new Usuario
                // {
                //     UserName = "Empleadowc",
                //     Email = "empleado@gmail.com",
                //     EmailConfirmed = true,
                //     Nombre = "Empleado",
                //     Apellido = "NaN",
                //     NivelAcceso = "1",
                //     Estado = "activo",
                //     DNI = "11223344"
                // }),

                //añadir nuevos usuarios aquí
                ("Maria_Elizabeth2@gmail.com", "Contador123*@", "Contador", new Usuario
                {
                    UserName = "MARIA ELIZABETH",
                    Email = "Maria_Elizabeth2@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "MARIA ELIZABETH",
                    Apellido = "NAVARRO CRUZ",
                    NivelAcceso = "1",
                    Estado = "activo",
                    DNI = "76312345"
                }),
                ("adminCTWC12@gmail.com", "ControlTotal123*@", "ControlTotal", new Usuario
                {
                    UserName = "AdministradorCT",
                    Email = "adminCTWC12@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "Administrador CT",
                    Apellido = "Sistema",
                    NivelAcceso = "3",
                    Estado = "activo",
                    DNI = "25111234"
                }),

                //Supervisor 2
                ("juanpablobustamante@gmail.com", "Supervisor12345*@", "Supervisor", new Usuario
                {
                    UserName = "JUAN BUSTAMANTE",
                    Email = "juanpablobustamante@gmail.com",
                    EmailConfirmed = true,
                    Nombre = "JUAN PABLO",
                    Apellido = "BUSTAMANTE RIVERA",
                    NivelAcceso = "2",
                    Estado = "activo",
                    DNI = "41315678"
                }),
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