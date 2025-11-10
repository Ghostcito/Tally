using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SoftWC.Controllers
{
    [Authorize]
    public class AsistenciaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AsistenciaService _asistenciaService;

        public AsistenciaController(ApplicationDbContext context, AsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
            _context = context;
        }

        // GET: Asitencia
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Asistencia.Include(a => a.Empleado);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet("Index/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var asistencias = _asistenciaService.GetAllAsistenciaByIdEmpleado(id);
            return View(asistencias);
        }

        // GET: Asitencia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Empleado)
                .FirstOrDefaultAsync(m => m.IdAsistencia == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // GET: Asitencia/Create
        public async Task<IActionResult> Create()
        {
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

            ViewData["IdEmpleado"] = new SelectList(empleados, "Id", "NombreCompleto");
            
            return View();
        }

        // POST: Asitencia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdAsistencia,IdEmpleado,Fecha,HoraEntrada,HoraSalida,Presente,Observacion")] Asistencia asistencia)
        {
            ModelState.Remove("IdEmpleado");
            ModelState.Remove("Empleado");

            

            // Asegurar que la Fecha tenga Kind=Utc (si tiene valor)
            if (asistencia.Fecha != default)
            {
                asistencia.Fecha = DateTime.SpecifyKind(asistencia.Fecha, DateTimeKind.Utc);
            }

            // Conversión de HoraEntrada a UTC (si tiene valor)
            if (asistencia.HoraEntrada.HasValue)
            {
                asistencia.HoraEntrada = DateTime.SpecifyKind(asistencia.HoraEntrada.Value, DateTimeKind.Utc);
            }

            // Conversión de HoraSalida a UTC (si tiene valor)
            if (asistencia.HoraSalida.HasValue)
            {
                asistencia.HoraSalida = DateTime.SpecifyKind(asistencia.HoraSalida.Value, DateTimeKind.Utc);
            }

            asistencia.HorasTrabajadas = await _asistenciaService.CalcularHorasTrabajadas((DateTime)asistencia.HoraEntrada, (DateTime)asistencia.HoraSalida);

            if (ModelState.IsValid)
            {
                _context.Add(asistencia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }
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

                ViewData["IdEmpleado"] = new SelectList(empleados, "Id", "NombreCompleto");
                return View(asistencia);
            }
        }

        // GET: Asitencia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }
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

            ViewData["IdEmpleado"] = new SelectList(empleados, "Id", "NombreCompleto");
            return View(asistencia);
        }

        // POST: Asitencia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsistencia,IdEmpleado,Fecha,HoraEntrada,HoraSalida,Presente,Observacion")] Asistencia asistencia)
        {
            if (id != asistencia.IdAsistencia)
            {
                return NotFound();
            }

            ModelState.Remove("IdEmpleado");
            ModelState.Remove("Empleado");

            // Asegurar que la Fecha tenga Kind=Utc (si tiene valor)
            if (asistencia.Fecha != default)
            {
                asistencia.Fecha = DateTime.SpecifyKind(asistencia.Fecha, DateTimeKind.Utc);
            }

            // Conversión de HoraEntrada a UTC (si tiene valor)
            if (asistencia.HoraEntrada.HasValue)
            {
                asistencia.HoraEntrada = DateTime.SpecifyKind(asistencia.HoraEntrada.Value, DateTimeKind.Utc);
            }

            // Conversión de HoraSalida a UTC (si tiene valor)
            if (asistencia.HoraSalida.HasValue)
            {
                asistencia.HoraSalida = DateTime.SpecifyKind(asistencia.HoraSalida.Value, DateTimeKind.Utc);
            }
            asistencia.HorasTrabajadas = await _asistenciaService.CalcularHorasTrabajadas((DateTime)asistencia.HoraEntrada, (DateTime)asistencia.HoraSalida);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciaExists(asistencia.IdAsistencia))
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

            ViewData["IdEmpleado"] = new SelectList(empleados, "Id", "NombreCompleto");
            return View(asistencia);
        }

        // GET: Asitencia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Empleado)
                .FirstOrDefaultAsync(m => m.IdAsistencia == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // POST: Asitencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia != null)
            {
                _context.Asistencia.Remove(asistencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsistenciaExists(int id)
        {
            return _context.Asistencia.Any(e => e.IdAsistencia == id);
        }
    }
}
