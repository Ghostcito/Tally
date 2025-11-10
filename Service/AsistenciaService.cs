using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prueba_Geolocalizacion.Utils;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Models.Dto;

namespace SoftWC.Service
{
    public class AsistenciaService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        public AsistenciaService(ApplicationDbContext context, UserService userService)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<Asistencia> AddEntrada()
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();

            //CAMBIANDO ZONA HORARIO
            var limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var horaPeru = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);
            horaPeru = DateTime.SpecifyKind(horaPeru, DateTimeKind.Utc);


            var asistencia = new Asistencia
            {
                IdEmpleado = userPrincipal.Id,
                Empleado = userPrincipal,
                Fecha = horaPeru.Date,
                HoraEntrada = horaPeru,
                Presente = true,
            };
            return asistencia;

        }

        public async Task<Asistencia> GetAsistenciaByDate(DateTime fecha)
        {
            fecha = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
            var userPrincipal = await _userService.GetCurrentUserAsync();
            fecha = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
            var asistencia = await _context.Asistencia
                .FirstOrDefaultAsync(a => a.IdEmpleado == userPrincipal.Id && a.Fecha.Date == fecha.Date);
            return asistencia;
        }

        public async Task<Asistencia?> AddSalida(Asistencia asistencia)
        {
            var limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var horaPeru = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);
            horaPeru = DateTime.SpecifyKind(horaPeru, DateTimeKind.Utc);

            asistencia.HoraSalida = horaPeru;
            asistencia.HorasTrabajadas = await CalcularHorasTrabajadas((DateTime)asistencia.HoraEntrada, (DateTime)asistencia.HoraSalida);
            return asistencia;
        }

        public async Task<decimal> CalcularHorasTrabajadas(DateTime horaEntrada, DateTime horaSalida)
        {
            return (decimal)(horaSalida - horaEntrada).TotalHours;
        }
        public async Task<Asistencia> AddAsistencia(Asistencia asistencia)
        {
            _context.Asistencia.Add(asistencia);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar asistencia: " + ex.InnerException?.Message, ex);
            }
            return asistencia;
        }

        public async Task<(Sede, bool)> ValidarDistancia(UbicacionDTO ubicacion)
        {
            var empleado = await _context.Usuario
            .Include(u => u.Sedes)
            .FirstOrDefaultAsync(u => u.Id == ubicacion.EmpleadoId);
            if (empleado.Sedes == null || !empleado.Sedes.Any())
                return (null, false);
            return await DetectarSede(Convert.ToDecimal(ubicacion.Latitud), Convert.ToDecimal(ubicacion.Longitud), empleado.Sedes);
        }

        public async Task<(Sede, bool)> DetectarSede(decimal latitud, decimal longitud, ICollection<Sede> sedes)
        {
            double distMin = double.MaxValue;
            Sede sedeCercana = sedes.FirstOrDefault();
            foreach (var sede in sedes)
            {
                var distancia = GeoUtils.CalcularDistancia(Convert.ToDouble(latitud), Convert.ToDouble(longitud), Convert.ToDouble(sede.Latitud), Convert.ToDouble(sede.Longitud));
                if (distancia < distMin)
                {
                    sedeCercana = sede;
                    distMin = distancia;
                }
            }
            if (distMin <= Convert.ToDouble(sedeCercana.Radio))
            {
                return (sedeCercana, true);
            }
            return (sedeCercana, false);
        }

        public async Task<bool> VerificarUnicaEntrada(DateTime fecha)
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();
            fecha = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
            var asistencia = await _context.Asistencia
                .FirstOrDefaultAsync(a => a.IdEmpleado == userPrincipal.Id && a.Fecha.Date == fecha.Date && a.HoraEntrada != null);
            return asistencia == null;
        }

        public async Task<(string, Turno?)> VerificarHoraEntrada(TimeSpan horaMarcada)
        {
            var turnoAsignado = await VerificarTurnosAsignados();
            if (!turnoAsignado.Item1)
            {
                return ("NO_ASIGNADO", null);
            }
            var turno = turnoAsignado.Item2.Turno;
            // Definir los rangos permitidos con desviación de 10 minutos
            var margen = TimeSpan.FromMinutes(10);
            // Hora de entrada permitida
            var horaEntradaEsperada = turno.HoraInicio;

            var entradaMin = horaEntradaEsperada - margen;
            var entradaMax = horaEntradaEsperada + margen;

            if (horaMarcada < entradaMin)
            {
                // La hora de entrada es antes del rango permitido
                return ("ANTICIPADO", turno);
            }
            if (horaMarcada > entradaMax)
            {
                // La hora de entrada es después del rango permitido
                return ("TARDANZA", turno);
            }
            return ("PUNTUAL", turno);
        }

        public async Task<(string, Turno?)> VerificarHoraSalida(TimeSpan horaMarcada)
        {
            var turnoAsignado = await VerificarTurnosAsignados();
            if (!turnoAsignado.Item1)
            {
                return ("NO_ASIGNADO", null);
            }
            var turno = turnoAsignado.Item2.Turno;

            // Definir los rangos permitidos con desviación de 10 minutos
            var margen = TimeSpan.FromMinutes(10);
            // Hora de entrada permitida
            var horaFinEsperada = turno.HoraFin;

            var entradaMin = horaFinEsperada - margen;
            var entradaMax = horaFinEsperada + margen;

            if (horaMarcada < entradaMin)
            {
                // La hora de entrada es antes del rango permitido
                return ("ANTICIPADO", turno);
            }
            if (horaMarcada > entradaMax)
            {
                // La hora de entrada es después del rango permitido
                return ("TARDANZA", turno);
            }
            return ("PUNTUAL", turno);
        }

        public async Task<(bool, UsuarioTurno)> VerificarTurnosAsignados()
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();
            var usuarioTurno = await _context.UsuarioTurno
                .Include(ut => ut.Turno)
                .FirstOrDefaultAsync(ut => ut.UsuarioId == userPrincipal.Id);
            if (usuarioTurno == null) return (false, null);
            return (true, usuarioTurno);
        }

        public async Task<DateTime> GetFechaHoraActual(DateTime fecha)
        {
            // Obtener la hora actual en la zona horaria de Lima
            var limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);
            fecha = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
            return fecha;

        }




        public List<Asistencia> GetAllAsistencias()
        {
            return _context.Asistencia.ToList();
        }

        public async Task<Asistencia> GetAllAsistenciaById(int id)
        {
            return await _context.Asistencia.FindAsync(id);
        }

        public async Task<List<Asistencia>> GetAllAsistenciaByIdEmpleado(string id)
        {
            return await _context.Asistencia.Where(a => a.IdEmpleado == id).ToListAsync();
        }

        public void UpdateAsistencia(Asistencia asistencia)
        {
            _context.Asistencia.Update(asistencia);
            _context.SaveChanges();
        }
        public void DeleteAsistencia(int id)
        {
            var asistencia = _context.Asistencia.Find(id);
            if (asistencia != null)
            {
                _context.Asistencia.Remove(asistencia);
                _context.SaveChanges();
            }
        }

        public async Task<bool> VerificarTiempoPermanencia(DateTime horaEntrada, DateTime horaSalida)
        {
            // Definir el tiempo mínimo requerido
            var tiempoMinimo = TimeSpan.FromMinutes(10);

            // Calcular la diferencia entre la hora de entrada y la hora de salida
            var diferencia = horaSalida - horaEntrada;

            // Verificar si la diferencia es mayor o igual al tiempo mínimo requerido
            return diferencia >= tiempoMinimo;
        }



    }
}