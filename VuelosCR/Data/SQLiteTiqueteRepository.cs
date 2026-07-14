using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using VuelosCRSA.Models;

namespace VuelosCRSA.Data
{
    public class SQLiteTiqueteRepository : ITiqueteRepository
    {
        private readonly string _connectionString;

        public SQLiteTiqueteRepository(string dbPath)
        {
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                        string sql = @"
                    CREATE TABLE IF NOT EXISTS Tiquetes (
                        Id TEXT PRIMARY KEY,
                        Cedula TEXT NOT NULL,
                        Nombre TEXT NOT NULL,
                        Origen TEXT NOT NULL,
                        Destino TEXT NOT NULL,
                        Aerolinea TEXT NOT NULL,
                        Clase TEXT NOT NULL,
                        TipoViaje TEXT NOT NULL DEFAULT 'Ida',
                        FechaVuelo TEXT NOT NULL,
                        FechaVuelta TEXT NOT NULL DEFAULT '',
                        PrecioBase DECIMAL NOT NULL
                    );";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                try
                {
                    using (var checkCmd = new SQLiteCommand("PRAGMA table_info(Tiquetes);", connection))
                    using (var reader = checkCmd.ExecuteReader())
                    {
                        bool hasOrigen = false, hasClase = false, hasFecha = false, hasTipoViaje = false, hasFechaVuelta = false;
                        while (reader.Read())
                        {
                            var name = reader["name"].ToString();
                            if (name.Equals("Origen", StringComparison.OrdinalIgnoreCase)) hasOrigen = true;
                            if (name.Equals("Clase", StringComparison.OrdinalIgnoreCase)) hasClase = true;
                            if (name.Equals("FechaVuelo", StringComparison.OrdinalIgnoreCase)) hasFecha = true;
                            if (name.Equals("TipoViaje", StringComparison.OrdinalIgnoreCase)) hasTipoViaje = true;
                            if (name.Equals("FechaVuelta", StringComparison.OrdinalIgnoreCase)) hasFechaVuelta = true;
                        }
                        if (!hasOrigen)
                        {
                            using (var alterCmd = new SQLiteCommand("ALTER TABLE Tiquetes ADD COLUMN Origen TEXT NOT NULL DEFAULT '';", connection))
                            {
                                alterCmd.ExecuteNonQuery();
                            }
                        }
                        if (!hasClase)
                        {
                            using (var alterCmd = new SQLiteCommand("ALTER TABLE Tiquetes ADD COLUMN Clase TEXT NOT NULL DEFAULT '';", connection))
                            {
                                alterCmd.ExecuteNonQuery();
                            }
                        }
                        if (!hasFecha)
                        {
                            using (var alterCmd = new SQLiteCommand("ALTER TABLE Tiquetes ADD COLUMN FechaVuelo TEXT NOT NULL DEFAULT '';", connection))
                            {
                                alterCmd.ExecuteNonQuery();
                            }
                        }
                        if (!hasTipoViaje)
                        {
                            using (var alterCmd = new SQLiteCommand("ALTER TABLE Tiquetes ADD COLUMN TipoViaje TEXT NOT NULL DEFAULT 'Ida';", connection))
                            {
                                alterCmd.ExecuteNonQuery();
                            }
                        }
                        if (!hasFechaVuelta)
                        {
                            using (var alterCmd = new SQLiteCommand("ALTER TABLE Tiquetes ADD COLUMN FechaVuelta TEXT NOT NULL DEFAULT '';", connection))
                            {
                                alterCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void Insert(Tiquete t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (string.IsNullOrWhiteSpace(t.Id)) t.Id = Guid.NewGuid().ToString("N");

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Tiquetes (Id, Cedula, Nombre, Origen, Destino, Aerolinea, Clase, TipoViaje, FechaVuelo, FechaVuelta, PrecioBase)
                    VALUES (@Id, @Cedula, @Nombre, @Origen, @Destino, @Aerolinea, @Clase, @TipoViaje, @FechaVuelo, @FechaVuelta, @PrecioBase);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", t.Id);
                    command.Parameters.AddWithValue("@Cedula", t.Cliente?.Cedula ?? "");
                    command.Parameters.AddWithValue("@Nombre", t.Cliente?.Nombre ?? "");
                    command.Parameters.AddWithValue("@Origen", t.Origen ?? "");
                    command.Parameters.AddWithValue("@Destino", t.Destino);
                    command.Parameters.AddWithValue("@Aerolinea", t.Aerolinea);
                    command.Parameters.AddWithValue("@Clase", t.Clase ?? "");
                    command.Parameters.AddWithValue("@TipoViaje", t.TipoViaje ?? "Ida");
                    command.Parameters.AddWithValue("@FechaVuelo", t.FechaVuelo.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@FechaVuelta", t.FechaVuelta.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@PrecioBase", t.PrecioBase);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Tiquete> GetAll()
        {
            var list = new List<Tiquete>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Tiquetes;";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Tiquete
                        {
                            Id = reader["Id"].ToString(),
                            Cliente = new Cliente 
                            { 
                                Cedula = reader["Cedula"].ToString(), 
                                Nombre = reader["Nombre"].ToString() 
                            },
                            Origen = reader["Origen"].ToString(),
                            Destino = reader["Destino"].ToString(),
                            Aerolinea = reader["Aerolinea"].ToString(),
                            Clase = reader["Clase"].ToString(),
                            TipoViaje = reader["TipoViaje"]?.ToString() ?? "Ida",
                            FechaVuelo = DateTime.TryParse(reader["FechaVuelo"]?.ToString(), out var f) ? f : DateTime.Today,
                            FechaVuelta = DateTime.TryParse(reader["FechaVuelta"]?.ToString(), out var fv) ? fv : DateTime.Today.AddDays(7),
                            PrecioBase = Convert.ToDecimal(reader["PrecioBase"])
                        });
                    }
                }
            }
            return list;
        }

        public void Delete(Tiquete t)
        {
            if (t == null) return;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Tiquetes WHERE Id = @Id;";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", t.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Clear()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Tiquetes;";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSavedData()
        {
            Clear();
        }

        public void Update(Tiquete t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
                    UPDATE Tiquetes SET
                        Cedula = @Cedula,
                        Nombre = @Nombre,
                        Origen = @Origen,
                        Destino = @Destino,
                        Aerolinea = @Aerolinea,
                        Clase = @Clase,
                        TipoViaje = @TipoViaje,
                        FechaVuelo = @FechaVuelo,
                        FechaVuelta = @FechaVuelta,
                        PrecioBase = @PrecioBase
                    WHERE Id = @Id;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", t.Id);
                    command.Parameters.AddWithValue("@Cedula", t.Cliente?.Cedula ?? "");
                    command.Parameters.AddWithValue("@Nombre", t.Cliente?.Nombre ?? "");
                    command.Parameters.AddWithValue("@Origen", t.Origen ?? "");
                    command.Parameters.AddWithValue("@Destino", t.Destino);
                    command.Parameters.AddWithValue("@Aerolinea", t.Aerolinea);
                    command.Parameters.AddWithValue("@Clase", t.Clase ?? "");
                    command.Parameters.AddWithValue("@TipoViaje", t.TipoViaje ?? "Ida");
                    command.Parameters.AddWithValue("@FechaVuelo", t.FechaVuelo.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@FechaVuelta", t.FechaVuelta.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@PrecioBase", t.PrecioBase);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteLast()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Tiquetes WHERE rowid = (SELECT MAX(rowid) FROM Tiquetes);";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
