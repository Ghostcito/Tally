using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SoftWC.Controllers
{
    [Authorize]

    public class SedeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public SedeController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Sede By Cliente Id
        [HttpGet("Sede/Index/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var applicationDbContext = _context.Sede.Include(s => s.Cliente).Where(s => s.ClienteId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet("Sede/Index")]
        public async Task<IActionResult> Index(string searchNombre, string searchCiudad, string searchProvincia)
        {
            var query = _context.Sede
                .Include(s => s.Cliente)
                .Include(s => s.Usuarios)
                .AsQueryable();

            // Filtros case-insensitive con ILike (PostgreSQL/SQL Server)
            if (!string.IsNullOrEmpty(searchNombre))
            {
                query = query.Where(s => EF.Functions.ILike(s.Nombre_local, $"%{searchNombre}%"));
            }

            if (!string.IsNullOrEmpty(searchCiudad))
            {
                query = query.Where(s => EF.Functions.ILike(s.Ciudad, $"%{searchCiudad}%"));
            }

            if (!string.IsNullOrEmpty(searchProvincia))
            {
                query = query.Where(s => EF.Functions.ILike(s.Provincia, $"%{searchProvincia}%"));
            }

            ViewBag.CurrentFilterNombre = searchNombre;
            ViewBag.CurrentFilterCiudad = searchCiudad;
            ViewBag.CurrentFilterProvincia = searchProvincia;

            return View(await query.ToListAsync());
        }

        // GET: Sede/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(m => m.SedeId == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        // GET: Sede/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "Nombre");
            return View();
        }

        // POST: Sede/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SedeId,ClienteId,Nombre_local,Direccion_local,Ciudad,Provincia,Latitud,Longitud,estadoSede")] Sede sede)
        {
            if (ModelState.IsValid)
            {
                sede.Radio = 200; // Asignar un radio por defecto de 80 metros
                _context.Add(sede);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "ClienteId", sede.ClienteId);
            return View(sede);
        }

        // GET: Sede/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede.FindAsync(id);
            if (sede == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "ClienteId", sede.ClienteId);
            return View(sede);
        }

        // POST: Sede/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SedeId,ClienteId,Nombre_local,Direccion_local,Ciudad,Provincia,Latitud,Longitud,estadoSede")] Sede sede)
        {
            if (id != sede.SedeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sede);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SedeExists(sede.SedeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "ClienteId", sede.ClienteId);
            return View(sede);
        }

        // GET: Sede/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(m => m.SedeId == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        // POST: Sede/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sede = await _context.Sede.FindAsync(id);
            if (sede != null)
            {
                _context.Sede.Remove(sede);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SedeExists(int id)
        {
            return _context.Sede.Any(e => e.SedeId == id);
        }

        public async Task<IActionResult> AsignarUsuarios(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede
                .Include(s => s.Usuarios)
                .FirstOrDefaultAsync(s => s.SedeId == id);

            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        [HttpGet("sedes-disponibles")]
        public async Task<IActionResult> GetSedesDisponibles([FromQuery] string filtro = "", [FromQuery] string usuarioId = "")
        {
            var query = _context.Sede.AsQueryable();

            // Aplicar filtro si existe
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(s =>
                    s.Nombre_local.Contains(filtro) ||
                    s.Ciudad.Contains(filtro) ||
                    s.Direccion_local.Contains(filtro));
            }

            // Excluir sedes ya asignadas al usuario
            if (!string.IsNullOrEmpty(usuarioId))
            {
                var sedesAsignadas = _context.Sede
                    .Where(s => s.Usuarios.Any(u => u.Id == usuarioId))
                    .Select(s => s.SedeId);

                query = query.Where(s => !sedesAsignadas.Contains(s.SedeId));
            }

            var sedes = await query
                .Select(s => new
                {
                    id = s.SedeId,
                    nombre = s.Nombre_local,
                    ciudad = s.Ciudad,
                    direccion = s.Direccion_local
                })
                .ToListAsync();

            return Ok(sedes);
        }

        // POST: api/sedes/{sedeId}/asignar-usuarios
        [HttpPost("{sedeId}/asignar-usuarios")]
        public async Task<IActionResult> AsignarUsuariosASede(int sedeId, [FromBody] List<string> usuarioIds)
        {
            // Validar que se proporcionaron IDs de usuario
            if (usuarioIds == null || !usuarioIds.Any())
            {
                return BadRequest("Debe proporcionar al menos un ID de usuario");
            }

            // Obtener la sede con sus usuarios actuales
            var sede = await _context.Sede
                .Include(s => s.Usuarios)
                .FirstOrDefaultAsync(s => s.SedeId == sedeId);

            if (sede == null)
            {
                return NotFound($"Sede con ID {sedeId} no encontrada");
            }

            // Obtener los usuarios solicitados
            var usuariosExistentes = await _userManager.Users
                .Where(u => usuarioIds.Contains(u.Id))
                .ToListAsync();

            // Validar que todos los usuarios existen
            if (usuariosExistentes.Count != usuarioIds.Count)
            {
                var usuariosNoEncontrados = usuarioIds.Except(usuariosExistentes.Select(u => u.Id));
                return BadRequest($"Los siguientes usuarios no existen: {string.Join(", ", usuariosNoEncontrados)}");
            }

            // Validar que todos los usuarios tienen el rol EMPLEADO
            var usuariosNoEmpleados = new List<string>();

            foreach (var usuario in usuariosExistentes)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                if (!roles.Contains("Empleado"))
                {
                    usuariosNoEmpleados.Add(usuario.UserName);
                }
            }

            if (usuariosNoEmpleados.Any())
            {
                return BadRequest($"Los siguientes usuarios no tienen el rol EMPLEADO: {string.Join(", ", usuariosNoEmpleados)}");
            }

            // Asignar usuarios a la sede (evitando duplicados)
            foreach (var usuario in usuariosExistentes)
            {
                if (!sede.Usuarios.Any(u => u.Id == usuario.Id))
                {
                    sede.Usuarios.Add(usuario);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Success = true,
                    Message = "Usuarios asignados correctamente",
                    SedeId = sede.SedeId,
                    UsuariosAsignados = usuariosExistentes.Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Email
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error interno al procesar la solicitud",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("sede/{sedeId}/empleados-disponibles")]
        public async Task<IActionResult> EmpleadosDisponibles(string filtro, int sedeId)
        {
            try
            {
                // Usuarios ya asignados a esta sede
                var usuariosAsignados = await _context.Sede
                    .Where(s => s.SedeId == sedeId)
                    .SelectMany(s => s.Usuarios)
                    .Select(u => u.Id)
                    .ToListAsync();

                // Base query
                var query = _userManager.Users
                    .Where(u => !usuariosAsignados.Contains(u.Id));

                // Aplicar filtro insensible a mayúsculas/minúsculas
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtro = filtro.ToLower(); // Normalizar a minúsculas
                    query = query.Where(u =>
                        u.Nombre.ToLower().Contains(filtro) ||
                        u.Apellido.ToLower().Contains(filtro) ||
                        u.UserName.ToLower().Contains(filtro) ||
                        u.Email.ToLower().Contains(filtro));
                }

                // Obtener usuarios con rol "Empleado"
                var empleados = new List<dynamic>();
                foreach (var usuario in await query.ToListAsync())
                {
                    if (await _userManager.IsInRoleAsync(usuario, "Empleado"))
                    {
                        empleados.Add(new
                        {
                            id = usuario.Id,
                            nombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                            userName = usuario.UserName,
                            email = usuario.Email
                        });
                    }
                }

                return Ok(empleados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoverUsuarioDeSede(int sedeId, string usuarioId)
        {
            try
            {
                var sede = await _context.Sede
                    .Include(s => s.Usuarios)
                    .FirstOrDefaultAsync(s => s.SedeId == sedeId);

                if (sede == null)
                    return NotFound($"Sede con ID {sedeId} no encontrada");

                var usuario = sede.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
                if (usuario == null)
                    return BadRequest("El usuario no está asignado a esta sede");

                sede.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Usuario {usuario.UserName} removido de la sede {sede.Nombre_local}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
