using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SoftWC.Models;
using SoftWC.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SoftWC.Models.ViewModels;
using System.Globalization;
using SoftWC.Service.Interfaces;
using SoftWC.Service.Implementation;

namespace SoftWC.Controllers;

[Authorize(Roles = "Administrador,Supervisor,Contador,Controltotal")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UserService _userService;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    private readonly ApplicationDbContext _context;

    public AdminController(ILogger<AdminController> logger, UserService userService, SignInManager<Usuario> signInManager, ApplicationDbContext context, IPdfExportService pdfExportService, IExcelExportService excelExportService)
    {
        _context = context;
        _userService = userService;
        _logger = logger;
        _signInManager = signInManager;
        _pdfExportService = pdfExportService;
        _excelExportService = excelExportService;
    }
    [Authorize(Roles = "Administrador,Contador,Supervisor,Controltotal")]
    public IActionResult Index()
    {
        var asistencias = _context.Asistencia
            .Where(a => a.Fecha.Month == DateTime.Now.Month)
            .GroupBy(a => a.IdEmpleado)
            .Select(g => new
            {
                Empleado = g.First().Empleado.Nombre + " " + g.First().Empleado.Apellido,
                Horas = g.Sum(a => a.HorasTrabajadas)
            }).ToList();

        var empleadosPorSede = _context.Usuario
            .SelectMany(u => u.Sedes, (usuario, sede) => new { sede.Nombre_local })
            .GroupBy(x => x.Nombre_local)
            .Select(g => new { Sede = g.Key, Total = g.Count() })
            .ToList();

        var empleadosPorTurno = _context.UsuarioTurno
            .Where(ut => ut.Activo && ut.Turno != null)
            .GroupBy(ut => ut.Turno.NombreTurno)
            .Select(g => new { Turno = g.Key, Total = g.Count() })
            .ToList();

        var horasPorSemana = _context.Asistencia
            .AsEnumerable()
            .GroupBy(a => a.Fecha.Date.AddDays(-(int)a.Fecha.DayOfWeek)) // Agrupa por inicio de semana (domingo)
            .Select(g => new
            {
                Semana = g.Key.ToString("yyyy-MM-dd"),
                TotalHoras = g.Sum(a => a.HorasTrabajadas)
            })
            .OrderBy(x => x.Semana)
            .ToList();

        var empleadosPorSupervisor = _context.Supervision
            .GroupBy(s => s.Supervisor.Nombre)
            .Select(g => new { Supervisor = g.Key, Total = g.Count() })
            .ToList();

        var empleadosEnSistema = (from user in _context.Users
                                  join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                  join role in _context.Roles on userRole.RoleId equals role.Id
                                  where role.Name == "Empleado"
                                  select user.Nombre).ToList();

        // Obtener fecha actual
        var fechaActual = DateTime.UtcNow;

        // Calcular los últimos 3 meses
        var pagosUltimosMeses = _context.Asistencia
            .Where(a => a.Fecha >= fechaActual.AddMonths(-3)) // últimos 3 meses
            .GroupBy(a => new { a.Fecha.Year, a.Fecha.Month })
            .Select(g => new
            {
                Mes = g.Key.Month,
                Año = g.Key.Year,
                Total = g.Sum(a => a.HorasTrabajadas * a.Empleado.Servicio.PrecioBase)
            })
            .OrderBy(g => g.Año).ThenBy(g => g.Mes)
            .ToList();

        // Obtener usuarios próximos a cumplir años en los próximos 30 días
        var hoy = DateTime.Today;
        var proximosCumpleanios = _context.Users
        .Where(u => u.FechaNacimiento != null)
        .AsEnumerable() // Para trabajar con DateTime sin errores en EF
        .Select(u => new
        {
            Usuario = u,
            ProximoCumple = new DateTime(hoy.Year, u.FechaNacimiento.Value.Month, u.FechaNacimiento.Value.Day)
        })
        .Where(x => x.ProximoCumple >= hoy && x.ProximoCumple <= hoy.AddDays(30))
        .OrderBy(x => x.ProximoCumple)
        .Take(4)
        .Select(x => x.Usuario)
        .ToList();

        // Obtener la cantidad total de usuarios con rol "Empleado"
        var totalEmpleados = (from user in _context.Users
                              join userRole in _context.UserRoles on user.Id equals userRole.UserId
                              join role in _context.Roles on userRole.RoleId equals role.Id
                              where role.Name == "Empleado"
                              select user).Count();

        var puntajePorTipo = _context.Evaluaciones
            .GroupBy(e => e.TipoEmpleado)
            .Select(g => new
            {
                Tipo = g.Key,
                Promedio = Math.Round(g.Average(e =>
                    ((int)e.Responsabilidad + (int)e.Puntualidad + (int)e.CalidadTrabajo +
                    (int)e.UsoMateriales + (int)e.Actitud) / 5.0), 2)
            })
            .ToList();

        var evaluaciones = _context.Evaluaciones.ToList();

        var distribucion = new
        {
            Responsabilidad = evaluaciones.Any() ? _context.Evaluaciones.Average(e => (int)e.Responsabilidad) : 0,
            Puntualidad = evaluaciones.Any() ? _context.Evaluaciones.Average(e => (int)e.Puntualidad) : 0,
            CalidadTrabajo = evaluaciones.Any() ? _context.Evaluaciones.Average(e => (int)e.CalidadTrabajo) : 0,
            UsoMateriales = evaluaciones.Any() ? _context.Evaluaciones.Average(e => (int)e.UsoMateriales) : 0,
            Actitud = evaluaciones.Any() ? _context.Evaluaciones.Average(e => (int)e.Actitud) : 0
        };

        ViewBag.CriteriosLabels = new[] { "Responsabilidad", "Puntualidad", "CalidadTrabajo", "UsoMateriales", "Actitud" };
        ViewBag.CriteriosData = new[] {
            distribucion.Responsabilidad,
            distribucion.Puntualidad,
            distribucion.CalidadTrabajo,
            distribucion.UsoMateriales,
            distribucion.Actitud
        };


        ViewBag.TiposEmpleadoLabels = puntajePorTipo.Select(x => x.Tipo).ToList();
        ViewBag.TiposEmpleadoData = puntajePorTipo.Select(x => x.Promedio).ToList();


        // ViewBag con listas simples (labels y data separados)
        ViewBag.Empleados = asistencias.Select(a => a.Empleado).ToList();
        ViewBag.Horas = asistencias.Select(a => a.Horas).ToList();

        ViewBag.Sedes = empleadosPorSede.Select(e => e.Sede).ToList();
        ViewBag.EmpleadosPorSede = empleadosPorSede.Select(e => e.Total).ToList();

        ViewBag.Turnos = empleadosPorTurno.Select(e => e.Turno).ToList();
        ViewBag.EmpleadosPorTurno = empleadosPorTurno.Select(e => e.Total).ToList();

        ViewBag.Semanas = horasPorSemana.Select(s => s.Semana).ToList();
        ViewBag.HorasPorSemana = horasPorSemana.Select(s => s.TotalHoras).ToList();

        ViewBag.Supervisores = empleadosPorSupervisor.Select(e => e.Supervisor).ToList();
        ViewBag.EmpleadosPorSupervisor = empleadosPorSupervisor.Select(e => e.Total).ToList();

        ViewBag.EmpleadosEnSistema = empleadosEnSistema;

        ViewBag.MesesPagos = pagosUltimosMeses.Select(p => $"{p.Mes}/{p.Año}").ToList();
        ViewBag.TotalPagosPorMes = pagosUltimosMeses.Select(p => p.Total).ToList();

        ViewBag.ProximosCumpleanios = proximosCumpleanios;
        ViewBag.TotalEmpleados = totalEmpleados;

        // Empleados por sede (usando Usuario y Sede)
        var usuariosConSedes = _context.Usuario
            .Include(u => u.Sedes)
            .ToList(); // Trae a memoria

        var sedesRaw = usuariosConSedes
            .SelectMany(u => u.Sedes.Select(sede => new { Sede = sede.Nombre_local }))
            .GroupBy(x => x.Sede)
            .Select(g => new
            {
                sede = g.Key,
                cantidad = g.Count()
            }).ToList();

        ViewBag.Sedes = sedesRaw.Select(x => x.sede).ToList();
        ViewBag.EmpleadosPorSede = sedesRaw.Select(x => x.cantidad).ToList();
        ViewBag.SedesRaw = sedesRaw;

        // Usuarios por sede para el filtro
        var sedesUsuariosRaw = usuariosConSedes
            .SelectMany(u => u.Sedes.Select(sede => new
            {
                Sede = sede.Nombre_local,
                Usuario = u.Nombre + " " + u.Apellido
            }))
            .GroupBy(x => x.Sede)
            .Select(g => new
            {
                sede = g.Key,
                usuarios = g.Select(x => x.Usuario).ToList()
            }).ToList();

        ViewBag.SedesUsuariosRaw = sedesUsuariosRaw;

        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    //metodo para verificar el logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Logout"] = true; // Indicador de que fue una salida voluntaria
        return RedirectToAction("Login", "Account");
    }

    private (DateTime inicio, DateTime fin) CalcularRangoQuincena(int año, int mes, int quincena)
    {
        DateTime inicio, fin;

        if (quincena == 1)
        {
            inicio = new DateTime(año, mes, 1, 0, 0, 0, DateTimeKind.Utc);
            fin = new DateTime(año, mes, 15, 23, 59, 59, DateTimeKind.Utc);
        }
        else
        {
            inicio = new DateTime(año, mes, 16, 0, 0, 0, DateTimeKind.Utc);
            fin = new DateTime(año, mes, DateTime.DaysInMonth(año, mes), 23, 59, 59, DateTimeKind.Utc);
        }

        return (inicio, fin);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]

    [Authorize(Roles = "Administrador,Contador,Controltotal")]
    public async Task<IActionResult> ResumenPagos(int? año = null, int? mes = null, int? quincena = null)
    {
        try
        {
            // Establecer valores por defecto
            año = año ?? DateTime.Now.Year;
            mes = mes ?? DateTime.Now.Month;
            quincena = quincena ?? (DateTime.Now.Day <= 15 ? 1 : 2);

            _logger.LogInformation($"Parámetros recibidos: Año={año}, Mes={mes}, Quincena={quincena}");

            // Validar quincena
            if (quincena != 1 && quincena != 2)
            {
                _logger.LogWarning("Quincena inválida, usando valor por defecto.");
                quincena = DateTime.Now.Day <= 15 ? 1 : 2;
            }

            var (inicio, fin) = CalcularRangoQuincena(año.Value, mes.Value, quincena.Value);
            _logger.LogInformation($"Rango calculado: Inicio={inicio}, Fin={fin}");

            // Verificar si hay asistencias en el rango antes de hacer todo el resumen
            var cantidadAsistencias = await _context.Asistencia
                .Where(a => a.Fecha >= inicio && a.Fecha <= fin)
                .CountAsync();
            _logger.LogInformation($"Cantidad de asistencias en el rango: {cantidadAsistencias}");

            // Consulta optimizada para agrupar correctamente
            var resumen = await _context.Asistencia
                .Where(a => a.HorasTrabajadas != null && a.Empleado.Servicio.PrecioBase != null)
                .Where(a => a.Fecha >= inicio && a.Fecha <= fin)
                .Include(a => a.Empleado)
                .Include(a => a.Empleado.Servicio)
                .GroupBy(a => new
                {
                    a.IdEmpleado,
                    NombreCompleto = a.Empleado.Nombre + " " + a.Empleado.Apellido,
                    a.Empleado.DNI,
                    NombreServicio = a.Empleado.Servicio.NombreServicio,
                    a.Empleado.Servicio.PrecioBase
                })
                .Select(g => new ResumenPagoVM
                {
                    Empleado = g.Key.NombreCompleto,
                    DNI = g.Key.DNI,
                    TotalHoras = g.Sum(a => a.HorasTrabajadas),
                    PrecioBase = g.Key.PrecioBase,
                    TotalPago = g.Sum(a => a.HorasTrabajadas) * g.Key.PrecioBase,
                    Servicios = new List<string> { g.Key.NombreServicio },
                    Detalles = g.OrderBy(a => a.Fecha).Select(a => new DetallePagoVM
                    {
                        Fecha = a.Fecha,
                        Servicio = g.Key.NombreServicio,
                        Horas = a.HorasTrabajadas,
                        PagoHora = g.Key.PrecioBase,
                        TotalDia = a.HorasTrabajadas * g.Key.PrecioBase
                    }).ToList()
                })
                .OrderBy(r => r.Empleado)
                .ToListAsync();

            _logger.LogInformation($"Resumen de pagos generado con {resumen.Count} registros");

            ViewBag.AñoSeleccionado = año;
            ViewBag.MesSeleccionado = mes;
            ViewBag.QuincenaSeleccionada = quincena;

            return View(resumen);
        }
        catch (Exception ex)
        {
            return View(new List<ResumenPagoVM>());
        }
    }


    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar(
        int año, int mes, int quincena, string formato)
    {
        var datos = await ObtenerDatosQuincena(año, mes, quincena);

        if (formato.ToLower() == "excel")
        {
            var fileContent = _excelExportService.GenerateExcel(datos, año, mes, quincena);
            return File(fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Pagos_{año}_{mes}_Q{quincena}.xlsx");
        }
        else if (formato.ToLower() == "pdf")
        {
            var fileContent = _pdfExportService.GeneratePdf(datos, año, mes, quincena);
            return File(fileContent, "application/pdf",
                $"Pagos_{año}_{mes}_Q{quincena}.pdf");
        }

        return BadRequest("Formato no soportado");
    }

    private async Task<List<ResumenPagoVM>> ObtenerDatosQuincena(int año, int mes, int quincena)
    {
        // Reutiliza la misma lógica del action ResumenPagos
        var (inicio, fin) = CalcularRangoQuincena(año, mes, quincena);

        return await _context.Asistencia
        .Where(a => a.HorasTrabajadas != null && a.Empleado.Servicio.PrecioBase != null)
        .Where(a => a.Fecha >= inicio && a.Fecha <= fin)
        .GroupBy(a => new
        {
            a.IdEmpleado,
            a.Empleado.UserName,
            a.Empleado.DNI,
            a.Empleado.Servicio.NombreServicio,
            a.Empleado.Servicio.PrecioBase
        })
        .Select(g => new ResumenPagoVM
        {
            Empleado = g.Key.UserName,
            DNI = g.Key.DNI,
            TotalHoras = g.Sum(a => a.HorasTrabajadas),
            PrecioBase = g.Key.PrecioBase,
            TotalPago = g.Sum(a => a.HorasTrabajadas) * g.Key.PrecioBase,
            Servicios = new List<string> { g.Key.NombreServicio }
            // Detalles se omite o se inicializa como lista vacía si es requerido
        })
        .ToListAsync();
    }
}
