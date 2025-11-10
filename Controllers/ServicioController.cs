using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;

namespace SoftWC.Controllers
{
    [Authorize(Roles = "Administrador,Supervisor,Controltotal")]
    public class ServicioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Servicio
        public async Task<IActionResult> Index()
        {
            return View(await _context.Servicio.ToListAsync());
        }

        // GET: Servicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicio
                .FirstOrDefaultAsync(m => m.ServicioId == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servicio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicioId,NombreServicio,TipoServicio,Descripcion,PrecioBase,DuracionEstimada")] Servicio servicio)
        {
            ModelState.Remove("Usuarios"); // Excluir Usuarios de la validación
            if (ModelState.IsValid)
            {
                _context.Add(servicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicio);
        }

        // GET: Servicio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }
            return View(servicio);
        }

        // POST: Servicio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicioId,NombreServicio,TipoServicio,Descripcion,PrecioBase,DuracionEstimada")] Servicio servicio)
        {
            if (id != servicio.ServicioId)
            {
                return NotFound();
            }
            ModelState.Remove("Usuarios"); // Excluir Usuarios de la validación
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioExists(servicio.ServicioId))
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

            return View(servicio);
        }

        // GET: Servicio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicio
                .FirstOrDefaultAsync(m => m.ServicioId == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // POST: Servicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicio.Remove(servicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicio.Any(e => e.ServicioId == id);
        }
    }
}
