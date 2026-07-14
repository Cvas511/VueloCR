using System;

namespace VuelosCRSA.Models
{
    public class Tiquete    
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public Cliente Cliente { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Aerolinea { get; set; }
        public string Clase { get; set; }
        public string TipoViaje { get; set; } = "Ida";
        public DateTime FechaVuelo { get; set; } = DateTime.Today;
        public DateTime FechaVuelta { get; set; } = DateTime.Today.AddDays(7);
        public decimal PrecioBase { get; set; }
        public decimal PorcentajeServicio { get; set; }

        public decimal CalcularMontoServicio()
        {
            return Math.Round(PrecioBase * PorcentajeServicio, 2);
        }

        public decimal CalcularMontoIVA()
        {
            decimal subtotal = PrecioBase + CalcularMontoServicio();
            return Math.Round(subtotal * 0.13m, 2);
        }

        public decimal CalcularPrecioFinalTiquete()
        {
            var montoServicio = CalcularMontoServicio();
            var subtotal = PrecioBase + montoServicio;
            var montoIva = Math.Round(subtotal * 0.13m, 2);
            return subtotal + montoIva;
        }
    }
}
