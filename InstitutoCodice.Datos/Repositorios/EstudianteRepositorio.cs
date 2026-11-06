using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de estudiantes, incluyendo registro y actualización.
    /// </summary>
    public class EstudianteRepositorio : RepositorioBase
    {
        public List<Estudiante> Listar()
        {
            const string sql = @"
                SELECT Id, Rut, Nombres, Apellidos, Correo, Curso, Activo
                FROM Estudiante
                ORDER BY Apellidos, Nombres";

            return Ejecutar(conexion =>
            {
                List<Estudiante> lista = new List<Estudiante>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                        lista.Add(Mapeo.LeerEstudiante(lector));
                }
                return lista;
            });
        }

        public void Agregar(Estudiante estudiante)
        {
            const string sql = @"
                INSERT INTO Estudiante (Rut, Nombres, Apellidos, Correo, Curso, Activo)
                VALUES (@Rut, @Nombres, @Apellidos, @Correo, @Curso, @Activo);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, estudiante);
                    estudiante.Id = (int)comando.ExecuteScalar();
                }
            });
        }

        public void Actualizar(Estudiante estudiante)
        {
            const string sql = @"
                UPDATE Estudiante
                SET Rut = @Rut, Nombres = @Nombres, Apellidos = @Apellidos,
                    Correo = @Correo, Curso = @Curso, Activo = @Activo
                WHERE Id = @Id";

            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, estudiante);
                    comando.Parameters.AddWithValue("@Id", estudiante.Id);
                    comando.ExecuteNonQuery();
                }
            });
        }

        private static void AgregarParametros(SqlCommand comando, Estudiante estudiante)
        {
            comando.Parameters.AddWithValue("@Rut", estudiante.Rut);
            comando.Parameters.AddWithValue("@Nombres", estudiante.Nombres);
            comando.Parameters.AddWithValue("@Apellidos", estudiante.Apellidos);
            comando.Parameters.AddWithValue("@Correo", ValorONulo(estudiante.Correo));
            comando.Parameters.AddWithValue("@Curso", estudiante.Curso);
            comando.Parameters.AddWithValue("@Activo", estudiante.Activo);
        }
    }
}