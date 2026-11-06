using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Consulta de la bitácora de auditoría (solo administrador).
    /// Es de solo lectura: los registros no se pueden editar ni borrar.
    /// </summary>
    public partial class FormAuditoria : Form
    {
        private const string TodasLasAcciones = "(Todas)";
        private readonly AuditoriaRepositorio repositorio = new AuditoriaRepositorio();

        public FormAuditoria()
        {
            InitializeComponent();
        }

        private void FormAuditoria_Load(object sender, EventArgs e)
        {
            // Defensa en profundidad: aunque el botón esté desactivado para docentes,
            // el formulario vuelve a comprobar el rol.
            if (!Sesion.EsAdministrador)
            {
                Mensajes.Advertencia("Solo el administrador puede consultar la bitácora.");
                Close();
                return;
            }

            var acciones = new List<string> { TodasLasAcciones };
            acciones.AddRange(AccionAuditoria.Todas);
            cmbAccion.DataSource = acciones;

            dtpDesde.Value = DateTime.Today.AddDays(-7);
            dtpHasta.Value = DateTime.Today;

            Buscar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                Mensajes.Advertencia("La fecha \"Desde\" no puede ser posterior a \"Hasta\".");
                return;
            }

            // Las fechas se guardan en UTC; el filtro se elige en hora local
            DateTime desdeUtc = dtpDesde.Value.Date.ToUniversalTime();
            DateTime hastaUtc = dtpHasta.Value.Date.AddDays(1).ToUniversalTime();
            string? accion = cmbAccion.SelectedItem as string;
            if (accion == TodasLasAcciones)
                accion = null;

            try
            {
                var registros = repositorio.Listar(desdeUtc, hastaUtc, accion, txtUsuario.Text);

                dgvAuditoria.Rows.Clear();
                foreach (var r in registros)
                {
                    dgvAuditoria.Rows.Add(
                        r.FechaUtc.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss"),
                        r.NombreUsuario,
                        r.Accion,
                        r.Detalle,
                        r.ValorAnterior,
                        r.ValorNuevo);
                }

                dgvAuditoria.ClearSelection();
                lblTotal.Text = "Registros: " + registros.Count + (registros.Count == 500 ? " (se muestran los 500 más recientes)" : "");
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}