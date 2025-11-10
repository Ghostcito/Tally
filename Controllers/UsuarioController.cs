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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SoftWC.Models.Dto;
using SoftWC.Models.ViewModels;

namespace SoftWC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly EmpleadoService _empleadoService;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(ApplicationDbContext context, UserService userService, EmpleadoService empleadoService, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _empleadoService = empleadoService;
            _userService = userService;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var usuarios = await _empleadoService.GetEmpleados();
            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchNombre, string searchApellido, string searchDni)
        {
            var usuarios = await _empleadoService.GetEmpleados();
            
            // Aplicar filtros si existen valores
            if (!string.IsNullOrEmpty(searchNombre))
            {
                usuarios = usuarios.Where(u => 
                    u.Nombre.Contains(searchNombre, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(searchApellido))
            {
                usuarios = usuarios.Where(u => 
                    u.Apellido.Contains(searchApellido, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(searchDni))
            {
                usuarios = usuarios.Where(u => 
                    u.DNI.Contains(searchDni)).ToList(); // DNI usualmente es case-sensitive
            }

            ViewBag.CurrentFilterNombre = searchNombre;
            ViewBag.CurrentFilterApellido = searchApellido;
            ViewBag.CurrentFilterDni = searchDni;

            return View(usuarios);
        }

        public async Task<IActionResult> FindAllBySede(int id)
        {
            var usuarios = await _context.Usuario
            .Include(u => u.Sedes)
            .Where(u => u.Sedes.Any(s => s.SedeId == id) &&
                        (u.NivelAcceso == "1" || u.NivelAcceso == "2"))
            .ToListAsync();
            return View("Index", usuarios);
        }

        public async Task<IActionResult> FindAllSedesByUsuario(int id)
        {
            var usuario = await _context.Usuario
                .Include(u => u.Sedes)
                .FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .Include(u => u.Sedes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener las evaluaciones relacionadas al usuario
            var evaluaciones = await _context.Evaluaciones
                .Where(e => e.IdEmpleado == id)
                .OrderByDescending(e => e.FechaEvaluacion)
                .ToListAsync();

            // Pasar la lista de evaluaciones por ViewBag
            ViewBag.Evaluaciones = evaluaciones;

            return View(usuario);
        }



        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,DNI,FechaIngreso,FechaNacimiento,NivelAcceso,Estado,Salario,UserName,Email,PasswordHash,PhoneNumber")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var nombreUsuario = usuario.Nombre + " " + usuario.Apellido;
                usuario.UserName = nombreUsuario;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RoleDisponibles"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            ViewData["ServiciosDisponibles"] = new SelectList(_context.Servicio.ToList(), "ServicioId", "NombreServicio");

            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nombre,Apellido,DNI,FechaIngreso,FechaNacimiento,NivelAcceso,Estado,Salario,Id,UserName,Email,PhoneNumber,ServicioId")] Usuario usuario, string PasswordActual, string NuevaContrasena, string ConfirmarContrasena, string RolSeleccionado)
        {
            var servicioId = usuario.ServicioId ?? 0;
            Console.WriteLine("Editando usuario: " + servicioId+1);    
            ViewData["RoleDisponibles"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            ViewData["ServiciosDisponibles"] = new SelectList(_context.Servicio.ToList(), "ServicioId", "NombreServicio");
            if (id != usuario.Id)
                return NotFound();

            var usuarioActual = await _userService.FindByDniAsync(usuario.DNI);
            if (usuarioActual == null)
                return NotFound();

            ModelState.Remove("PasswordActual");
            ModelState.Remove("NuevaContrasena");
            ModelState.Remove("ConfirmarContrasena");
            ModelState.Remove("RolSeleccionado");

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar campos comunes
                    usuarioActual.Nombre = usuario.Nombre;
                    usuarioActual.Apellido = usuario.Apellido;
                    usuarioActual.DNI = usuario.DNI;
                    usuarioActual.FechaIngreso = usuario.FechaIngreso.HasValue
                    ? DateTime.SpecifyKind(usuario.FechaIngreso.Value, DateTimeKind.Local).ToUniversalTime()
                    : null;

                    usuarioActual.FechaNacimiento = usuario.FechaNacimiento.HasValue
                        ? DateTime.SpecifyKind(usuario.FechaNacimiento.Value, DateTimeKind.Local).ToUniversalTime()
                        : null;

                    usuarioActual.NivelAcceso = usuario.NivelAcceso;
                    usuarioActual.Estado = usuario.Estado;
                    usuarioActual.Salario = usuario.Salario;
                    usuarioActual.UserName = usuario.Nombre;
                    usuarioActual.Email = usuario.Email;
                    usuarioActual.PhoneNumber = usuario.PhoneNumber;
                    usuarioActual.ServicioId = servicioId;

                    // Validar y cambiar la contraseña
                    if (!string.IsNullOrWhiteSpace(PasswordActual) &&
                        !string.IsNullOrWhiteSpace(NuevaContrasena) &&
                        !string.IsNullOrWhiteSpace(ConfirmarContrasena))
                    {
                        if (NuevaContrasena != ConfirmarContrasena)
                        {
                            ModelState.AddModelError("", "La nueva contraseña y la confirmación no coinciden.");
                            return View(usuario);
                        }

                        var passwordCorrecta = await _userManager.CheckPasswordAsync(usuarioActual, PasswordActual);
                        if (!passwordCorrecta)
                        {
                            ModelState.AddModelError("", "La contraseña actual no es correcta.");
                            return View(usuario);
                        }

                        var resultado = await _userManager.ChangePasswordAsync(usuarioActual, PasswordActual, NuevaContrasena);
                        if (!resultado.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(RolSeleccionado))
                            {
                                await _userManager.AddToRoleAsync(usuarioActual, RolSeleccionado);
                            }

                            foreach (var error in resultado.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(usuario);
                        }
                    }

                    Console.WriteLine("Editando usuario x33: " + usuario.Id);
                    _context.Update(usuarioActual);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                        return NotFound();
                    else
                        throw;
                }
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
                return View(usuario);
            }

            return View(usuario);
        }


        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // // POST: Usuario/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(string id)
        // {
        //     var usuario = await _context.Usuario.FindAsync(id);
        //     if (usuario != null)
        //     {
        //         _context.Usuario.Remove(usuario);
        //     }

        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        // Combertira en inactivo en lugar de eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                // Marcar como inactivo en lugar de eliminar
                usuario.Estado = "Inactivo";

                _context.Usuario.Update(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool UsuarioExists(string id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }

        // POST: api/usuarios/{usuarioId}/asignar-sedes

        // GET: Usuario/GestionarSedes/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GestionarSedes(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.Users
                .Include(u => u.Sedes)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Verificar que sea EMPLEADO
            var roles = await _userManager.GetRolesAsync(usuario);
            if (!roles.Contains("EMPLEADO"))
            {
                return BadRequest("Solo los empleados pueden tener sedes asignadas");
            }

            return View(usuario);
        }

        // GET: api/Usuario/empleados-disponibles
        [HttpGet("usuarios/empleados-disponibles")]
        public async Task<IActionResult> GetEmpleadosDisponibles([FromQuery] string filtro = "", [FromQuery] int sedeId = 0)
        {
            // Obtener solo usuarios con rol EMPLEADO
            var empleados = await _userManager.GetUsersInRoleAsync("EMPLEADO");
            var query = empleados.AsQueryable();

            // Aplicar filtro si existe
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(u =>
                    u.Nombre.Contains(filtro) ||
                    u.Apellido.Contains(filtro) ||
                    u.UserName.Contains(filtro) ||
                    u.Email.Contains(filtro));
            }

            // Excluir usuarios ya asignados a la sede
            if (sedeId > 0)
            {
                var usuariosAsignados = _context.Sede
                    .Where(s => s.SedeId == sedeId)
                    .SelectMany(s => s.Usuarios)
                    .Select(u => u.Id);

                query = query.Where(u => !usuariosAsignados.Contains(u.Id));
            }

            var result = query.Select(u => new
            {
                id = u.Id,
                userName = u.UserName,
                nombreCompleto = $"{u.Nombre} {u.Apellido}",
                email = u.Email
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{usuarioId}/sedes-asignadas")]
        public async Task<IActionResult> GetSedesAsignadas(string usuarioId)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            // Verificar que sea EMPLEADO
            var roles = await _userManager.GetRolesAsync(usuario);
            if (!roles.Contains("EMPLEADO"))
                return BadRequest("Solo los empleados pueden tener sedes asignadas");

            var sedes = await _context.Sede
                .Where(s => s.Usuarios.Any(u => u.Id == usuarioId))
                .Select(s => new
                {
                    s.SedeId,
                    s.Nombre_local,
                    s.Direccion_local,
                    s.Ciudad
                })
                .ToListAsync();

            return Ok(sedes);
        }

        [HttpPost("{usuarioId}/asignar-sede/{sedeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AsignarSedeAUsuario(string usuarioId, int sedeId)
        {
            try
            {
                // Validar usuario
                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                // Validar que sea EMPLEADO
                var roles = await _userManager.GetRolesAsync(usuario);
                if (!roles.Contains("EMPLEADO"))
                    return BadRequest("Solo se pueden asignar sedes a empleados");

                // Validar sede
                var sede = await _context.Sede
                    .Include(s => s.Usuarios)
                    .FirstOrDefaultAsync(s => s.SedeId == sedeId);

                if (sede == null)
                    return NotFound("Sede no encontrada");

                // Verificar si ya está asignado
                if (sede.Usuarios.Any(u => u.Id == usuarioId))
                    return BadRequest("El usuario ya está asignado a esta sede");

                // Asignar
                sede.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Sede {sede.Nombre_local} asignada al empleado {usuario.UserName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpDelete("{usuarioId}/remover-sede/{sedeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoverSedeDeUsuario(string usuarioId, int sedeId)
        {
            try
            {
                // Validar usuario
                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if (usuario == null)
                    return NotFound(new { Message = "Usuario no encontrado" });

                // Validar sede
                var sede = await _context.Sede
                    .Include(s => s.Usuarios)
                    .FirstOrDefaultAsync(s => s.SedeId == sedeId);

                if (sede == null)
                    return NotFound(new { Message = "Sede no encontrada" });

                // Verificar si está asignado
                var usuarioEnSede = sede.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
                if (usuarioEnSede == null)
                    return BadRequest(new { Message = "El usuario no está asignado a esta sede" });

                // Remover
                sede.Usuarios.Remove(usuarioEnSede);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Sede {sede.Nombre_local} removida del empleado {usuario.UserName}"
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

        // GET: Usuario/Calificar/5
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> Calificar(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var empleado = await _context.Usuario.FindAsync(id);
        if (empleado == null)
        {
            return NotFound();
        }

        var model = new EvaluacionViewModel
        {
            IdEmpleado = id,
            NombreEmpleado = $"{empleado.Nombre} {empleado.Apellido}",
            FechaEvaluacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            EvaluadorId = _context.Usuario.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id,
        };

        var userRoleId = await _context.UserRoles
            .Where(ur => ur.UserId == model.IdEmpleado)
            .Select(ur => ur.RoleId)
            .FirstOrDefaultAsync();

        var userRoleName = await _context.Roles
            .Where(r => r.Id == userRoleId)
            .Select(r => r.Name)
            .FirstOrDefaultAsync();

        model.TipoEmpleado = userRoleName;


        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> Calificar(EvaluacionViewModel model)
    {
        
        model.FechaEvaluacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        if (ModelState.IsValid)
        {
            var evaluacion = new Evaluaciones
            {
                IdEmpleado = model.IdEmpleado,
                TipoEmpleado = model.TipoEmpleado,
                FechaEvaluacion = model.FechaEvaluacion,
                Descripcion = model.Descripcion,
                Responsabilidad = model.Responsabilidad,
                Puntualidad = model.Puntualidad,
                CalidadTrabajo = model.CalidadTrabajo,
                UsoMateriales = model.UsoMateriales,
                Actitud = model.Actitud,
                EvaluadorId = model.EvaluadorId
            };
        Console.WriteLine("enviado: " + evaluacion);
            _context.Add(evaluacion);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Evaluación registrada correctamente";
            return RedirectToAction(nameof(Details), new { id = model.IdEmpleado });
        }

        // Agrega esta parte para mostrar errores del ModelState en consola
        foreach (var key in ModelState.Keys)
        {
            var errors = ModelState[key].Errors;
            foreach (var error in errors)
            {
                Console.WriteLine($"Error en '{key}': {error.ErrorMessage}");
            }
        }

        var empleado = await _context.Usuario.FindAsync(model.IdEmpleado);
        model.NombreEmpleado = $"{empleado?.Nombre} {empleado?.Apellido}";
        
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        ViewBag.TiposEmpleado = new SelectList(roles);
        return View(model);
    }


    // Vista para mostrar historial de evaluaciones
    public async Task<IActionResult> EvaluacionesEmpleado(string id)
    {
        var evaluaciones = await _context.Evaluaciones
            .Include(e => e.Empleado)
            .Include(e => e.Evaluador)
            .Where(e => e.IdEmpleado == id)
            .OrderByDescending(e => e.FechaEvaluacion)
            .ToListAsync();

        ViewBag.EmpleadoId = id;
        return View(evaluaciones);
    }    


    }
}
