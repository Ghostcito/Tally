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
    [Authorize(Roles = "Administrador,Supervisor,Controltotal")]
    public class TurnoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TurnoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Turno
        public async Task<IActionResult> Index()
        {
            return View(await _context.Turno.ToListAsync());
        }

        // GET: Turno/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turno = await _context.Turno
                .FirstOrDefaultAsync(m => m.TurnoId == id);
            if (turno == null)
            {
                return NotFound();
            }

            return View(turno);
        }

        // GET: Turno/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Turno/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TurnoId,NombreTurno,HoraInicio,HoraFin,Descripcion")] Turno turno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(turno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(turno);
        }

        // GET: Turno/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turno = await _context.Turno.FindAsync(id);
            if (turno == null)
            {
                return NotFound();
            }
            return View(turno);
        }

        // POST: Turno/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TurnoId,NombreTurno,HoraInicio,HoraFin,Descripcion")] Turno turno)
        {
            if (id != turno.TurnoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnoExists(turno.TurnoId))
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
            return View(turno);
        }

        // GET: Turno/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turno = await _context.Turno
                .FirstOrDefaultAsync(m => m.TurnoId == id);
            if (turno == null)
            {
                return NotFound();
            }

            return View(turno);
        }

        // POST: Turno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var turno = await _context.Turno.FindAsync(id);
            if (turno != null)
            {
                _context.Turno.Remove(turno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TurnoExists(int id)
        {
            return _context.Turno.Any(e => e.TurnoId == id);
        }
    }
}
