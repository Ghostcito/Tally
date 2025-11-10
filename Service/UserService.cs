using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;

namespace SoftWC.Service
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext context, UserManager<Usuario> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<Usuario?> FindByDniAsync(string dni)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.DNI == dni);
        }

        public async Task<bool> IsDniAvailableAsync(string dni)
        {
            return !await _context.Users
                .AnyAsync(u => u.DNI == dni);
        }


        // Método para obtener todos los usuarios registrados
        public async Task<List<Usuario>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<List<Usuario>> GetAllUsersByRol()
        {
            var userPrincipal = _httpContextAccessor.HttpContext?.User;
            var usuario = await _userManager.GetUserAsync(userPrincipal);
            var usuarios = await _userManager.Users.ToListAsync();

            if (_userManager.GetRolesAsync(usuario).Result.Contains("Administrador"))
            {
                return usuarios.Where(u => !_userManager.GetRolesAsync(u).Result.Contains("Administrador")).ToList();
            }
            else
            {
                return usuarios.Where(u => _userManager.GetRolesAsync(u).Result.Contains("Empleado")).ToList();
            }

        }

        //Metodo para obtener usuario logeado
        public async Task<Usuario> GetCurrentUserAsync()
        {
            var userPrincipal = _httpContextAccessor.HttpContext?.User;
            return await _userManager.GetUserAsync(userPrincipal);
        }
        //Metodo para obtener rol de usuario logeado
        public Task<IList<string>> GetRolCurrentUserAsync()
        {
            var userPrincipal = GetCurrentUserAsync().Result;
            return _userManager.GetRolesAsync(userPrincipal);
        }

        // Método para obtener un usuario por su ID
        public async Task<Usuario> GetUserByIdAsync(string id)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }





        // Método para obtener usuarios con información de roles
        // public async Task<List<UsuarioConRolViewModel>> GetAllUsersWithRolesAsync()
        // {
        //     var usuarios = await _userManager.Users.ToListAsync();
        //     var usuariosConRoles = new List<UsuarioConRolViewModel>();

        //     foreach (var usuario in usuarios)
        //     {
        //         var roles = await _userManager.GetRolesAsync(usuario);
        //         usuariosConRoles.Add(new UsuarioConRolViewModel
        //         {
        //             Usuario = usuario,
        //             Roles = roles.ToList()
        //         });
        //     }

        //     return usuariosConRoles;
        // }

    }
}