namespace InstitutoCodice.App.Formularios
{
    partial class FormCalificaciones
    {
        /// <summary>
        /// Contenedor de componentes del formulario.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Libera los recursos que utiliza el formulario.
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
            this.lblAsignatura = new System.Windows.Forms.Label();
            this.cmbAsignatura = new System.Windows.Forms.ComboBox();
            this.grpEvaluaciones = new System.Windows.Forms.GroupBox();
            this.dgvEvaluaciones = new System.Windows.Forms.DataGridView();
            this.colEvalId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEvalNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEvalFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEvalPonderacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSuma = new System.Windows.Forms.Label();
            this.lblNuevaEval = new System.Windows.Forms.Label();
            this.lblNombreEval = new System.Windows.Forms.Label();
            this.txtNombreEval = new System.Windows.Forms.TextBox();
            this.lblFechaEval = new System.Windows.Forms.Label();
            this.dtpFechaEval = new System.Windows.Forms.DateTimePicker();
            this.lblPonderacion = new System.Windows.Forms.Label();
            this.nudPonderacion = new System.Windows.Forms.NumericUpDown();
            this.btnAgregarEval = new System.Windows.Forms.Button();
            this.grpNotas = new System.Windows.Forms.GroupBox();
            this.lblEvaluacionSel = new System.Windows.Forms.Label();
            this.dgvNotas = new System.Windows.Forms.DataGridView();
            this.colEstId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotaRut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotaNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAyudaNotas = new System.Windows.Forms.Label();
            this.btnGuardarNotas = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvaluaciones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPonderacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotas)).BeginInit();
            this.grpEvaluaciones.SuspendLayout();
            this.grpNotas.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAsignatura
            // 
            this.lblAsignatura.AutoSize = true;
            this.lblAsignatura.Location = new System.Drawing.Point(15, 20);
            this.lblAsignatura.Name = "lblAsignatura";
            this.lblAsignatura.TabIndex = 0;
            this.lblAsignatura.Text = "Asignatura:";
            // 
            // cmbAsignatura
            // 
            this.cmbAsignatura.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAsignatura.Location = new System.Drawing.Point(100, 17);
            this.cmbAsignatura.Name = "cmbAsignatura";
            this.cmbAsignatura.Size = new System.Drawing.Size(340, 23);
            this.cmbAsignatura.TabIndex = 1;
            this.cmbAsignatura.SelectedIndexChanged += new System.EventHandler(this.cmbAsignatura_SelectedIndexChanged);
            // 
            // grpEvaluaciones
            // 
            this.grpEvaluaciones.Controls.Add(this.dgvEvaluaciones);
            this.grpEvaluaciones.Controls.Add(this.lblSuma);
            this.grpEvaluaciones.Controls.Add(this.lblNuevaEval);
            this.grpEvaluaciones.Controls.Add(this.lblNombreEval);
            this.grpEvaluaciones.Controls.Add(this.txtNombreEval);
            this.grpEvaluaciones.Controls.Add(this.lblFechaEval);
            this.grpEvaluaciones.Controls.Add(this.dtpFechaEval);
            this.grpEvaluaciones.Controls.Add(this.lblPonderacion);
            this.grpEvaluaciones.Controls.Add(this.nudPonderacion);
            this.grpEvaluaciones.Controls.Add(this.btnAgregarEval);
            this.grpEvaluaciones.Location = new System.Drawing.Point(15, 55);
            this.grpEvaluaciones.Name = "grpEvaluaciones";
            this.grpEvaluaciones.Size = new System.Drawing.Size(430, 440);
            this.grpEvaluaciones.TabIndex = 2;
            this.grpEvaluaciones.Text = "Evaluaciones de la asignatura";
            // 
            // dgvEvaluaciones
            // 
            this.dgvEvaluaciones.AllowUserToAddRows = false;
            this.dgvEvaluaciones.AllowUserToDeleteRows = false;
            this.dgvEvaluaciones.AllowUserToResizeRows = false;
            this.dgvEvaluaciones.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvEvaluaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEvaluaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEvaluaciones.MultiSelect = false;
            this.dgvEvaluaciones.RowHeadersVisible = false;
            this.dgvEvaluaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEvaluaciones.ReadOnly = true;
            this.dgvEvaluaciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colEvalId,
                        this.colEvalNombre,
                        this.colEvalFecha,
                        this.colEvalPonderacion});
            this.dgvEvaluaciones.Location = new System.Drawing.Point(15, 25);
            this.dgvEvaluaciones.Name = "dgvEvaluaciones";
            this.dgvEvaluaciones.Size = new System.Drawing.Size(400, 215);
            this.dgvEvaluaciones.TabIndex = 3;
            this.dgvEvaluaciones.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEvaluaciones_CellClick);
            // 
            // colEvalId
            // 
            this.colEvalId.FillWeight = 100F;
            this.colEvalId.HeaderText = "Id";
            this.colEvalId.Name = "colEvalId";
            this.colEvalId.ReadOnly = true;
            this.colEvalId.Visible = false;
            // 
            // colEvalNombre
            // 
            this.colEvalNombre.FillWeight = 160F;
            this.colEvalNombre.HeaderText = "Evaluación";
            this.colEvalNombre.Name = "colEvalNombre";
            this.colEvalNombre.ReadOnly = true;
            // 
            // colEvalFecha
            // 
            this.colEvalFecha.FillWeight = 80F;
            this.colEvalFecha.HeaderText = "Fecha";
            this.colEvalFecha.Name = "colEvalFecha";
            this.colEvalFecha.ReadOnly = true;
            // 
            // colEvalPonderacion
            // 
            this.colEvalPonderacion.FillWeight = 80F;
            this.colEvalPonderacion.HeaderText = "Ponderación %";
            this.colEvalPonderacion.Name = "colEvalPonderacion";
            this.colEvalPonderacion.ReadOnly = true;
            // 
            // lblSuma
            // 
            this.lblSuma.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSuma.Location = new System.Drawing.Point(15, 250);
            this.lblSuma.Name = "lblSuma";
            this.lblSuma.Size = new System.Drawing.Size(400, 20);
            this.lblSuma.TabIndex = 4;
            this.lblSuma.Text = "Suma de ponderaciones: 0 %";
            // 
            // lblNuevaEval
            // 
            this.lblNuevaEval.AutoSize = true;
            this.lblNuevaEval.ForeColor = System.Drawing.Color.DimGray;
            this.lblNuevaEval.Location = new System.Drawing.Point(15, 282);
            this.lblNuevaEval.Name = "lblNuevaEval";
            this.lblNuevaEval.TabIndex = 5;
            this.lblNuevaEval.Text = "Nueva evaluación:";
            // 
            // lblNombreEval
            // 
            this.lblNombreEval.AutoSize = true;
            this.lblNombreEval.Location = new System.Drawing.Point(15, 310);
            this.lblNombreEval.Name = "lblNombreEval";
            this.lblNombreEval.TabIndex = 6;
            this.lblNombreEval.Text = "Nombre:";
            // 
            // txtNombreEval
            // 
            this.txtNombreEval.Location = new System.Drawing.Point(120, 307);
            this.txtNombreEval.Name = "txtNombreEval";
            this.txtNombreEval.Size = new System.Drawing.Size(295, 23);
            this.txtNombreEval.TabIndex = 7;
            // 
            // lblFechaEval
            // 
            this.lblFechaEval.AutoSize = true;
            this.lblFechaEval.Location = new System.Drawing.Point(15, 345);
            this.lblFechaEval.Name = "lblFechaEval";
            this.lblFechaEval.TabIndex = 8;
            this.lblFechaEval.Text = "Fecha:";
            // 
            // dtpFechaEval
            // 
            this.dtpFechaEval.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaEval.Location = new System.Drawing.Point(120, 342);
            this.dtpFechaEval.Name = "dtpFechaEval";
            this.dtpFechaEval.Size = new System.Drawing.Size(140, 23);
            this.dtpFechaEval.TabIndex = 9;
            // 
            // lblPonderacion
            // 
            this.lblPonderacion.AutoSize = true;
            this.lblPonderacion.Location = new System.Drawing.Point(15, 380);
            this.lblPonderacion.Name = "lblPonderacion";
            this.lblPonderacion.TabIndex = 10;
            this.lblPonderacion.Text = "Ponderación %:";
            // 
            // nudPonderacion
            // 
            this.nudPonderacion.DecimalPlaces = 0;
            this.nudPonderacion.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudPonderacion.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudPonderacion.Value = new decimal(new int[] { 25, 0, 0, 0 });
            this.nudPonderacion.Location = new System.Drawing.Point(120, 377);
            this.nudPonderacion.Name = "nudPonderacion";
            this.nudPonderacion.Size = new System.Drawing.Size(80, 23);
            this.nudPonderacion.TabIndex = 11;
            // 
            // btnAgregarEval
            // 
            this.btnAgregarEval.Location = new System.Drawing.Point(225, 373);
            this.btnAgregarEval.Name = "btnAgregarEval";
            this.btnAgregarEval.Size = new System.Drawing.Size(190, 30);
            this.btnAgregarEval.TabIndex = 12;
            this.btnAgregarEval.Text = "Agregar evaluación";
            this.btnAgregarEval.UseVisualStyleBackColor = true;
            this.btnAgregarEval.Click += new System.EventHandler(this.btnAgregarEval_Click);
            // 
            // grpNotas
            // 
            this.grpNotas.Controls.Add(this.lblEvaluacionSel);
            this.grpNotas.Controls.Add(this.dgvNotas);
            this.grpNotas.Controls.Add(this.lblAyudaNotas);
            this.grpNotas.Controls.Add(this.btnGuardarNotas);
            this.grpNotas.Location = new System.Drawing.Point(460, 55);
            this.grpNotas.Name = "grpNotas";
            this.grpNotas.Size = new System.Drawing.Size(525, 440);
            this.grpNotas.TabIndex = 13;
            this.grpNotas.Text = "Notas de la evaluación seleccionada";
            // 
            // lblEvaluacionSel
            // 
            this.lblEvaluacionSel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEvaluacionSel.Location = new System.Drawing.Point(15, 25);
            this.lblEvaluacionSel.Name = "lblEvaluacionSel";
            this.lblEvaluacionSel.Size = new System.Drawing.Size(495, 20);
            this.lblEvaluacionSel.TabIndex = 14;
            this.lblEvaluacionSel.Text = "Seleccione una evaluación de la lista.";
            // 
            // dgvNotas
            // 
            this.dgvNotas.AllowUserToAddRows = false;
            this.dgvNotas.AllowUserToDeleteRows = false;
            this.dgvNotas.AllowUserToResizeRows = false;
            this.dgvNotas.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvNotas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNotas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNotas.MultiSelect = false;
            this.dgvNotas.RowHeadersVisible = false;
            this.dgvNotas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNotas.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvNotas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colEstId,
                        this.colNotaRut,
                        this.colNotaNombre,
                        this.colNota});
            this.dgvNotas.Location = new System.Drawing.Point(15, 50);
            this.dgvNotas.Name = "dgvNotas";
            this.dgvNotas.Size = new System.Drawing.Size(495, 320);
            this.dgvNotas.TabIndex = 15;
            // 
            // colEstId
            // 
            this.colEstId.FillWeight = 100F;
            this.colEstId.HeaderText = "Id";
            this.colEstId.Name = "colEstId";
            this.colEstId.ReadOnly = true;
            this.colEstId.Visible = false;
            // 
            // colNotaRut
            // 
            this.colNotaRut.FillWeight = 80F;
            this.colNotaRut.HeaderText = "RUT";
            this.colNotaRut.Name = "colNotaRut";
            this.colNotaRut.ReadOnly = true;
            // 
            // colNotaNombre
            // 
            this.colNotaNombre.FillWeight = 190F;
            this.colNotaNombre.HeaderText = "Estudiante";
            this.colNotaNombre.Name = "colNotaNombre";
            this.colNotaNombre.ReadOnly = true;
            // 
            // colNota
            // 
            this.colNota.FillWeight = 60F;
            this.colNota.HeaderText = "Nota";
            this.colNota.Name = "colNota";
            this.colNota.ReadOnly = false;
            // 
            // lblAyudaNotas
            // 
            this.lblAyudaNotas.ForeColor = System.Drawing.Color.DimGray;
            this.lblAyudaNotas.Location = new System.Drawing.Point(15, 385);
            this.lblAyudaNotas.Name = "lblAyudaNotas";
            this.lblAyudaNotas.Size = new System.Drawing.Size(330, 40);
            this.lblAyudaNotas.TabIndex = 16;
            this.lblAyudaNotas.Text = "Notas entre 1,0 y 7,0. Deja la celda vacía si el estudiante aún no tiene nota.";
            // 
            // btnGuardarNotas
            // 
            this.btnGuardarNotas.Location = new System.Drawing.Point(360, 385);
            this.btnGuardarNotas.Name = "btnGuardarNotas";
            this.btnGuardarNotas.Size = new System.Drawing.Size(150, 32);
            this.btnGuardarNotas.TabIndex = 17;
            this.btnGuardarNotas.Text = "Guardar notas";
            this.btnGuardarNotas.UseVisualStyleBackColor = true;
            this.btnGuardarNotas.Click += new System.EventHandler(this.btnGuardarNotas_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(885, 510);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 18;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormCalificaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 560);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.grpNotas);
            this.Controls.Add(this.grpEvaluaciones);
            this.Controls.Add(this.cmbAsignatura);
            this.Controls.Add(this.lblAsignatura);
            this.Name = "FormCalificaciones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Evaluaciones y calificaciones";
            this.Load += new System.EventHandler(this.FormCalificaciones_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvaluaciones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPonderacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotas)).EndInit();
            this.grpEvaluaciones.ResumeLayout(false);
            this.grpEvaluaciones.PerformLayout();
            this.grpNotas.ResumeLayout(false);
            this.grpNotas.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblAsignatura;
        private System.Windows.Forms.ComboBox cmbAsignatura;
        private System.Windows.Forms.GroupBox grpEvaluaciones;
        private System.Windows.Forms.DataGridView dgvEvaluaciones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEvalId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEvalNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEvalFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEvalPonderacion;
        private System.Windows.Forms.Label lblSuma;
        private System.Windows.Forms.Label lblNuevaEval;
        private System.Windows.Forms.Label lblNombreEval;
        private System.Windows.Forms.TextBox txtNombreEval;
        private System.Windows.Forms.Label lblFechaEval;
        private System.Windows.Forms.DateTimePicker dtpFechaEval;
        private System.Windows.Forms.Label lblPonderacion;
        private System.Windows.Forms.NumericUpDown nudPonderacion;
        private System.Windows.Forms.Button btnAgregarEval;
        private System.Windows.Forms.GroupBox grpNotas;
        private System.Windows.Forms.Label lblEvaluacionSel;
        private System.Windows.Forms.DataGridView dgvNotas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotaRut;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotaNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNota;
        private System.Windows.Forms.Label lblAyudaNotas;
        private System.Windows.Forms.Button btnGuardarNotas;
        private System.Windows.Forms.Button btnCerrar;
    }
}