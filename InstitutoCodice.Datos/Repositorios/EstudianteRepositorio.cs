using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de estudiantes, incluyendo registro y actualización.
    /// Cada alta, modificación, desactivación y bloqueo del tratamiento queda en la bitácora,
    /// dentro de la misma transacción: si algo falla, no se guarda nada.
    /// Minimización: la bitácora identifica al estudiante por RUT y, de sus nombres y correo,
    /// registra solo que cambiaron, no su valor.
    /// </summary>
    public class EstudianteRepositorio : RepositorioBase
    {
        private const string Entidad = "Estudiante";

        public List<Estudiante> Listar()
        {
            const string sql = @"
                SELECT Id, Rut, Nombres, Apellidos, Correo, Curso, Activo, TratamientoBloqueado
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

        public void Agregar(Estudiante estudiante, Usuario autor)
        {
            const string sql = @"
                INSERT INTO Estudiante (Rut, Nombres, Apellidos, Correo, Curso, Activo, TratamientoBloqueado)
                VALUES (@Rut, @Nombres, @Apellidos, @Correo, @Curso, @Activo, @TratamientoBloqueado);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    AgregarParametros(comando, estudiante);
                    estudiante.Id = (int)comando.ExecuteScalar();
                }

                AuditoriaRepositorio.Registrar(autor, AccionAuditoria.EstudianteCreado,
                    "Estudiante RUT " + estudiante.Rut + " registrado.",
                    Entidad, estudiante.Id, null, "Curso: " + estudiante.Curso, conexion, transaccion);

                if (estudiante.TratamientoBloqueado)
                    RegistrarTratamiento(autor, estudiante, false, conexion, transaccion);
            });
        }

        public void Actualizar(Estudiante estudiante, Usuario autor)
        {
            const string sqlAnterior = @"
                SELECT Rut, Nombres, Apellidos, Correo, Curso, Activo, TratamientoBloqueado
                FROM Estudiante
                WHERE Id = @Id";
            const string sql = @"
                UPDATE Estudiante
                SET Rut = @Rut, Nombres = @Nombres, Apellidos = @Apellidos,
                    Correo = @Correo, Curso = @Curso, Activo = @Activo,
                    TratamientoBloqueado = @TratamientoBloqueado
                WHERE Id = @Id";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                // 1. Datos actuales, leídos de la base (no de la pantalla)
                string rut, nombres, apellidos, correo, curso;
                bool activo, bloqueado;
                using (SqlCommand comando = new SqlCommand(sqlAnterior, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Id", estudiante.Id);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (!lector.Read())
                            throw new InvalidOperationException("El estudiante ya no existe. Recargue la lista.");

                        rut = lector.GetString(0);
                        nombres = lector.GetString(1);
                        apellidos = lector.GetString(2);
                        correo = lector.IsDBNull(3) ? string.Empty : lector.GetString(3);
                        curso = lector.GetString(4);
                        activo = lector.GetBoolean(5);
                        bloqueado = lector.GetBoolean(6);
                    }
                }

                // 2. Guardar
                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    AgregarParametros(comando, estudiante);
                    comando.Parameters.AddWithValue("@Id", estudiante.Id);
                    comando.ExecuteNonQuery();
                }

                // 3. Auditar cada tipo de cambio
                ResumenCambios cambios = CambiosAuditoria.Resumir(
                    new CampoAuditado("RUT", rut, estudiante.Rut),
                    new CampoAuditado("Nombres", nombres, estudiante.Nombres, esDatoPersonal: true),
                    new CampoAuditado("Apellidos", apellidos, estudiante.Apellidos, esDatoPersonal: true),
                    new CampoAuditado("Correo", correo, estudiante.Correo, esDatoPersonal: true),
                    new CampoAuditado("Curso", curso, estudiante.Curso));

                if (cambios.HayCambios)
                    AuditoriaRepositorio.Registrar(autor, AccionAuditoria.EstudianteModificado,
                        "Estudiante RUT " + estudiante.Rut + ": se modificó " + cambios.TextoCampos + ".",
                        Entidad, estudiante.Id, cambios.ValorAnterior, cambios.ValorNuevo, conexion, transaccion);

                if (activo != estudiante.Activo)
                    AuditoriaRepositorio.Registrar(autor,
                        estudiante.Activo ? AccionAuditoria.EstudianteReactivado : AccionAuditoria.EstudianteDesactivado,
                        "Estudiante RUT " + estudiante.Rut + (estudiante.Activo ? " reactivado." : " desactivado."),
                        Entidad, estudiante.Id,
                        activo ? "Activo" : "Inactivo", estudiante.Activo ? "Activo" : "Inactivo",
                        conexion, transaccion);

                if (bloqueado != estudiante.TratamientoBloqueado)
                    RegistrarTratamiento(autor, estudiante, bloqueado, conexion, transaccion);
            });
        }

        /// <summary>
        /// Ley 21.719: registra el bloqueo o desbloqueo del tratamiento de datos del estudiante.
        /// </summary>
        private static void RegistrarTratamiento(Usuario autor, Estudiante estudiante, bool bloqueadoAntes,
                                                 SqlConnection conexion, SqlTransaction transaccion)
        {
            bool bloqueado = estudiante.TratamientoBloqueado;
            AuditoriaRepositorio.Registrar(autor,
                bloqueado ? AccionAuditoria.TratamientoBloqueado : AccionAuditoria.TratamientoDesbloqueado,
                "Tratamiento de datos del estudiante RUT " + estudiante.Rut
                    + (bloqueado ? " bloqueado" : " desbloqueado") + " a solicitud del titular (Ley 21.719).",
                Entidad, estudiante.Id,
                bloqueadoAntes ? "Bloqueado" : "Habilitado", bloqueado ? "Bloqueado" : "Habilitado",
                conexion, transaccion);
        }

        private static void AgregarParametros(SqlCommand comando, Estudiante estudiante)
        {
            comando.Parameters.AddWithValue("@Rut", estudiante.Rut);
            comando.Parameters.AddWithValue("@Nombres", estudiante.Nombres);
            comando.Parameters.AddWithValue("@Apellidos", estudiante.Apellidos);
            comando.Parameters.AddWithValue("@Correo", ValorONulo(estudiante.Correo));
            comando.Parameters.AddWithValue("@Curso", estudiante.Curso);
            comando.Parameters.AddWithValue("@Activo", estudiante.Activo);
            comando.Parameters.AddWithValue("@TratamientoBloqueado", estudiante.TratamientoBloqueado);
        }
    }
}