using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trainingApp.Modelo;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Shapes;

namespace trainingApp
{
    public class DatabaseManager
    {
        private readonly string connectionString = "Filename=trainingApp.db";

        public async Task InitializeDatabaseAsync()
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                // Crear tabla Ejercicio
                String createEjercicioTable = "CREATE TABLE IF NOT EXISTS Ejercicio (" +
                                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                "Nombre NVARCHAR(100), " +
                                "Descripcion NVARCHAR(250), " +
                                "Repeticiones INTEGER, " +
                                "Series INTEGER, " +
                                "RutinaId INTEGER, " +
                                "FOREIGN KEY (RutinaId) REFERENCES Rutina(Id))";

                SqliteCommand createEjercicioCmd = new SqliteCommand(createEjercicioTable, db);
                await createEjercicioCmd.ExecuteNonQueryAsync();

                // Crear tabla Rutina
                String createRutinaTable = "CREATE TABLE IF NOT EXISTS Rutina (" +
                                                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                "DiaDeSemana NVARCHAR(50))";

                SqliteCommand createRutinaCmd = new SqliteCommand(createRutinaTable, db);
                await createRutinaCmd.ExecuteNonQueryAsync();

                // Cerrar conexión
                db.Close();
            }
        }

        public async Task InsertEjercicioAsync(Ejercicio ejercicio)
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                String insertEjercicioCommand = "INSERT INTO Ejercicio (Nombre, Descripcion, Repeticiones, Series) VALUES (@Nombre, @Descripcion, @Repeticiones, @Series)";
                SqliteCommand insertEjercicioCmd = new SqliteCommand(insertEjercicioCommand, db);
                insertEjercicioCmd.Parameters.AddWithValue("@Nombre", ejercicio.Nombre);
                insertEjercicioCmd.Parameters.AddWithValue("@Descripcion", ejercicio.Descripcion);
                insertEjercicioCmd.Parameters.AddWithValue("@Repeticiones", ejercicio.Repeticiones);
                insertEjercicioCmd.Parameters.AddWithValue("@Series", ejercicio.Series);

                await insertEjercicioCmd.ExecuteNonQueryAsync();

                db.Close();
            }
        }

        public async Task InsertRutinaAsync(Rutina rutina)
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                String insertRutinaCommand = "INSERT INTO Rutina (DiaDeSemana) VALUES (@DiaDeSemana)";
                SqliteCommand insertRutinaCmd = new SqliteCommand(insertRutinaCommand, db);
                insertRutinaCmd.Parameters.AddWithValue("@DiaDeSemana", rutina.DiaDeSemana);

                await insertRutinaCmd.ExecuteNonQueryAsync();

                // Obtener el ID insertado
                String selectLastInsertRowId = "SELECT last_insert_rowid()";
                SqliteCommand selectLastInsertRowIdCmd = new SqliteCommand(selectLastInsertRowId, db);
                Int64 lastInsertRowId64 = (Int64)await selectLastInsertRowIdCmd.ExecuteScalarAsync();
                int lastInsertRowId = (int)lastInsertRowId64;
                rutina.Id = lastInsertRowId;

                db.Close();
            }
        }

        public async Task<ObservableCollection<Rutina>> RetrieveRutinasAsync()
        {
            ObservableCollection<Rutina> rutinas = new ObservableCollection<Rutina>();

            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                String selectRutinasCommand = "SELECT Id, DiaDeSemana FROM Rutina";
                SqliteCommand selectRutinasCmd = new SqliteCommand(selectRutinasCommand, db);

                SqliteDataReader query = await selectRutinasCmd.ExecuteReaderAsync();
                while (query.Read())
                {
                    Rutina rutina = new Rutina
                    {
                        Id = query.GetInt32(0),
                        DiaDeSemana = query.GetString(1),
                        Ejercicios = await RetrieveEjerciciosForRutinaAsync(query.GetInt32(0))
                    };
                    rutinas.Add(rutina);
                }

                db.Close();
            }

            return rutinas;
        }

        public ObservableCollection<Rutina> GetRutinas()
        {
            var rutinas = new ObservableCollection<Rutina>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var selectCommand = new SqliteCommand("SELECT * FROM Rutina", connection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var rutina = new Rutina
                        {
                            Id = reader.GetInt32(0),
                            DiaDeSemana = reader.GetString(1),
                            Ejercicios = new ObservableCollection<Ejercicio>()
                        };

                        var ejerciciosCommand = new SqliteCommand("SELECT * FROM Ejercicio WHERE RutinaId = @RutinaId", connection);
                        ejerciciosCommand.Parameters.AddWithValue("@RutinaId", rutina.Id);
                        using (var ejerciciosReader = ejerciciosCommand.ExecuteReader())
                        {
                            while (ejerciciosReader.Read())
                            {
                                var ejercicio = new Ejercicio
                                {
                                    Id = ejerciciosReader.GetInt32(0),
                                    Nombre = ejerciciosReader.GetString(1),
                                    Descripcion = ejerciciosReader.GetString(2),
                                    Repeticiones = ejerciciosReader.GetInt32(3),
                                    Series = ejerciciosReader.GetInt32(4)
                                };
                                rutina.Ejercicios.Add(ejercicio);
                            }
                        }
                        rutinas.Add(rutina);
                    }
                }
            }
            return rutinas;
        }

        private async Task<ICollection<Ejercicio>> RetrieveEjerciciosForRutinaAsync(int rutinaId)
        {
            List<Ejercicio> ejercicios = new List<Ejercicio>();

            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                String selectEjerciciosCommand = "SELECT Id, Nombre, Descripcion, Repeticiones, Series FROM Ejercicio WHERE RutinaId = @RutinaId";
                SqliteCommand selectEjerciciosCmd = new SqliteCommand(selectEjerciciosCommand, db);
                selectEjerciciosCmd.Parameters.AddWithValue("@RutinaId", rutinaId);

                SqliteDataReader query = await selectEjerciciosCmd.ExecuteReaderAsync();
                while (query.Read())
                {
                    Ejercicio ejercicio = new Ejercicio
                    {
                        Id = query.GetInt32(0),
                        Nombre = query.GetString(1),
                        Descripcion = query.GetString(2),
                        Repeticiones = query.GetInt32(3),
                        Series = query.GetInt32(4)
                    };
                    ejercicios.Add(ejercicio);
                }

                db.Close();
            }

            return ejercicios;
        }
    }
}
