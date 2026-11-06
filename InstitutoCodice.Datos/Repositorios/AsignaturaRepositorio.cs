using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de asignaturas, con listado general y por docente.
    /// </summary>
    public class AsignaturaRepositorio : RepositorioBase
    {
        private const string SqlBase = @"
            SELECT a.Id, a.Codigo, a.Nombre, a.Curso, a.DocenteId, u.Nombre AS NombreDocente
            FROM Asignatura a
            INNER JOIN Usuario u ON u.Id = a.DocenteId";

        public List<Asignatura> Listar()
        {
            return Consultar(SqlBase + " ORDER BY a.Nombre", null);
        }

        public List<Asignatura> ListarPorDocente(int docenteId)
        {
            return Consultar(SqlBase + " WHERE a.DocenteId = @DocenteId ORDER BY a.Nombre", docenteId);
        }

        /// <summary>
        /// Retorna todas las asignaturas para administrador y solo las propias para docente.
        /// </summary>
        public List<Asignatura> ListarParaUsuario(Usuario usuario)
        {
            if (usuario is Docente)
                return ListarPorDocente(usuario.Id);

            return Listar();
        }

        public void Agregar(Asignatura asignatura)
        {
            const string sql = @"
                INSERT INTO Asignatura (Codigo, Nombre, Curso, DocenteId)
                VALUES (@Codigo, @Nombre, @Curso, @DocenteId);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, asignatura);
                    asignatura.Id = (int)comando.ExecuteScalar();
                }
            });
        }

        public void Actualizar(Asignatura asignatura)
        {
            const string sql = @"
                UPDATE Asignatura
                SET Codigo = @Codigo, Nombre = @Nombre, Curso = @Curso, DocenteId = @DocenteId
                WHERE Id = @Id";

            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, asignatura);
                    comando.Parameters.AddWithValue("@Id", asignatura.Id);
                    comando.ExecuteNonQuery();
                }
            });
        }

        private List<Asignatura> Consultar(string sql, int? docenteId)
        {
            return Ejecutar(conexion =>
            {
                List<Asignatura> lista = new List<Asignatura>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    if (docenteId.HasValue)
                        comando.Parameters.AddWithValue("@DocenteId", docenteId.Value);

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                            lista.Add(Mapeo.LeerAsignatura(lector));
                    }
                }
                return lista;
            });
        }

        private static void AgregarParametros(SqlCommand comando, Asignatura asignatura)
        {
            comando.Parameters.AddWithValue("@Codigo", asignatura.Codigo);
            comando.Parameters.AddWithValue("@Nombre", asignatura.Nombre);
            comando.Parameters.AddWithValue("@Curso", asignatura.Curso);
            comando.Parameters.AddWithValue("@DocenteId", asignatura.Docente.Id);
        }
    }
}