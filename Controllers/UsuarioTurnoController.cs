using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;

namespace SoftWC.Controllers
{
    [Authorize(Roles = "Administrador,Supervisor,Controltotal")]
    public class UsuarioTurnoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmpleadoService _empleadoService;

        public UsuarioTurnoController(ApplicationDbContext context, EmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
            _context = context;
        }

        // GET: UsuarioTurno
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UsuarioTurno.Include(u => u.Turno).Include(u => u.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UsuarioTurno/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioTurno = await _context.UsuarioTurno
                .Include(u => u.Turno)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioTurnoId == id);
            if (usuarioTurno == null)
            {
                return NotFound();
            }

            return View(usuarioTurno);
        }

        // GET: UsuarioTurno/Create
        public async Task<IActionResult> Create()
        {
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno");
            var empleados = await _context.Usuario
                .Where(u => _context.UserRoles.Any(ur => 
                    ur.UserId == u.Id && 
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Empleado")))
                .Select(u => new 
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}"  // Combina Nombre + Apellido
                })
                .ToListAsync();

            ViewData["UsuarioId"] = new SelectList(empleados, "Id", "NombreCompleto");
            return View();
        }

        // POST: UsuarioTurno/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioTurnoId,UsuarioId,TurnoId,FechaInicio,FechaFin,Activo")] UsuarioTurno usuarioTurno)
        {
            usuarioTurno.Usuario = await _context.Usuario.FindAsync(usuarioTurno.UsuarioId);
            usuarioTurno.Turno = await _context.Turno.FindAsync(usuarioTurno.TurnoId);

            ModelState.Remove("Usuario");
            ModelState.Remove("Turno");

            // 2. Validación manual de UsuarioId y TurnoId
            if (string.IsNullOrEmpty(usuarioTurno.UsuarioId))
            {
                ModelState.AddModelError("UsuarioId", "El campo Usuario es obligatorio.");
            }
            if (usuarioTurno.TurnoId <= 0) // Asumiendo que TurnoId es un entero positivo
            {
                ModelState.AddModelError("TurnoId", "El campo Turno es obligatorio.");
            }


            if (ModelState.IsValid)
            {
                if (usuarioTurno.FechaInicio.HasValue)
                {
                    usuarioTurno.FechaInicio = DateTime.SpecifyKind(usuarioTurno.FechaInicio.Value, DateTimeKind.Utc);
                }

                if (usuarioTurno.FechaFin.HasValue)
                {
                    usuarioTurno.FechaFin = DateTime.SpecifyKind(usuarioTurno.FechaFin.Value, DateTimeKind.Utc);
                }

                _context.Add(usuarioTurno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine(usuarioTurno.UsuarioId + " " + usuarioTurno.TurnoId);
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(modelError.ErrorMessage);
                }
                ModelState.AddModelError("", "Error al crear el Usuario Turno. Verifique los datos ingresados.");
            }
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno", usuarioTurno.TurnoId);
            var empleados = await _context.Usuario
                .Where(u => _context.UserRoles.Any(ur => 
                    ur.UserId == u.Id && 
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Empleado")))
                .Select(u => new 
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}"  // Combina Nombre + Apellido
                })
                .ToListAsync();

            ViewData["UsuarioId"] = new SelectList(empleados, "Id", "NombreCompleto");
            return View(usuarioTurno);
        }

        // GET: UsuarioTurno/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioTurno = await _context.UsuarioTurno.FindAsync(id);
            if (usuarioTurno == null)
            {
                return NotFound();
            }
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno", usuarioTurno.TurnoId);
            var empleados = await _context.Usuario
                .Where(u => _context.UserRoles.Any(ur => 
                    ur.UserId == u.Id && 
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Empleado")))
                .Select(u => new 
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}"  // Combina Nombre + Apellido
                })
                .ToListAsync();

            ViewData["UsuarioId"] = new SelectList(empleados, "Id", "NombreCompleto");
            return View(usuarioTurno);
        }

        // POST: UsuarioTurno/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioTurnoId,UsuarioId,TurnoId,FechaInicio,FechaFin,Activo")] UsuarioTurno usuarioTurno)
        {
            if (id != usuarioTurno.UsuarioTurnoId)
            {
                return NotFound();
            }
            usuarioTurno.Usuario = await _context.Usuario.FindAsync(usuarioTurno.UsuarioId);
            usuarioTurno.Turno = await _context.Turno.FindAsync(usuarioTurno.TurnoId);

            ModelState.Remove("Usuario");
            ModelState.Remove("Turno");

            // 2. Validación manual de UsuarioId y TurnoId
            if (string.IsNullOrEmpty(usuarioTurno.UsuarioId))
            {
                ModelState.AddModelError("UsuarioId", "El campo Usuario es obligatorio.");
            }
            if (usuarioTurno.TurnoId <= 0) // Asumiendo que TurnoId es un entero positivo
            {
                ModelState.AddModelError("TurnoId", "El campo Turno es obligatorio.");
            }

            if (ModelState.IsValid)
            {
                if (usuarioTurno.FechaInicio.HasValue)
                {
                    usuarioTurno.FechaInicio = DateTime.SpecifyKind(usuarioTurno.FechaInicio.Value, DateTimeKind.Utc);
                }

                if (usuarioTurno.FechaFin.HasValue)
                {
                    usuarioTurno.FechaFin = DateTime.SpecifyKind(usuarioTurno.FechaFin.Value, DateTimeKind.Utc);
                }

                try
                {
                    _context.Update(usuarioTurno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioTurnoExists(usuarioTurno.UsuarioTurnoId))
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
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno", usuarioTurno.TurnoId);
            var empleados = await _context.Usuario
                .Where(u => _context.UserRoles.Any(ur => 
                    ur.UserId == u.Id && 
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Empleado")))
                .Select(u => new 
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}"  // Combina Nombre + Apellido
                })
                .ToListAsync();

            ViewData["UsuarioId"] = new SelectList(empleados, "Id", "NombreCompleto");
            return View(usuarioTurno);
        }

        // GET: UsuarioTurno/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioTurno = await _context.UsuarioTurno
                .Include(u => u.Turno)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioTurnoId == id);
            if (usuarioTurno == null)
            {
                return NotFound();
            }

            return View(usuarioTurno);
        }

        // POST: UsuarioTurno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioTurno = await _context.UsuarioTurno.FindAsync(id);
            if (usuarioTurno != null)
            {
                _context.UsuarioTurno.Remove(usuarioTurno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioTurnoExists(int id)
        {
            return _context.UsuarioTurno.Any(e => e.UsuarioTurnoId == id);
        }
    }
}
