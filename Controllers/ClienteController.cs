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
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace SoftWC.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cliente
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cliente.ToListAsync());
        }

        // GET: Cliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.ClienteId == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Cliente/Create
        public IActionResult Create()
        {
                ViewData["TipoCliente"] = new SelectList(
                Enum.GetValues(typeof(TipoClienteEnum))
                    .Cast<TipoClienteEnum>()
                    .Select(e => new {
                        Value = (int)e,
                        Text = e.GetType()
                            .GetMember(e.ToString())[0]
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? e.ToString()
                    }),
                "Value", 
                "Text");            
            return View();
        }

        // POST: Cliente/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,Nombre,Apellido,Telefono,Correo,TipoCliente,Estado")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TipoCliente"] = new SelectList(
                Enum.GetValues(typeof(TipoClienteEnum))
                    .Cast<TipoClienteEnum>()
                    .Select(e => new {
                        Value = (int)e,
                        Text = e.GetType()
                            .GetMember(e.ToString())[0]
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? e.ToString()
                    }),
                "Value", 
                "Text");        
            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            // Guardando datos TipoCliente dropdown list
            ViewData["TipoCliente"] = new SelectList(
                Enum.GetValues(typeof(TipoClienteEnum))
                    .Cast<TipoClienteEnum>()
                    .Select(e => new {
                        Value = (int)e,
                        Text = e.GetType()
                            .GetMember(e.ToString())[0]
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? e.ToString()
                    }),
                "Value", 
                "Text");
            return View(cliente); 
        }

        // POST: Cliente/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClienteId,Nombre,Apellido,Telefono,Correo,TipoCliente,Estado")] Cliente cliente)
        {
            if (id != cliente.ClienteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.ClienteId))
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
            // Guardando datos TipoCliente dropdown list
            ViewData["TipoCliente"] = new SelectList(
                Enum.GetValues(typeof(TipoClienteEnum))
                    .Cast<TipoClienteEnum>()
                    .Select(e => new {
                        Value = (int)e,
                        Text = e.GetType()
                            .GetMember(e.ToString())[0]
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? e.ToString()
                    }),
                "Value", 
                "Text");
            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.ClienteId == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente != null)
            {
                _context.Cliente.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.ClienteId == id);
        }
    }
}
