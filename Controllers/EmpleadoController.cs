using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SoftWC.Models;
using SoftWC.Service;
using SoftWC.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SoftWC.Models.Dto;

namespace SoftWC.Controllers
{
    [Authorize]
    public class EmpleadoController : Controller
    {
        private readonly ILogger<EmpleadoController> _logger;
        private readonly UserService _userService;
        private readonly AsistenciaService _asistenciaService;
        private readonly EmpleadoService _empleadoService;
        private readonly SignInManager<Usuario> _signInManager;


        public EmpleadoController(ILogger<EmpleadoController> logger, UserService userService, AsistenciaService asistenciaService, EmpleadoService empleadoService, SignInManager<Usuario> signInManager)
        {
            _asistenciaService = asistenciaService;
            _logger = logger;
            _userService = userService;
            _empleadoService = empleadoService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> MarcaEntrada()
        {
            //Obtener Fecha y Hora Actual
            var fecha = await _asistenciaService.GetFechaHoraActual(DateTime.Now);
            Console.WriteLine("Fecha y hora actual: " + fecha);

            //Validar entrada unica por empleado
            if (!await _asistenciaService.VerificarUnicaEntrada(fecha))
            {
                return View("EntradaExistente");
            }

            //Obtener ubicación
            UbicacionDTO ubicacion = new UbicacionDTO
            {
                Latitud = Convert.ToDouble(TempData["Latitud"]),
                Longitud = Convert.ToDouble(TempData["Longitud"]),
                EmpleadoId = TempData["EmpleadoId"]?.ToString()
            };

            // Verificar distancia a la sede
            var verificacion = await _asistenciaService.ValidarDistancia(ubicacion);
            if (verificacion.Item1 == null)
            {
                return View("NoSedesAsign");
            }

            // Obtener turno del empleado
            var estado = await _asistenciaService.VerificarHoraEntrada(fecha.TimeOfDay);
            if (estado.Item1.Equals("NO_ASIGNADO"))
            {
                return Json(new
                {
                    showAlert = true,
                    title = "Sin turno asignado",
                    text = "No tienes un turno asignado para hoy.",
                    icon = "warning"
                });
            }
            if (estado.Item1.Equals("ANTICIPADO"))
            {
                return View("FueraDeHora");
            }

            //Creando el ViewModel para la vista

            MarcaViewModel viewModel = new MarcaViewModel
            {
                NombreSede = verificacion.Item1.Nombre_local,
                horaActual = fecha.ToString("HH:mm"),
                fechaActual = fecha.ToString("dd/MM/yyyy"),
                HoraEntradaEsperada = estado.Item2?.HoraInicio.ToString(@"hh\:mm"),
                localizacionExitosa = verificacion.Item2
            };

            //Recuperar asistencia del empleado
            ViewData["Estado"] = estado.Item1;
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarEntrada()
        {
            //Generar Asistencia
            Asistencia asistencia = await _asistenciaService.AddEntrada();
            await _asistenciaService.AddAsistencia(asistencia);
            ViewData["HoraRegistrada"] = asistencia.HoraEntrada.Value.ToString(@"HH\:mm");
            ViewData["FechaRegistrada"] = asistencia.Fecha.ToString("dd 'de' MM 'del' yyyy");

            return View("Confirmacion");
        }

        public async Task<IActionResult> MarcaSalida()
        {

            //CREANDO VIEWMODEL PARA LA UBICACION
            UbicacionDTO ubicacion = new UbicacionDTO
            {
                Latitud = Convert.ToDouble(TempData["Latitud"]),
                Longitud = Convert.ToDouble(TempData["Longitud"]),
                EmpleadoId = TempData["EmpleadoId"]?.ToString()
            };

            // Verificar que la ubicación no sea nula
            var verificacion = await _asistenciaService.ValidarDistancia(ubicacion);
            if (verificacion.Item1 == null)
            {
                return View("NoSedesAsign");
            }

            // Obtener Fecha y Hora Actual con zona horario
            var fecha = await _asistenciaService.GetFechaHoraActual(DateTime.Now);

            // Validar si ya existe una entrada registrada
            var asistencia = await _asistenciaService.GetAsistenciaByDate(fecha);
            if (asistencia == null || asistencia.HoraEntrada == null)
            {
                return View("NoSalida");
            }
            if (asistencia.HoraSalida != null)
            {
                return View("EntradaExistente");
            }

            // Verificar si el empleado tiene un turno asignado
            /*var estado = await _asistenciaService.VerificarHoraSalida(fecha.TimeOfDay);
            if (estado.Item1.Equals("NO_ASIGNADO"))
            {
                return Json(new
                {
                    showAlert = true,
                    title = "Sin turno asignado",
                    text = "No tienes un turno asignado para hoy.",
                    icon = "warning"
                });
            }*/
            /*if (estado.Item1.Equals("ANTICIPADO"))
            {
                ViewData["TIPO"] = "Salida";
                return View("FueraDeHora");
            }*/

            /*var horasPerdidas = Decimal.Zero;

            if (estado.Item1.Equals("TARDANZA"))
            {
                DateTime horaSalidaEsperada = DateTime.Parse(estado.Item2.HoraFin.ToString(@"hh\:mm"));
                horasPerdidas = await _asistenciaService.CalcularHorasTrabajadas(horaSalidaEsperada, fecha);

            }*/

            MarcaViewModel viewModel = new MarcaViewModel
            {
                NombreSede = verificacion.Item1.Nombre_local,
                horaActual = fecha.ToString("HH:mm"),
                fechaActual = fecha.ToString("dd/MM/yyyy"),
                HoraEntrada = asistencia.HoraEntrada?.ToString(@"HH\:mm"),
                //HoraSalidaEsperada = estado.Item2?.HoraFin.ToString(@"hh\:mm"),
                HorasTrabajadas = await _asistenciaService.CalcularHorasTrabajadas(asistencia.HoraEntrada.Value, fecha),
                //HorasDescontadas = horasPerdidas,
                localizacionExitosa = verificacion.Item2
            };

            /*ViewData["Estado"] = estado.Item1;
            TempData["EstadoSalida"] = estado.Item1;
            TempData["HoraEsperada"] = estado.Item2?.HoraFin.ToString(@"hh\:mm");*/

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarSalida()
        {
            var fecha = await _asistenciaService.GetFechaHoraActual(DateTime.Now);

            var asistencia = await _asistenciaService.GetAsistenciaByDate(fecha);

            asistencia = await _asistenciaService.AddSalida(asistencia);
            _asistenciaService.UpdateAsistencia(asistencia);
            ViewData["HoraRegistrada"] = asistencia.HoraSalida.Value.ToString(@"HH\:mm");
            ViewData["FechaRegistrada"] = asistencia.Fecha.ToString("dd 'de' MM 'del' yyyy");
            return View("Confirmacion");
        }

        public IActionResult PostEntradasSalidas([FromBody] UbicacionDTO ubicacion, string tipo)
        {

            if (ubicacion == null) return BadRequest("No se recibió la ubicación.");
            if (string.IsNullOrEmpty(ubicacion.EmpleadoId)) return BadRequest("No se recibió el ID del empleado.");
            TempData["Latitud"] = ubicacion.Latitud.ToString(System.Globalization.CultureInfo.InvariantCulture);
            TempData["Longitud"] = ubicacion.Longitud.ToString(System.Globalization.CultureInfo.InvariantCulture);
            TempData["EmpleadoId"] = ubicacion.EmpleadoId;

            if (tipo == "salida")
            {
                return Json(new { redirectUrl = Url.Action("MarcaSalida") });
            }
            else
            {
                // Lógica de entrada (por defecto)
                return Json(new { redirectUrl = Url.Action("MarcaEntrada") });
            }
        }

        public async Task<IActionResult> LogoutConfirmado()
        {
            return View();
        }

        //metodo para verificar el logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["LogoutSuccess"] = true; // Se guarda en TempData
            return Redirect("~/Identity/Account/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}