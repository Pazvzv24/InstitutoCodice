namespace InstitutoCodice.App.Formularios
{
    partial class FormAsignaturas
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
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblCurso = new System.Windows.Forms.Label();
            this.txtCurso = new System.Windows.Forms.TextBox();
            this.lblDocente = new System.Windows.Forms.Label();
            this.cmbDocente = new System.Windows.Forms.ComboBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.lblModo = new System.Windows.Forms.Label();
            this.dgvAsignaturas = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDocente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnRecargar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignaturas)).BeginInit();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDatos
            // 
            this.grpDatos.Controls.Add(this.lblCodigo);
            this.grpDatos.Controls.Add(this.txtCodigo);
            this.grpDatos.Controls.Add(this.lblNombre);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.lblCurso);
            this.grpDatos.Controls.Add(this.txtCurso);
            this.grpDatos.Controls.Add(this.lblDocente);
            this.grpDatos.Controls.Add(this.cmbDocente);
            this.grpDatos.Controls.Add(this.btnNuevo);
            this.grpDatos.Controls.Add(this.btnGuardar);
            this.grpDatos.Controls.Add(this.lblModo);
            this.grpDatos.Location = new System.Drawing.Point(15, 15);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(300, 300);
            this.grpDatos.TabIndex = 0;
            this.grpDatos.Text = "Datos de la asignatura";
            // 
            // lblCodigo
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Location = new System.Drawing.Point(15, 35);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.TabIndex = 1;
            this.lblCodigo.Text = "Código:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(110, 32);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(170, 23);
            this.txtCodigo.TabIndex = 2;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(15, 70);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.TabIndex = 3;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(110, 67);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(170, 23);
            this.txtNombre.TabIndex = 4;
            // 
            // lblCurso
            // 
            this.lblCurso.AutoSize = true;
            this.lblCurso.Location = new System.Drawing.Point(15, 105);
            this.lblCurso.Name = "lblCurso";
            this.lblCurso.TabIndex = 5;
            this.lblCurso.Text = "Curso:";
            // 
            // txtCurso
            // 
            this.txtCurso.Location = new System.Drawing.Point(110, 102);
            this.txtCurso.Name = "txtCurso";
            this.txtCurso.Size = new System.Drawing.Size(170, 23);
            this.txtCurso.TabIndex = 6;
            // 
            // lblDocente
            // 
            this.lblDocente.AutoSize = true;
            this.lblDocente.Location = new System.Drawing.Point(15, 140);
            this.lblDocente.Name = "lblDocente";
            this.lblDocente.TabIndex = 7;
            this.lblDocente.Text = "Docente:";
            // 
            // cmbDocente
            // 
            this.cmbDocente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDocente.Location = new System.Drawing.Point(110, 137);
            this.cmbDocente.Name = "cmbDocente";
            this.cmbDocente.Size = new System.Drawing.Size(170, 23);
            this.cmbDocente.TabIndex = 8;
            // 
            // btnNuevo
            // 
            this.btnNuevo.Location = new System.Drawing.Point(15, 185);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(125, 32);
            this.btnNuevo.TabIndex = 9;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(155, 185);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(125, 32);
            this.btnGuardar.TabIndex = 10;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // lblModo
            // 
            this.lblModo.ForeColor = System.Drawing.Color.DimGray;
            this.lblModo.Location = new System.Drawing.Point(15, 235);
            this.lblModo.Name = "lblModo";
            this.lblModo.Size = new System.Drawing.Size(265, 40);
            this.lblModo.TabIndex = 11;
            this.lblModo.Text = "Modo: nueva asignatura";
            // 
            // dgvAsignaturas
            // 
            this.dgvAsignaturas.AllowUserToAddRows = false;
            this.dgvAsignaturas.AllowUserToDeleteRows = false;
            this.dgvAsignaturas.AllowUserToResizeRows = false;
            this.dgvAsignaturas.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvAsignaturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAsignaturas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAsignaturas.MultiSelect = false;
            this.dgvAsignaturas.RowHeadersVisible = false;
            this.dgvAsignaturas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAsignaturas.ReadOnly = true;
            this.dgvAsignaturas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colId,
                        this.colCodigo,
                        this.colNombre,
                        this.colCurso,
                        this.colDocente});
            this.dgvAsignaturas.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvAsignaturas.Location = new System.Drawing.Point(330, 15);
            this.dgvAsignaturas.Name = "dgvAsignaturas";
            this.dgvAsignaturas.Size = new System.Drawing.Size(555, 400);
            this.dgvAsignaturas.TabIndex = 12;
            this.dgvAsignaturas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAsignaturas_CellClick);
            // 
            // colId
            // 
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 100F;
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colId.Visible = false;
            // 
            // colCodigo
            // 
            this.colCodigo.DataPropertyName = "Codigo";
            this.colCodigo.FillWeight = 70F;
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.Name = "colCodigo";
            this.colCodigo.ReadOnly = true;
            // 
            // colNombre
            // 
            this.colNombre.DataPropertyName = "Nombre";
            this.colNombre.FillWeight = 170F;
            this.colNombre.HeaderText = "Nombre";
            this.colNombre.Name = "colNombre";
            this.colNombre.ReadOnly = true;
            // 
            // colCurso
            // 
            this.colCurso.DataPropertyName = "Curso";
            this.colCurso.FillWeight = 70F;
            this.colCurso.HeaderText = "Curso";
            this.colCurso.Name = "colCurso";
            this.colCurso.ReadOnly = true;
            // 
            // colDocente
            // 
            this.colDocente.DataPropertyName = "NombreDocente";
            this.colDocente.FillWeight = 130F;
            this.colDocente.HeaderText = "Docente";
            this.colDocente.Name = "colDocente";
            this.colDocente.ReadOnly = true;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.btnCerrar.Location = new System.Drawing.Point(785, 432);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 13;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnRecargar
            // 
            this.btnRecargar.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.btnRecargar.Location = new System.Drawing.Point(675, 432);
            this.btnRecargar.Name = "btnRecargar";
            this.btnRecargar.Size = new System.Drawing.Size(100, 32);
            this.btnRecargar.TabIndex = 50;
            this.btnRecargar.Text = "Recargar";
            this.btnRecargar.UseVisualStyleBackColor = true;
            this.btnRecargar.Click += new System.EventHandler(this.btnRecargar_Click);
            // 
            // FormAsignaturas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 480);
            this.MinimumSize = new System.Drawing.Size(916, 519); // no se puede achicar más que el diseño original
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnRecargar);
            this.Controls.Add(this.dgvAsignaturas);
            this.Controls.Add(this.grpDatos);
            this.KeyPreview = true; // F5 = Recargar
            this.Name = "FormAsignaturas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de asignaturas";
            this.Load += new System.EventHandler(this.FormAsignaturas_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAsignaturas_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignaturas)).EndInit();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblCurso;
        private System.Windows.Forms.TextBox txtCurso;
        private System.Windows.Forms.Label lblDocente;
        private System.Windows.Forms.ComboBox cmbDocente;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label lblModo;
        private System.Windows.Forms.DataGridView dgvAsignaturas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurso;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDocente;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnRecargar;
    }
}