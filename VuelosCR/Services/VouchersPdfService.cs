using System;
using System.Globalization;
using System.IO;
using System.Text;
using VuelosCRSA.Models;

namespace VuelosCRSA.Services
{
    /// <summary>Genera comprobantes PDF autónomos, sin depender de software externo.</summary>
    public class VouchersPdfService
    {
        public string SaveVoucherPdf(Tiquete ticket, string outputDir, int voucherNumber, string language)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            var filePath = Path.Combine(outputDir, $"Voucher_{voucherNumber:D6}.pdf");
            var contentBytes = Encoding.GetEncoding(1252).GetBytes(BuildPageContent(ticket, voucherNumber, language));

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.ASCII, 1024, true))
            {
                writer.NewLine = "\n";
                writer.WriteLine("%PDF-1.4");
                var offsets = new long[7];
                WriteObject(writer, offsets, 1, "<< /Type /Catalog /Pages 2 0 R >>");
                WriteObject(writer, offsets, 2, "<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
                WriteObject(writer, offsets, 3, "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 5 0 R /F2 6 0 R >> >> /Contents 4 0 R >>");

                writer.Flush();
                offsets[4] = stream.Position;
                writer.WriteLine("4 0 obj");
                writer.WriteLine("<< /Length " + contentBytes.Length + " >>");
                writer.WriteLine("stream");
                writer.Flush();
                stream.Write(contentBytes, 0, contentBytes.Length);
                writer.WriteLine();
                writer.WriteLine("endstream");
                writer.WriteLine("endobj");
                WriteObject(writer, offsets, 5, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");
                WriteObject(writer, offsets, 6, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>");
                writer.Flush();

                var xrefOffset = stream.Position;
                writer.WriteLine("xref");
                writer.WriteLine("0 7");
                writer.WriteLine("0000000000 65535 f ");
                for (var i = 1; i <= 6; i++) writer.WriteLine(offsets[i].ToString("D10", CultureInfo.InvariantCulture) + " 00000 n ");
                writer.WriteLine("trailer");
                writer.WriteLine("<< /Size 7 /Root 1 0 R >>");
                writer.WriteLine("startxref");
                writer.WriteLine(xrefOffset.ToString(CultureInfo.InvariantCulture));
                writer.WriteLine("%%EOF");
                writer.Flush();
                File.WriteAllBytes(filePath, stream.ToArray());
            }
            return filePath;
        }

        private static void WriteObject(StreamWriter writer, long[] offsets, int number, string body)
        {
            writer.Flush();
            offsets[number] = writer.BaseStream.Position;
            writer.WriteLine(number + " 0 obj");
            writer.WriteLine(body);
            writer.WriteLine("endobj");
        }

        private static string BuildPageContent(Tiquete ticket, int voucherNumber, string language)
        {
            var isEnglish = language == "en";
            var culture = CultureInfo.GetCultureInfo("es-CR");
            var lines = new[]
            {
                isEnglish ? "VuelosCR.SA - Flight ticket" : "VuelosCR.SA - Tiquete aéreo",
                (isEnglish ? "Voucher number: " : "Número de comprobante: ") + voucherNumber.ToString("D6"),
                (isEnglish ? "Passenger: " : "Pasajero: ") + (ticket.Cliente?.Nombre ?? "-"),
                (isEnglish ? "Identification: " : "Cédula: ") + (ticket.Cliente?.Cedula ?? "-"),
                (isEnglish ? "Origin: " : "Origen: ") + (ticket.Origen ?? "-"),
                (isEnglish ? "Destination: " : "Destino: ") + (ticket.Destino ?? "-"),
                (isEnglish ? "Airline: " : "Aerolínea: ") + (ticket.Aerolinea ?? "-"),
                (isEnglish ? "Class: " : "Clase: ") + (ticket.Clase ?? "-"),
                (isEnglish ? "Trip: " : "Tipo: ") + (ticket.TipoViaje ?? "Ida"),
                (isEnglish ? "Date: " : "Fecha: ") + ticket.FechaVuelo.ToString("yyyy-MM-dd"),
                (ticket.TipoViaje == "Ida y Vuelta"
                    ? (isEnglish ? "Return: " : "Vuelta: ") + ticket.FechaVuelta.ToString("yyyy-MM-dd")
                    : ""),
                (isEnglish ? "Base fare: $" : "Tarifa base: $") + ticket.PrecioBase.ToString("N2", culture),
                (isEnglish ? "Service fee: $" : "Servicio: $") + ticket.CalcularMontoServicio().ToString("N2", culture),
                (isEnglish ? "VAT (13%): $" : "IVA (13%): $") + ticket.CalcularMontoIVA().ToString("N2", culture),
                (isEnglish ? "TOTAL: $" : "TOTAL: $") + ticket.CalcularPrecioFinalTiquete().ToString("N2", culture),
                isEnglish ? "Thank you for flying with VuelosCR.SA." : "Gracias por viajar con VuelosCR.SA."
            };
            var sb = new StringBuilder("q\n0.15 0.65 0.60 rg 45 770 505 35 re f\n0 0 0 rg\n");
            var lineIndex = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                var isTitle = i == 0 || i == lines.Length - 1;
                var y = lineIndex == 0 ? 782 : 730 - ((lineIndex - 1) * 36);
                sb.Append("BT /").Append(isTitle ? "F2" : "F1").Append(isTitle ? " 18" : " 12")
                    .Append(" Tf 55 ").Append(y.ToString(CultureInfo.InvariantCulture)).Append(" Td (")
                    .Append(EscapePdfText(lines[i])).AppendLine(") Tj ET");
                lineIndex++;
            }
            sb.AppendLine("0.75 0.75 0.75 RG 45 95 m 550 95 l S");
            sb.AppendLine("Q");
            return sb.ToString();
        }

        private static string EscapePdfText(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }
    }
}
