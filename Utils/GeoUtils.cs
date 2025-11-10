using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Geolocalizacion.Utils
{
    public class GeoUtils
    {
        public static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371e3; // radio de la Tierra en metros

            var φ1 = DegreesToRadians(lat1);
            var φ2 = DegreesToRadians(lat2);
            var Δφ = DegreesToRadians(lat2 - lat1);
            var Δλ = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // en metros
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}