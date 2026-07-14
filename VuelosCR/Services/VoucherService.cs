using VuelosCRSA.Models;

namespace VuelosCRSA.Services
{
    public class VoucherService
    {
        public string BuildVoucherText(Tiquete tiquete, string language)
        {
            if (tiquete == null)
            {
                return string.Empty;
            }

            if (language == "en")
            {
                return $"Voucher:\nClient: {tiquete.Cliente.Nombre} ({tiquete.Cliente.Cedula})\nTrip: {tiquete.TipoViaje}\nDate: {tiquete.FechaVuelo:yyyy-MM-dd}" +
                    (tiquete.TipoViaje == "Ida y Vuelta" ? $"\nReturn: {tiquete.FechaVuelta:yyyy-MM-dd}" : "") +
                    $"\nDestination: {tiquete.Destino}\nOrigin: {tiquete.Origen}\nAirline: {tiquete.Aerolinea}\nClass: {tiquete.Clase}\nTotal: ${tiquete.CalcularPrecioFinalTiquete():N2}";
            }

            return $"Voucher:\nCliente: {tiquete.Cliente.Nombre} ({tiquete.Cliente.Cedula})\nTipo: {tiquete.TipoViaje}\nFecha: {tiquete.FechaVuelo:yyyy-MM-dd}" +
                (tiquete.TipoViaje == "Ida y Vuelta" ? $"\nVuelta: {tiquete.FechaVuelta:yyyy-MM-dd}" : "") +
                $"\nDestino: {tiquete.Destino}\nOrigen: {tiquete.Origen}\nAerolínea: {tiquete.Aerolinea}\nClase: {tiquete.Clase}\nTotal: ${tiquete.CalcularPrecioFinalTiquete():N2}";
        }

        public string BuildPurchaseConfirmation(string language)
        {
            return language == "en"
                ? "Purchase recorded. Voucher printed on screen."
                : "Compra registrada. Voucher impreso en pantalla.";
        }
    }
}
