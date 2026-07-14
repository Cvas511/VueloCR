using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using VuelosCRSA.Models;

namespace VuelosCRSA.Data
{
    public class TiqueteRepository : ITiqueteRepository
    {
        private readonly string _dbPath;
        private const string HeaderLine = "Id,Cedula,Nombre,Destino,Aerolinea,PrecioBase,MontoServicio,MontoIVA,PrecioFinal";

        public TiqueteRepository(string dbPath)
        {
            _dbPath = dbPath;
            EnsureStorage();
        }

        private void EnsureStorage()
        {
            var dir = Path.GetDirectoryName(_dbPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(_dbPath))
            {
                File.WriteAllText(_dbPath, HeaderLine + Environment.NewLine);
            }
        }

        public void Insert(Tiquete t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (string.IsNullOrWhiteSpace(t.Id)) t.Id = Guid.NewGuid().ToString("N");
            var line = string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                Escape(t.Id),
                Escape(t.Cliente?.Cedula),
                Escape(t.Cliente?.Nombre),
                Escape(t.Destino),
                Escape(t.Aerolinea),
                FormatDecimal(t.PrecioBase),
                FormatDecimal(t.CalcularMontoServicio()),
                FormatDecimal(t.CalcularMontoIVA()),
                FormatDecimal(t.CalcularPrecioFinalTiquete())
            );
            File.AppendAllText(_dbPath, line + Environment.NewLine);
        }

        public IEnumerable<Tiquete> GetAll()
        {
            var list = new List<Tiquete>();
            if (!File.Exists(_dbPath)) return list;
            foreach (var ln in File.ReadLines(_dbPath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(ln)) continue;
                var parts = SplitCsvLine(ln);
                if (parts.Length < 8) continue;
                list.Add(ParseTicket(parts));
            }
            return list;
        }

        public void Delete(Tiquete t)
        {
            if (t == null) return;
            var tickets = GetAll().Where(item => !SameTicket(item, t)).ToList();
            SaveAll(tickets);
        }

        public void Clear()
        {
            SaveAll(new List<Tiquete>());
        }

        public void DeleteSavedData()
        {
            if (File.Exists(_dbPath))
            {
                File.Delete(_dbPath);
            }

            EnsureStorage();
        }

        public void DeleteLast()
        {
            var tickets = GetAll().ToList();
            if (tickets.Count == 0)
            {
                return;
            }

            tickets.RemoveAt(tickets.Count - 1);
            SaveAll(tickets);
        }

        public void Update(Tiquete t)
        {
            if (t == null) return;
            var tickets = GetAll().ToList();
            var idx = tickets.FindIndex(item => SameTicket(item, t));
            if (idx >= 0)
            {
                tickets[idx] = t;
                SaveAll(tickets);
            }
        }

        private void SaveAll(IEnumerable<Tiquete> tickets)
        {
            var sb = new StringBuilder();
            sb.AppendLine(HeaderLine);
            foreach (var ticket in tickets)
            {
                // Al reescribir datos antiguos, se les asigna Id y quedan migrados.
                if (string.IsNullOrWhiteSpace(ticket.Id)) ticket.Id = Guid.NewGuid().ToString("N");
                sb.AppendLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    Escape(ticket.Id),
                    Escape(ticket.Cliente?.Cedula),
                    Escape(ticket.Cliente?.Nombre),
                    Escape(ticket.Destino),
                    Escape(ticket.Aerolinea),
                    FormatDecimal(ticket.PrecioBase),
                    FormatDecimal(ticket.CalcularMontoServicio()),
                    FormatDecimal(ticket.CalcularMontoIVA()),
                    FormatDecimal(ticket.CalcularPrecioFinalTiquete())));
            }

            File.WriteAllText(_dbPath, sb.ToString());
        }

        private Tiquete ParseTicket(string[] parts)
        {
            // También admite archivos creados por versiones anteriores (sin Id).
            var offset = parts.Length >= 9 ? 1 : 0;
            return new Tiquete()
            {
                Id = offset == 1 ? Unescape(parts[0]) : string.Empty,
                Cliente = new Cliente() { Cedula = Unescape(parts[offset]), Nombre = Unescape(parts[offset + 1]) },
                Destino = Unescape(parts[offset + 2]),
                Aerolinea = Unescape(parts[offset + 3]),
                PrecioBase = decimal.TryParse(parts[offset + 4], NumberStyles.Any, CultureInfo.InvariantCulture, out var pb) ? pb : 0m
            };
        }

        private string FormatDecimal(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private bool SameTicket(Tiquete left, Tiquete right)
        {
            if (left == null || right == null) return false;
            if (!string.IsNullOrWhiteSpace(left.Id) && !string.IsNullOrWhiteSpace(right.Id))
            {
                return string.Equals(left.Id, right.Id, StringComparison.OrdinalIgnoreCase);
            }
            return string.Equals(left.Cliente?.Cedula, right.Cliente?.Cedula, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.Cliente?.Nombre, right.Cliente?.Nombre, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.Destino, right.Destino, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.Aerolinea, right.Aerolinea, StringComparison.OrdinalIgnoreCase)
                && left.PrecioBase == right.PrecioBase;
        }

        private string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (s.Contains(",") || s.Contains("\n") || s.Contains("\r") || s.Contains("\""))
            {
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            }
            return s;
        }

        private string Unescape(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            s = s.Trim();
            if (s.StartsWith("\"") && s.EndsWith("\""))
            {
                var inner = s.Substring(1, s.Length - 2);
                return inner.Replace("\"\"", "\"");
            }
            return s;
        }

        // división muy simple que soporta campos entrecomillados
        private string[] SplitCsvLine(string line)
        {
            var parts = new System.Collections.Generic.List<string>();
            bool inQuotes = false;
            var cur = new System.Text.StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (ch == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // doble comilla -> comilla literal
                        cur.Append('"');
                        i++; // saltar siguiente
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (ch == ',' && !inQuotes)
                {
                    parts.Add(cur.ToString());
                    cur.Clear();
                }
                else
                {
                    cur.Append(ch);
                }
            }
            parts.Add(cur.ToString());
            return parts.ToArray();
        }
    }

}
