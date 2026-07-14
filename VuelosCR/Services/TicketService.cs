using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VuelosCRSA.Models;

namespace VuelosCRSA.Services
{
    public class TicketService
    {
        public bool TryCreateTicket(
            string cedula,
            string nombre,
            string destino,
            string aerolinea,
            string origen,
            string clase,
            string tipoViaje,
            DateTime fechaVuelo,
            DateTime fechaVuelta,
            IDictionary<string, decimal> destinos,
            decimal porcentajeServicio,
            decimal origenTarifa,
            decimal airlineFactor,
            decimal claseMultiplier,
            out Tiquete tiquete,
            out string errorMessage,
            string language = "es")
        {
            tiquete = null;

            if (string.IsNullOrWhiteSpace(cedula))
            {
                errorMessage = GetMessage(language, "ID is required.", "La cédula es obligatoria.");
                return false;
            }

            if (!Regex.IsMatch(cedula.Trim(), "^[0-9-]+$"))
            {
                errorMessage = GetMessage(language, "ID can only contain numbers and hyphens.", "La cédula solo puede contener números y guiones.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                errorMessage = GetMessage(language, "Name is required.", "El nombre es obligatorio.");
                return false;
            }

            if (!Regex.IsMatch(nombre.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s'-]+$"))
            {
                errorMessage = GetMessage(language, "Name contains invalid characters.", "El nombre contiene caracteres inválidos.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(origen))
            {
                errorMessage = GetMessage(language, "Select an origin.", "Seleccione un origen.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(destino))
            {
                errorMessage = GetMessage(language, "Select a destination.", "Seleccione un destino.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(aerolinea))
            {
                errorMessage = GetMessage(language, "Select an airline.", "Seleccione una aerolínea.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(clase))
            {
                errorMessage = GetMessage(language, "Select a class.", "Seleccione una clase.");
                return false;
            }

            if (origen == destino)
            {
                errorMessage = GetMessage(language, "Origin and destination must be different.", "El origen y destino deben ser diferentes.");
                return false;
            }

            decimal precioDestino = 0m;
            if (destinos != null)
            {
                destinos.TryGetValue(destino, out precioDestino);
            }

            var seasonMultiplier = GetSeasonMultiplier(fechaVuelo);
            var legPrice = Math.Round((origenTarifa + precioDestino) * airlineFactor * claseMultiplier, 2);
            var totalBase = legPrice * seasonMultiplier;

            if (tipoViaje == "Ida y Vuelta")
            {
                var returnMultiplier = GetSeasonMultiplier(fechaVuelta);
                totalBase += legPrice * returnMultiplier;
            }

            tiquete = new Tiquete
            {
                Cliente = new Cliente
                {
                    Cedula = cedula.Trim(),
                    Nombre = nombre.Trim()
                },
                Origen = origen,
                Destino = destino,
                Aerolinea = aerolinea,
                Clase = clase,
                TipoViaje = tipoViaje,
                FechaVuelo = fechaVuelo,
                FechaVuelta = fechaVuelta,
                PrecioBase = totalBase,
                PorcentajeServicio = porcentajeServicio
            };

            errorMessage = null;
            return true;
        }

        private static decimal GetSeasonMultiplier(DateTime fecha)
        {
            var m = fecha.Month;
            if (m == 12 || m == 1 || m == 7)
                return 1.30m;
            if (m == 2 || m == 6 || m == 11)
                return 1.15m;
            return 1.0m;
        }

        private string GetMessage(string language, string english, string spanish)
        {
            return language == "en" ? english : spanish;
        }
    }
}
