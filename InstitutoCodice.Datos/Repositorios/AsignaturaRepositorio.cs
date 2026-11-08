using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de asignaturas, con listado general y por docente.
    /// Cada alta y modificación queda en la bitácora, dentro de la misma transacción.
    /// </summary>
    public class AsignaturaRepositorio : RepositorioBase
    {
        private const string Entidad = "Asignatura";

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

        public void Agregar(Asignatura asignatura, Usuario autor)
        {
            const string sql = @"
                INSERT INTO Asignatura (Codigo, Nombre, Curso, DocenteId)
                VALUES (@Codigo, @Nombre, @Curso, @DocenteId);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    AgregarParametros(comando, asignatura);
                    asignatura.Id = (int)comando.ExecuteScalar();
                }

                string docente = NombreUsuarioDocente(asignatura.Docente.Id, conexion, transaccion);
                AuditoriaRepositorio.Registrar(autor, AccionAuditoria.AsignaturaCreada,
                    "Asignatura " + asignatura.Codigo + " - " + asignatura.Nombre + " (" + asignatura.Curso + ") creada.",
                    Entidad, asignatura.Id, null, "Docente: " + docente, conexion, transaccion);
            });
        }

        public void Actualizar(Asignatura asignatura, Usuario autor)
        {
            const string sqlAnterior = @"
                SELECT a.Codigo, a.Nombre, a.Curso, u.NombreUsuario
                FROM Asignatura a
                INNER JOIN Usuario u ON u.Id = a.DocenteId
                WHERE a.Id = @Id";
            const string sql = @"
                UPDATE Asignatura
                SET Codigo = @Codigo, Nombre = @Nombre, Curso = @Curso, DocenteId = @DocenteId
                WHERE Id = @Id";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                // 1. Datos actuales, leídos de la base
                string codigo, nombre, curso, docenteAnterior;
                using (SqlCommand comando = new SqlCommand(sqlAnterior, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Id", asignatura.Id);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (!lector.Read())
                            throw new InvalidOperationException("La asignatura ya no existe. Recargue la lista.");

                        codigo = lector.GetString(0);
                        nombre = lector.GetString(1);
                        curso = lector.GetString(2);
                        docenteAnterior = lector.GetString(3);
                    }
                }

                // 2. Guardar
                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    AgregarParametros(comando, asignatura);
                    comando.Parameters.AddWithValue("@Id", asignatura.Id);
                    comando.ExecuteNonQuery();
                }

                // 3. Auditar (una asignatura no contiene datos personales: se registran los valores)
                string docenteNuevo = NombreUsuarioDocente(asignatura.Docente.Id, conexion, transaccion);
                ResumenCambios cambios = CambiosAuditoria.Resumir(
                    new CampoAuditado("Código", codigo, asignatura.Codigo),
                    new CampoAuditado("Nombre", nombre, asignatura.Nombre),
                    new CampoAuditado("Curso", curso, asignatura.Curso),
                    new CampoAuditado("Docente", docenteAnterior, docenteNuevo));

                if (cambios.HayCambios)
                    AuditoriaRepositorio.Registrar(autor, AccionAuditoria.AsignaturaModificada,
                        "Asignatura " + asignatura.Codigo + ": se modificó " + cambios.TextoCampos + ".",
                        Entidad, asignatura.Id, cambios.ValorAnterior, cambios.ValorNuevo, conexion, transaccion);
            });
        }

        /// <summary>
        /// Nombre de usuario del docente (por ejemplo, cvaldes), para identificarlo en la bitácora.
        /// </summary>
        private static string NombreUsuarioDocente(int docenteId, SqlConnection conexion, SqlTransaction transaccion)
        {
            using (SqlCommand comando = new SqlCommand("SELECT NombreUsuario FROM Usuario WHERE Id = @Id", conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", docenteId);
                return comando.ExecuteScalar() as string ?? "Id " + docenteId;
            }
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