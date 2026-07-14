using System.Collections.Generic;

namespace VuelosCRSA.Models
{
    public class AppConfig
    {
        public List<DestinoConfig> Destinos { get; set; }
        public List<AerolineaConfig> Aerolineas { get; set; }
        public List<OrigenConfig> Origenes { get; set; }
    }

    public class DestinoConfig
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }

    public class AerolineaConfig
    {
        public string Nombre { get; set; }
        public decimal PorcentajeServicio { get; set; }
        public decimal FactorPrecio { get; set; } = 1.0m;
        public Dictionary<string, decimal> ClasesDisponibles { get; set; }
    }

    public class OrigenConfig
    {
        public string Nombre { get; set; }
        public decimal TarifaSalida { get; set; }
    }
}
