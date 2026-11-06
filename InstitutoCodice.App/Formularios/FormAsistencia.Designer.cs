namespace InstitutoCodice.App.Formularios
{
    partial class FormAsistencia
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
            this.lblAsignatura = new System.Windows.Forms.Label();
            this.cmbAsignatura = new System.Windows.Forms.ComboBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.btnCargar = new System.Windows.Forms.Button();
            this.lblEstadoCarga = new System.Windows.Forms.Label();
            this.btnTodosPresentes = new System.Windows.Forms.Button();
            this.dgvAsistencia = new System.Windows.Forms.DataGridView();
            this.colEstudianteId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsistencia)).BeginInit();
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
            this.cmbAsignatura.Size = new System.Drawing.Size(310, 23);
            this.cmbAsignatura.TabIndex = 1;
            // 
            // lblFecha
            // 
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(425, 20);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.TabIndex = 2;
            this.lblFecha.Text = "Fecha:";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(475, 17);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(130, 23);
            this.dtpFecha.TabIndex = 3;
            // 
            // btnCargar
            // 
            this.btnCargar.Location = new System.Drawing.Point(620, 14);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(120, 29);
            this.btnCargar.TabIndex = 4;
            this.btnCargar.Text = "Cargar lista";
            this.btnCargar.UseVisualStyleBackColor = true;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // lblEstadoCarga
            // 
            this.lblEstadoCarga.ForeColor = System.Drawing.Color.DimGray;
            this.lblEstadoCarga.Location = new System.Drawing.Point(15, 58);
            this.lblEstadoCarga.Name = "lblEstadoCarga";
            this.lblEstadoCarga.Size = new System.Drawing.Size(510, 20);
            this.lblEstadoCarga.TabIndex = 5;
            this.lblEstadoCarga.Text = "Seleccione la asignatura y la fecha, y presione \"Cargar lista\".";
            // 
            // btnTodosPresentes
            // 
            this.btnTodosPresentes.Location = new System.Drawing.Point(540, 52);
            this.btnTodosPresentes.Name = "btnTodosPresentes";
            this.btnTodosPresentes.Size = new System.Drawing.Size(200, 29);
            this.btnTodosPresentes.TabIndex = 6;
            this.btnTodosPresentes.Text = "Marcar todos presentes";
            this.btnTodosPresentes.UseVisualStyleBackColor = true;
            this.btnTodosPresentes.Click += new System.EventHandler(this.btnTodosPresentes_Click);
            // 
            // dgvAsistencia
            // 
            this.dgvAsistencia.AllowUserToAddRows = false;
            this.dgvAsistencia.AllowUserToDeleteRows = false;
            this.dgvAsistencia.AllowUserToResizeRows = false;
            this.dgvAsistencia.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvAsistencia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAsistencia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAsistencia.MultiSelect = false;
            this.dgvAsistencia.RowHeadersVisible = false;
            this.dgvAsistencia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAsistencia.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvAsistencia.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colEstudianteId,
                        this.colRut,
                        this.colNombre,
                        this.colEstado});
            this.dgvAsistencia.Location = new System.Drawing.Point(15, 90);
            this.dgvAsistencia.Name = "dgvAsistencia";
            this.dgvAsistencia.Size = new System.Drawing.Size(725, 340);
            this.dgvAsistencia.TabIndex = 7;
            // 
            // colEstudianteId
            // 
            this.colEstudianteId.FillWeight = 100F;
            this.colEstudianteId.HeaderText = "Id";
            this.colEstudianteId.Name = "colEstudianteId";
            this.colEstudianteId.ReadOnly = true;
            this.colEstudianteId.Visible = false;
            // 
            // colRut
            // 
            this.colRut.FillWeight = 80F;
            this.colRut.HeaderText = "RUT";
            this.colRut.Name = "colRut";
            this.colRut.ReadOnly = true;
            // 
            // colNombre
            // 
            this.colNombre.FillWeight = 200F;
            this.colNombre.HeaderText = "Estudiante";
            this.colNombre.Name = "colNombre";
            this.colNombre.ReadOnly = true;
            // 
            // colEstado
            // 
            this.colEstado.FillWeight = 90F;
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Items.AddRange(new object[] { "Presente", "Ausente", "Justificado" });
            this.colEstado.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.colEstado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = false;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(480, 445);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(150, 36);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "Guardar asistencia";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(640, 445);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 36);
            this.btnCerrar.TabIndex = 9;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormAsistencia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 520);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvAsistencia);
            this.Controls.Add(this.btnTodosPresentes);
            this.Controls.Add(this.lblEstadoCarga);
            this.Controls.Add(this.btnCargar);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.cmbAsignatura);
            this.Controls.Add(this.lblAsignatura);
            this.Name = "FormAsistencia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registro de asistencia";
            this.Load += new System.EventHandler(this.FormAsistencia_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsistencia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblAsignatura;
        private System.Windows.Forms.ComboBox cmbAsignatura;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.Label lblEstadoCarga;
        private System.Windows.Forms.Button btnTodosPresentes;
        private System.Windows.Forms.DataGridView dgvAsistencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstudianteId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRut;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombre;
        private System.Windows.Forms.DataGridViewComboBoxColumn colEstado;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCerrar;
    }
}