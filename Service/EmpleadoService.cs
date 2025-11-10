using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;

namespace SoftWC.Service
{
    public class EmpleadoService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserService _userService;
        public EmpleadoService(ApplicationDbContext context, UserManager<Usuario> userManager, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<IEnumerable<Usuario>> GetEmpleados()
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();
            var usuarios = await _userManager.Users.ToListAsync();
            
            var rolesUsuarioActual = await _userManager.GetRolesAsync(userPrincipal);

            if (rolesUsuarioActual.Contains("Administrador"))
            {
                return await FiltrarUsuariosPorRoles(usuarios, ["Administrador", "Controltotal"]);
            }
            else if (rolesUsuarioActual.Contains("Controltotal"))
            {
                // Si es Controltotal, devuelve todos excepto Administradores
                return usuarios;
            }
            else
            {
                return GetEmpleadosOfSuper(userPrincipal.Id).Result;
            }
        }

        public async Task<IEnumerable<Usuario>> GetEmpleadosOfSuper(string id)
        {
            var supervisiones = await _context.Supervision
                .Where(s => s.SupervisorId == id)
                .ToListAsync();

            var empleadosIds = supervisiones.Select(s => s.EmpleadoId).ToList();
            var empleados = await _context.Users
                .Where(u => empleadosIds.Contains(u.Id))
                .ToListAsync();
            return empleados;
        }

        // Método auxiliar para filtrar usuarios por roles a excluir
        private async Task<List<Usuario>> FiltrarUsuariosPorRoles(List<Usuario> usuarios, string[] rolesExcluir)
        {
            var usuariosFiltrados = new List<Usuario>();
            
            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                if (!roles.Any(r => rolesExcluir.Contains(r)))
                {
                    usuariosFiltrados.Add(usuario);
                }
            }
            
            return usuariosFiltrados;
        }

        public async Task<List<Usuario>> ObtenerEmpleadosAsync()
        {
            return await _context.Usuario
                .Where(u => _context.UserRoles.Any(ur => 
                    ur.UserId == u.Id && 
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Empleado")))
                .OrderBy(u => u.Apellido)
                .ThenBy(u => u.Nombre)
                .ToListAsync();
        }
    }
}