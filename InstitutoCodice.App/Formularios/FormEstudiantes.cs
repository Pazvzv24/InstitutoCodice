using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Gestión de estudiantes (RF-01, RF-02). Las desactivaciones quedan en la bitácora.
    /// Ley 21.719: exporta todos los datos de un titular (derechos de acceso y portabilidad)
    /// y permite bloquear el tratamiento de sus datos sin borrar su historial.
    /// F5 o "Recargar" vuelve a leer la lista desde la base.
    /// </summary>
    public partial class FormEstudiantes : Form
    {
        // Dependencias: llegan por el constructor (las entrega el Contenedor)
        private readonly EstudianteRepositorio repositorio;
        private readonly ServicioDatosTitular servicioDatosTitular;

        private List<Estudiante> estudiantes = new List<Estudiante>();
        private Estudiante? seleccionado;

        public FormEstudiantes(EstudianteRepositorio repositorio, ServicioDatosTitular servicioDatosTitular)
        {
            InitializeComponent();
            this.repositorio = repositorio;
            this.servicioDatosTitular = servicioDatosTitular;
            dgvEstudiantes.AutoGenerateColumns = false;
        }

        private void FormEstudiantes_Load(object sender, EventArgs e)
        {
            CargarEstudiantes();
            LimpiarFormulario();
        }

        private void CargarEstudiantes()
        {
            try
            {
                estudiantes = repositorio.Listar();
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void AplicarFiltro()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();
            var lista = filtro.Length == 0
                ? estudiantes
                : estudiantes.Where(x => (x.Rut + " " + x.NombreCompleto() + " " + x.Curso).ToLower().Contains(filtro)).ToList();

            dgvEstudiantes.DataSource = null;
            dgvEstudiantes.DataSource = lista;
            dgvEstudiantes.ClearSelection();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltro();
        }

        private void dgvEstudiantes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var estudiante = dgvEstudiantes.Rows[e.RowIndex].DataBoundItem as Estudiante;
            if (estudiante != null)
                MostrarEnFormulario(estudiante);
        }

        private void MostrarEnFormulario(Estudiante estudiante)
        {
            seleccionado = estudiante;
            txtRut.Text = estudiante.Rut;
            txtNombres.Text = estudiante.Nombres;
            txtApellidos.Text = estudiante.Apellidos;
            txtCorreo.Text = estudiante.Correo;
            txtCurso.Text = estudiante.Curso;
            chkActivo.Checked = estudiante.Activo;
            chkBloqueado.Checked = estudiante.TratamientoBloqueado;
            lblModo.Text = "Editando: " + estudiante.NombreCompleto()
                         + (estudiante.TratamientoBloqueado ? " (tratamiento bloqueado)" : "");
        }

        private void LimpiarFormulario()
        {
            seleccionado = null;
            txtRut.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            txtCorreo.Clear();
            txtCurso.Clear();
            chkActivo.Checked = true;
            chkBloqueado.Checked = false;
            lblModo.Text = "Modo: nuevo estudiante";
            dgvEstudiantes.ClearSelection();
            txtRut.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var estudiante = new Estudiante
            {
                Id = seleccionado != null ? seleccionado.Id : 0,
                Rut = ValidadorRut.Normalizar(txtRut.Text),
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Correo = txtCorreo.Text.Trim(),
                Curso = txtCurso.Text.Trim(),
                Activo = chkActivo.Checked,
                TratamientoBloqueado = chkBloqueado.Checked
            };

            string errores = Validar(estudiante);
            if (errores.Length > 0)
            {
                Mensajes.Advertencia("Revise los datos:\n" + errores);
                return;
            }

            // Ley 21.719: se pide confirmación antes de bloquear el tratamiento
            bool bloqueaAhora = estudiante.TratamientoBloqueado && (seleccionado == null || !seleccionado.TratamientoBloqueado);
            if (bloqueaAhora && !Mensajes.Confirmar("¿Bloquear el tratamiento de los datos de " + estudiante.NombreCompleto() + "?\n\n"
                    + "No aparecerá en inscripciones, asistencia, calificaciones ni informes hasta que se desbloquee.\n"
                    + "Su historial se conserva y sus datos se pueden seguir exportando."))
                return;

            try
            {
                if (estudiante.Id == 0)
                {
                    repositorio.Agregar(estudiante, Sesion.UsuarioActual); // se audita en la misma transacción
                    Mensajes.Info("Estudiante registrado correctamente.");
                }
                else
                {
                    // El repositorio audita, en la misma transacción, los datos modificados,
                    // la desactivación o reactivación y el bloqueo del tratamiento
                    repositorio.Actualizar(estudiante, Sesion.UsuarioActual);
                    Mensajes.Info("Datos del estudiante actualizados.");
                }

                CargarEstudiantes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private static string Validar(Estudiante estudiante)
        {
            var errores = new StringBuilder();

            if (estudiante.Rut.Length == 0)
                errores.AppendLine("- El RUT es obligatorio.");
            else if (!estudiante.ValidarRut())
                errores.AppendLine("- El RUT no es válido (ejemplo: 12345678-5).");

            if (estudiante.Nombres.Length == 0)
                errores.AppendLine("- Los nombres son obligatorios.");
            if (estudiante.Apellidos.Length == 0)
                errores.AppendLine("- Los apellidos son obligatorios.");
            if (estudiante.Curso.Length == 0)
                errores.AppendLine("- El curso es obligatorio.");
            if (estudiante.Correo.Length > 0 && (!estudiante.Correo.Contains("@") || !estudiante.Correo.Contains(".")))
                errores.AppendLine("- El correo no tiene un formato válido.");

            return errores.ToString();
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (seleccionado == null)
            {
                Mensajes.Advertencia("Seleccione un estudiante de la lista.");
                return;
            }

            if (!seleccionado.Activo)
            {
                Mensajes.Advertencia("El estudiante ya está inactivo.");
                return;
            }

            if (!Mensajes.Confirmar("¿Desactivar a " + seleccionado.NombreCompleto() + "?\nSu historial de notas y asistencia se conserva."))
                return;

            try
            {
                seleccionado.Desactivar();
                repositorio.Actualizar(seleccionado, Sesion.UsuarioActual); // audita la desactivación
                Mensajes.Info("Estudiante desactivado.");
                CargarEstudiantes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        // ------------------------------------------------------------ Ley 21.719

        /// <summary>
        /// Derechos de acceso y portabilidad: guarda en un archivo JSON todos los datos
        /// que el sistema tiene del estudiante seleccionado. Queda en la bitácora.
        /// </summary>
        private void btnExportarDatos_Click(object sender, EventArgs e)
        {
            // Defensa en profundidad: solo el administrador atiende las solicitudes de los titulares
            if (!Sesion.EsAdministrador)
            {
                Mensajes.Advertencia("Solo el administrador puede exportar los datos de un titular.");
                return;
            }

            if (seleccionado == null)
            {
                Mensajes.Advertencia("Seleccione un estudiante de la lista.");
                return;
            }

            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Guardar datos del titular";
                dialogo.Filter = "Archivo JSON (*.json)|*.json";
                dialogo.FileName = ServicioDatosTitular.NombreArchivo(seleccionado.Rut, DateTime.Now);
                dialogo.OverwritePrompt = true;

                if (dialogo.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    Cursor = Cursors.WaitCursor;
                    var datos = servicioDatosTitular.Generar(seleccionado, DateTime.UtcNow);
                    File.WriteAllText(dialogo.FileName, ServicioDatosTitular.ConvertirAJson(datos), new UTF8Encoding(false));
                    Cursor = Cursors.Default;

                    // Datos personales salen del sistema: queda registro de quién, de qué titular y cuándo
                    Bitacora.Registrar(AccionAuditoria.DatosTitularExportados,
                        "Datos del titular RUT " + seleccionado.Rut + " exportados (" + datos.Asignaturas.Count
                        + " asignaturas). Derechos de acceso y portabilidad.",
                        entidad: "Estudiante", entidadId: seleccionado.Id, valorNuevo: "JSON");

                    if (Mensajes.Confirmar("Datos exportados correctamente.\n\n"
                        + "Entregue el archivo solo al titular, por un medio seguro.\n\n"
                        + "¿Desea abrir la carpeta donde se guardó?"))
                        Process.Start("explorer.exe", "/select,\"" + dialogo.FileName + "\"");
                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    Mensajes.Error(ex);
                }
            }
        }

        // ------------------------------------------------------------ Recargar (F5)

        private void btnRecargar_Click(object sender, EventArgs e) => Recargar();

        private void FormEstudiantes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Recargar();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Vuelve a leer los datos desde la base (por ejemplo, si se modificaron en otra ventana).
        /// </summary>
        private void Recargar()
        {
            CargarEstudiantes(); // mantiene el filtro de búsqueda
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}