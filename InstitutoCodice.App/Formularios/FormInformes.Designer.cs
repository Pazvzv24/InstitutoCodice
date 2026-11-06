namespace InstitutoCodice.App.Formularios
{
    partial class FormInformes
    {
        /// <summary>
        /// Recursos del diseñador.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Libera los recursos del formulario.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Inicializa los controles del formulario.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpEstudiante = new System.Windows.Forms.GroupBox();
            this.cmbEstudiante = new System.Windows.Forms.ComboBox();
            this.btnVerEstudiante = new System.Windows.Forms.Button();
            this.grpCurso = new System.Windows.Forms.GroupBox();
            this.cmbAsignatura = new System.Windows.Forms.ComboBox();
            this.btnVerCurso = new System.Windows.Forms.Button();
            this.lblTituloInforme = new System.Windows.Forms.Label();
            this.dgvInforme = new System.Windows.Forms.DataGridView();
            this.colRut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstudiante = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAsignatura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromedio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAsistencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblResumen = new System.Windows.Forms.Label();
            this.lblCriterio = new System.Windows.Forms.Label();
            this.btnExportarExcel = new System.Windows.Forms.Button();
            this.btnExportarPdf = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInforme)).BeginInit();
            this.grpEstudiante.SuspendLayout();
            this.grpCurso.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpEstudiante
            // 
            this.grpEstudiante.Controls.Add(this.cmbEstudiante);
            this.grpEstudiante.Controls.Add(this.btnVerEstudiante);
            this.grpEstudiante.Location = new System.Drawing.Point(15, 15);
            this.grpEstudiante.Name = "grpEstudiante";
            this.grpEstudiante.Size = new System.Drawing.Size(420, 85);
            this.grpEstudiante.TabIndex = 0;
            this.grpEstudiante.Text = "Informe por estudiante";
            // 
            // cmbEstudiante
            // 
            this.cmbEstudiante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstudiante.Location = new System.Drawing.Point(15, 35);
            this.cmbEstudiante.Name = "cmbEstudiante";
            this.cmbEstudiante.Size = new System.Drawing.Size(270, 23);
            this.cmbEstudiante.TabIndex = 1;
            // 
            // btnVerEstudiante
            // 
            this.btnVerEstudiante.Location = new System.Drawing.Point(295, 32);
            this.btnVerEstudiante.Name = "btnVerEstudiante";
            this.btnVerEstudiante.Size = new System.Drawing.Size(110, 29);
            this.btnVerEstudiante.TabIndex = 2;
            this.btnVerEstudiante.Text = "Ver informe";
            this.btnVerEstudiante.UseVisualStyleBackColor = true;
            this.btnVerEstudiante.Click += new System.EventHandler(this.btnVerEstudiante_Click);
            // 
            // grpCurso
            // 
            this.grpCurso.Controls.Add(this.cmbAsignatura);
            this.grpCurso.Controls.Add(this.btnVerCurso);
            this.grpCurso.Location = new System.Drawing.Point(450, 15);
            this.grpCurso.Name = "grpCurso";
            this.grpCurso.Size = new System.Drawing.Size(435, 85);
            this.grpCurso.TabIndex = 3;
            this.grpCurso.Text = "Informe por curso (asignatura)";
            // 
            // cmbAsignatura
            // 
            this.cmbAsignatura.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAsignatura.Location = new System.Drawing.Point(15, 35);
            this.cmbAsignatura.Name = "cmbAsignatura";
            this.cmbAsignatura.Size = new System.Drawing.Size(285, 23);
            this.cmbAsignatura.TabIndex = 4;
            // 
            // btnVerCurso
            // 
            this.btnVerCurso.Location = new System.Drawing.Point(310, 32);
            this.btnVerCurso.Name = "btnVerCurso";
            this.btnVerCurso.Size = new System.Drawing.Size(110, 29);
            this.btnVerCurso.TabIndex = 5;
            this.btnVerCurso.Text = "Ver informe";
            this.btnVerCurso.UseVisualStyleBackColor = true;
            this.btnVerCurso.Click += new System.EventHandler(this.btnVerCurso_Click);
            // 
            // lblTituloInforme
            // 
            this.lblTituloInforme.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTituloInforme.Location = new System.Drawing.Point(15, 115);
            this.lblTituloInforme.Name = "lblTituloInforme";
            this.lblTituloInforme.Size = new System.Drawing.Size(870, 22);
            this.lblTituloInforme.TabIndex = 6;
            this.lblTituloInforme.Text = "Seleccione un estudiante o una asignatura.";
            // 
            // dgvInforme
            // 
            this.dgvInforme.AllowUserToAddRows = false;
            this.dgvInforme.AllowUserToDeleteRows = false;
            this.dgvInforme.AllowUserToResizeRows = false;
            this.dgvInforme.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvInforme.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInforme.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInforme.MultiSelect = false;
            this.dgvInforme.RowHeadersVisible = false;
            this.dgvInforme.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInforme.ReadOnly = true;
            this.dgvInforme.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colRut,
                        this.colEstudiante,
                        this.colAsignatura,
                        this.colNotas,
                        this.colPromedio,
                        this.colAsistencia,
                        this.colEstado});
            this.dgvInforme.Location = new System.Drawing.Point(15, 142);
            this.dgvInforme.Name = "dgvInforme";
            this.dgvInforme.Size = new System.Drawing.Size(870, 320);
            this.dgvInforme.TabIndex = 7;
            // 
            // colRut
            // 
            this.colRut.FillWeight = 75F;
            this.colRut.HeaderText = "RUT";
            this.colRut.Name = "colRut";
            this.colRut.ReadOnly = true;
            // 
            // colEstudiante
            // 
            this.colEstudiante.FillWeight = 150F;
            this.colEstudiante.HeaderText = "Estudiante";
            this.colEstudiante.Name = "colEstudiante";
            this.colEstudiante.ReadOnly = true;
            // 
            // colAsignatura
            // 
            this.colAsignatura.FillWeight = 150F;
            this.colAsignatura.HeaderText = "Asignatura";
            this.colAsignatura.Name = "colAsignatura";
            this.colAsignatura.ReadOnly = true;
            // 
            // colNotas
            // 
            this.colNotas.FillWeight = 220F;
            this.colNotas.HeaderText = "Notas";
            this.colNotas.Name = "colNotas";
            this.colNotas.ReadOnly = true;
            // 
            // colPromedio
            // 
            this.colPromedio.FillWeight = 60F;
            this.colPromedio.HeaderText = "Promedio";
            this.colPromedio.Name = "colPromedio";
            this.colPromedio.ReadOnly = true;
            // 
            // colAsistencia
            // 
            this.colAsistencia.FillWeight = 70F;
            this.colAsistencia.HeaderText = "% Asistencia";
            this.colAsistencia.Name = "colAsistencia";
            this.colAsistencia.ReadOnly = true;
            // 
            // colEstado
            // 
            this.colEstado.FillWeight = 70F;
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            // 
            // lblResumen
            // 
            this.lblResumen.ForeColor = System.Drawing.Color.DimGray;
            this.lblResumen.Location = new System.Drawing.Point(15, 472);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(870, 20);
            this.lblResumen.TabIndex = 8;
            // 
            // lblCriterio
            // 
            this.lblCriterio.ForeColor = System.Drawing.Color.DimGray;
            this.lblCriterio.Location = new System.Drawing.Point(15, 495);
            this.lblCriterio.Name = "lblCriterio";
            this.lblCriterio.Size = new System.Drawing.Size(420, 20);
            this.lblCriterio.TabIndex = 9;
            this.lblCriterio.Text = "Aprobado: promedio ≥ 4,0 y asistencia ≥ 75 %.";
            // 
            // btnExportarExcel
            // 
            this.btnExportarExcel.Location = new System.Drawing.Point(455, 512);
            this.btnExportarExcel.Name = "btnExportarExcel";
            this.btnExportarExcel.Size = new System.Drawing.Size(155, 32);
            this.btnExportarExcel.TabIndex = 10;
            this.btnExportarExcel.Text = "Exportar a Excel";
            this.btnExportarExcel.UseVisualStyleBackColor = true;
            this.btnExportarExcel.Click += new System.EventHandler(this.btnExportarExcel_Click);
            // 
            // btnExportarPdf
            // 
            this.btnExportarPdf.Location = new System.Drawing.Point(620, 512);
            this.btnExportarPdf.Name = "btnExportarPdf";
            this.btnExportarPdf.Size = new System.Drawing.Size(155, 32);
            this.btnExportarPdf.TabIndex = 11;
            this.btnExportarPdf.Text = "Exportar a PDF";
            this.btnExportarPdf.UseVisualStyleBackColor = true;
            this.btnExportarPdf.Click += new System.EventHandler(this.btnExportarPdf_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(785, 512);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 12;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormInformes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnExportarPdf);
            this.Controls.Add(this.btnExportarExcel);
            this.Controls.Add(this.lblCriterio);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.dgvInforme);
            this.Controls.Add(this.lblTituloInforme);
            this.Controls.Add(this.grpCurso);
            this.Controls.Add(this.grpEstudiante);
            this.Name = "FormInformes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Informes de notas y asistencia";
            this.Load += new System.EventHandler(this.FormInformes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInforme)).EndInit();
            this.grpEstudiante.ResumeLayout(false);
            this.grpEstudiante.PerformLayout();
            this.grpCurso.ResumeLayout(false);
            this.grpCurso.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpEstudiante;
        private System.Windows.Forms.ComboBox cmbEstudiante;
        private System.Windows.Forms.Button btnVerEstudiante;
        private System.Windows.Forms.GroupBox grpCurso;
        private System.Windows.Forms.ComboBox cmbAsignatura;
        private System.Windows.Forms.Button btnVerCurso;
        private System.Windows.Forms.Label lblTituloInforme;
        private System.Windows.Forms.DataGridView dgvInforme;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRut;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstudiante;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAsignatura;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromedio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAsistencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Label lblCriterio;
        private System.Windows.Forms.Button btnExportarExcel;
        private System.Windows.Forms.Button btnExportarPdf;
        private System.Windows.Forms.Button btnCerrar;
    }
}