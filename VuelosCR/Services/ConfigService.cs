using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VuelosCRSA.Models;

namespace VuelosCRSA.Services
{
    public class ConfigService
    {
        public Dictionary<string, decimal> Destinos { get; private set; }
        public string[] DestinosNombres { get; private set; }
        public Dictionary<string, decimal> AerolineasPorcentajes { get; private set; }
        public Dictionary<string, decimal> AerolineasFactores { get; private set; }
        public string[] AerolineasNombres { get; private set; }
        public Dictionary<string, Dictionary<string, decimal>> AerolineasClases { get; private set; }
        public Dictionary<string, decimal> OrigenTarifas { get; private set; }
        public string[] OrigenesNombres { get; private set; }

        public ConfigService(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException($"No se encontró {jsonPath}");

            var json = File.ReadAllText(jsonPath);
            var config = JsonConvert.DeserializeObject<AppConfig>(json);

            Destinos = config.Destinos.ToDictionary(d => d.Nombre, d => d.Precio);
            DestinosNombres = config.Destinos.Select(d => d.Nombre).ToArray();
            AerolineasPorcentajes = config.Aerolineas.ToDictionary(a => a.Nombre, a => a.PorcentajeServicio);
            AerolineasFactores = config.Aerolineas.ToDictionary(a => a.Nombre, a => a.FactorPrecio);
            AerolineasNombres = config.Aerolineas.Select(a => a.Nombre).ToArray();

            AerolineasClases = new Dictionary<string, Dictionary<string, decimal>>();
            foreach (var a in config.Aerolineas)
            {
                AerolineasClases[a.Nombre] = a.ClasesDisponibles ?? new Dictionary<string, decimal>();
            }

            if (config.Origenes != null)
            {
                OrigenTarifas = config.Origenes.ToDictionary(o => o.Nombre, o => o.TarifaSalida);
                OrigenesNombres = config.Origenes.Select(o => o.Nombre).ToArray();
            }
            else
            {
                OrigenTarifas = new Dictionary<string, decimal>();
                OrigenesNombres = Array.Empty<string>();
            }
        }

        public decimal GetPorcentajeServicio(string aerolinea)
        {
            if (string.IsNullOrEmpty(aerolinea)) return 0m;
            return AerolineasPorcentajes.TryGetValue(aerolinea, out var p) ? p : 0m;
        }

        public decimal GetFactorPrecio(string aerolinea)
        {
            if (string.IsNullOrEmpty(aerolinea)) return 1m;
            return AerolineasFactores.TryGetValue(aerolinea, out var f) ? f : 1m;
        }

        public string[] GetClasesForAirline(string aerolinea)
        {
            if (string.IsNullOrEmpty(aerolinea) || !AerolineasClases.ContainsKey(aerolinea))
                return new[] { "Turista" };
            return AerolineasClases[aerolinea].Keys.ToArray();
        }

        public decimal GetClaseMultiplier(string aerolinea, string clase)
        {
            if (string.IsNullOrEmpty(aerolinea) || string.IsNullOrEmpty(clase))
                return 1m;
            if (AerolineasClases.TryGetValue(aerolinea, out var clases) && clases.TryGetValue(clase, out var m))
                return m;
            return 1m;
        }

        public decimal GetOrigenTarifa(string origen)
        {
            if (string.IsNullOrEmpty(origen)) return 0m;
            return OrigenTarifas.TryGetValue(origen, out var t) ? t : 0m;
        }
    }
}
