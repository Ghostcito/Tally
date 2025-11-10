using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;
using Microsoft.AspNetCore.Identity;

namespace SoftWC.Controllers
{
    public class SupervisionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Supervision
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Supervision.Include(s => s.Empleado).Include(s => s.Supervisor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Supervision/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .Include(s => s.Empleado)
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        // GET: Supervision/Create
        public async Task<IActionResult> Create()
        {
            // Obtener el ID del rol "Empleado"
            var empleadoRole = await _context.Roles
                .Where(r => r.Name == "Empleado")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Obtener los IDs de los usuarios que tienen el rol "Administrador"
            var adminUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == empleadoRole)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Crear SelectList con Nombre + Apellido solo para administradores
            ViewData["EmpleadoId"] = new SelectList(
                _context.Users
                    .Where(u => adminUserIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    }),
                "Id", "NombreCompleto");

            // Obtener el ID del rol "Empleado"
            var supervisorRole = await _context.Roles
                .Where(r => r.Name == "Supervisor")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Obtener los IDs de los usuarios que tienen el rol "Administrador"
            var supervisorUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == supervisorRole)
                .Select(ur => ur.UserId)
                .ToListAsync();


            // Supervisores: todos los usuarios
            ViewData["SupervisorId"] = new SelectList(
                _context.Users
                    .Where(u => supervisorUserIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    }),
                "Id", "NombreCompleto");

            return View();
        }

        // POST: Supervision/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupervisorId,EmpleadoId,FechaInicio,FechaFin")] Supervision supervision)
        {
            // 1. Remover validaciones automáticas no deseadas
            ModelState.Remove("Empleado");
            ModelState.Remove("Supervisor");

            // Conversión de FechaInicio a UTC (si tiene valor)
            if (!supervision.FechaInicio.Equals(""))
            {
                supervision.FechaInicio = DateTime.SpecifyKind(supervision.FechaInicio, DateTimeKind.Utc);
            }

            // Conversión de FechaFin a UTC (si tiene valor)
            if (supervision.FechaFin.HasValue)
            {
                supervision.FechaFin = DateTime.SpecifyKind(supervision.FechaFin.Value, DateTimeKind.Utc);
            }

            // 2. Validación básica de campos requeridos
            if (string.IsNullOrEmpty(supervision.EmpleadoId))
            {
                ModelState.AddModelError("EmpleadoId", "Debe seleccionar un empleado");
            }

            if (string.IsNullOrEmpty(supervision.SupervisorId))
            {
                ModelState.AddModelError("SupervisorId", "Debe seleccionar un supervisor");
            }

            // 3. Validación para evitar misma persona como supervisor y empleado
            if (supervision.SupervisorId == supervision.EmpleadoId)
            {
                ModelState.AddModelError("EmpleadoId", "El supervisor no puede ser el mismo que el empleado");
            }

            // 4. Validación de duplicados - SOLUCIÓN PRINCIPAL
            if (!string.IsNullOrEmpty(supervision.SupervisorId) && !string.IsNullOrEmpty(supervision.EmpleadoId) && !supervision.FechaInicio.Equals(""))
            {
                bool asignacionDuplicada = await _context.Supervision
                    .AnyAsync(s => s.SupervisorId == supervision.SupervisorId &&
                                s.EmpleadoId == supervision.EmpleadoId &&
                                s.Id != supervision.Id && // Para permitir actualizaciones
                                (
                                    (supervision.FechaFin == null && s.FechaFin == null) || // Ambos sin fecha fin
                                    (supervision.FechaFin == null && s.FechaFin >= supervision.FechaInicio) || // Nueva sin fin, existente con fin
                                    (s.FechaFin == null && supervision.FechaFin >= s.FechaInicio) || // Existente sin fin, nueva con fin
                                    (supervision.FechaFin.HasValue && s.FechaFin.HasValue && 
                                    supervision.FechaInicio <= s.FechaFin && 
                                    supervision.FechaFin >= s.FechaInicio)
                                ));

                if (asignacionDuplicada)
                {
                    var usuarios = await _context.Usuario
                        .Where(u => u.Id == supervision.SupervisorId || u.Id == supervision.EmpleadoId)
                        .Select(u => new { u.Id, u.UserName })
                        .ToListAsync();

                    var supervisor = usuarios.FirstOrDefault(u => u.Id == supervision.SupervisorId)?.UserName ?? "N/A";
                    var empleado = usuarios.FirstOrDefault(u => u.Id == supervision.EmpleadoId)?.UserName ?? "N/A";

                    ModelState.AddModelError("", 
                        $"El supervisor {supervisor} ya está asignado al empleado {empleado} " +
                        $"en el período seleccionado. Por favor elija otro período o diferente asignación.");
                }
            }

            // 5. Resto de validaciones (fechas UTC, existencia en BD, etc.)
            // ... (mantén el resto de tu código existente)

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(supervision);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Manejo de errores
                }
            }

            // Recargar datos para la vista
            ViewData["EmpleadoId"] = new SelectList(_context.Usuario, "Id", "Nombre", supervision.EmpleadoId);
            ViewData["SupervisorId"] = new SelectList(_context.Usuario, "Id", "Nombre", supervision.SupervisorId);
            
            return View(supervision);
        }

        // GET: Supervision/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision.FindAsync(id);
            if (supervision == null)
            {
                return NotFound();
            }
            ViewData["EmpleadoId"] = new SelectList(_context.Usuario, "Id", "UserName", supervision.EmpleadoId);
            ViewData["SupervisorId"] = new SelectList(_context.Usuario, "Id", "UserName", supervision.SupervisorId);
            return View(supervision);
        }

        // POST: Supervision/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SupervisorId,EmpleadoId,FechaInicio,FechaFin")] Supervision supervision)
        {
            // 1. Verificar coincidencia de IDs
            if (id != supervision.Id)
            {
                return NotFound();
            }

            // Conversión de FechaInicio a UTC (si tiene valor)
            if (!supervision.FechaInicio.Equals(""))
            {
                supervision.FechaInicio = DateTime.SpecifyKind(supervision.FechaInicio, DateTimeKind.Utc);
            }

            // Conversión de FechaFin a UTC (si tiene valor)
            if (supervision.FechaFin.HasValue)
            {
                supervision.FechaFin = DateTime.SpecifyKind(supervision.FechaFin.Value, DateTimeKind.Utc);
            }

            // 2. Remover validaciones automáticas no deseadas
            ModelState.Remove("Empleado");
            ModelState.Remove("Supervisor");

            // 3. Validaciones básicas de campos requeridos
            if (string.IsNullOrEmpty(supervision.EmpleadoId))
            {
                ModelState.AddModelError("EmpleadoId", "Debe seleccionar un empleado");
            }

            if (string.IsNullOrEmpty(supervision.SupervisorId))
            {
                ModelState.AddModelError("SupervisorId", "Debe seleccionar un supervisor");
            }

            // 4. Conversión a UTC
            if (supervision.FechaInicio.Equals(""))
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio es requerida");
            }
            else
            {
                supervision.FechaInicio = DateTime.SpecifyKind(supervision.FechaInicio, DateTimeKind.Utc);
            }
            
            if (supervision.FechaFin.HasValue)
            {
                supervision.FechaFin = DateTime.SpecifyKind(supervision.FechaFin.Value, DateTimeKind.Utc);
            }

            // 5. Validación para evitar misma persona como supervisor y empleado
            if (supervision.SupervisorId == supervision.EmpleadoId)
            {
                ModelState.AddModelError("EmpleadoId", "El supervisor no puede ser el mismo que el empleado");
            }

            // 6. Validación de duplicados (excluyendo el registro actual)
            if (!string.IsNullOrEmpty(supervision.SupervisorId) && !string.IsNullOrEmpty(supervision.EmpleadoId) && !supervision.FechaInicio.Equals(""))
            {
                bool asignacionDuplicada = await _context.Supervision
                    .AnyAsync(s => s.SupervisorId == supervision.SupervisorId &&
                                s.EmpleadoId == supervision.EmpleadoId &&
                                s.Id != supervision.Id && // Excluye el registro actual
                                (
                                    (supervision.FechaFin == null && s.FechaFin == null) ||
                                    (supervision.FechaFin == null && s.FechaFin >= supervision.FechaInicio) ||
                                    (s.FechaFin == null && supervision.FechaFin >= s.FechaInicio) ||
                                    (supervision.FechaFin.HasValue && s.FechaFin.HasValue && 
                                    supervision.FechaInicio <= s.FechaFin && 
                                    supervision.FechaFin >= s.FechaInicio)
                                ));

                if (asignacionDuplicada)
                {
                    // Obtener nombres de usuario para mensaje de error
                    var usuarios = await _context.Usuario
                        .Where(u => u.Id == supervision.SupervisorId || u.Id == supervision.EmpleadoId)
                        .Select(u => new { u.Id, u.UserName })
                        .ToListAsync();

                    var supervisor = usuarios.FirstOrDefault(u => u.Id == supervision.SupervisorId)?.UserName ?? "N/A";
                    var empleado = usuarios.FirstOrDefault(u => u.Id == supervision.EmpleadoId)?.UserName ?? "N/A";

                    ModelState.AddModelError("", 
                        $"El supervisor {supervisor} ya está asignado al empleado {empleado} " +
                        $"en el período seleccionado. Por favor elija otro período o diferente asignación.");
                }
            }

            // 7. Validación de fechas
            if (supervision.FechaFin.HasValue && !supervision.FechaInicio.Equals("") && 
                supervision.FechaFin < supervision.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio");
            }

            // 8. Verificar existencia en BD
            if (!string.IsNullOrEmpty(supervision.SupervisorId))
            {
                var supervisorExiste = await _context.Usuario.AnyAsync(u => u.Id == supervision.SupervisorId);
                if (!supervisorExiste)
                {
                    ModelState.AddModelError("SupervisorId", "El supervisor seleccionado no existe");
                }
            }

            if (!string.IsNullOrEmpty(supervision.EmpleadoId))
            {
                var empleadoExiste = await _context.Usuario.AnyAsync(u => u.Id == supervision.EmpleadoId);
                if (!empleadoExiste)
                {
                    ModelState.AddModelError("EmpleadoId", "El empleado seleccionado no existe");
                }
            }

            // 9. Guardar cambios si todo es válido
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supervision);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!SupervisionExists(supervision.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudieron guardar los cambios. " +
                                                "El registro fue modificado por otro usuario. " +
                                                "Por favor refresque la página e intente nuevamente.");
                        Console.WriteLine($"Error de concurrencia: {ex.Message}");
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar los cambios. " +
                                            "Por favor verifique los datos e intente nuevamente.");
                    Console.WriteLine($"Error al guardar: {ex.Message}");
                }
            }

            // 10. Recargar datos para la vista en caso de error
            ViewData["EmpleadoId"] = new SelectList(_context.Usuario, "Id", "UserName", supervision.EmpleadoId);
            ViewData["SupervisorId"] = new SelectList(_context.Usuario, "Id", "UserName", supervision.SupervisorId);
            
            return View(supervision);
        }

        // GET: Supervision/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .Include(s => s.Empleado)
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        // POST: Supervision/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supervision = await _context.Supervision.FindAsync(id);
            if (supervision != null)
            {
                _context.Supervision.Remove(supervision);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupervisionExists(int id)
        {
            return _context.Supervision.Any(e => e.Id == id);
        }
    }
}
