namespace InstitutoCodice.App.Formularios
{
    partial class FormEstudiantes
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
            this.lblRut = new System.Windows.Forms.Label();
            this.txtRut = new System.Windows.Forms.TextBox();
            this.lblNombres = new System.Windows.Forms.Label();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.lblApellidos = new System.Windows.Forms.Label();
            this.txtApellidos = new System.Windows.Forms.TextBox();
            this.lblCorreo = new System.Windows.Forms.Label();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.lblCurso = new System.Windows.Forms.Label();
            this.txtCurso = new System.Windows.Forms.TextBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.chkBloqueado = new System.Windows.Forms.CheckBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnDesactivar = new System.Windows.Forms.Button();
            this.btnExportarDatos = new System.Windows.Forms.Button();
            this.lblModo = new System.Windows.Forms.Label();
            this.lblAyudaRut = new System.Windows.Forms.Label();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.dgvEstudiantes = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNombres = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colApellidos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCorreo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colBloqueado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnRecargar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstudiantes)).BeginInit();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDatos
            // 
            this.grpDatos.Controls.Add(this.lblRut);
            this.grpDatos.Controls.Add(this.txtRut);
            this.grpDatos.Controls.Add(this.lblNombres);
            this.grpDatos.Controls.Add(this.txtNombres);
            this.grpDatos.Controls.Add(this.lblApellidos);
            this.grpDatos.Controls.Add(this.txtApellidos);
            this.grpDatos.Controls.Add(this.lblCorreo);
            this.grpDatos.Controls.Add(this.txtCorreo);
            this.grpDatos.Controls.Add(this.lblCurso);
            this.grpDatos.Controls.Add(this.txtCurso);
            this.grpDatos.Controls.Add(this.chkActivo);
            this.grpDatos.Controls.Add(this.chkBloqueado);
            this.grpDatos.Controls.Add(this.btnNuevo);
            this.grpDatos.Controls.Add(this.btnGuardar);
            this.grpDatos.Controls.Add(this.btnDesactivar);
            this.grpDatos.Controls.Add(this.btnExportarDatos);
            this.grpDatos.Controls.Add(this.lblModo);
            this.grpDatos.Controls.Add(this.lblAyudaRut);
            this.grpDatos.Location = new System.Drawing.Point(15, 15);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(300, 490);
            this.grpDatos.TabIndex = 0;
            this.grpDatos.Text = "Datos del estudiante";
            // 
            // lblRut
            // 
            this.lblRut.AutoSize = true;
            this.lblRut.Location = new System.Drawing.Point(15, 35);
            this.lblRut.Name = "lblRut";
            this.lblRut.TabIndex = 1;
            this.lblRut.Text = "RUT:";
            // 
            // txtRut
            // 
            this.txtRut.Location = new System.Drawing.Point(110, 32);
            this.txtRut.Name = "txtRut";
            this.txtRut.Size = new System.Drawing.Size(170, 23);
            this.txtRut.TabIndex = 2;
            // 
            // lblNombres
            // 
            this.lblNombres.AutoSize = true;
            this.lblNombres.Location = new System.Drawing.Point(15, 70);
            this.lblNombres.Name = "lblNombres";
            this.lblNombres.TabIndex = 3;
            this.lblNombres.Text = "Nombres:";
            // 
            // txtNombres
            // 
            this.txtNombres.Location = new System.Drawing.Point(110, 67);
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(170, 23);
            this.txtNombres.TabIndex = 4;
            // 
            // lblApellidos
            // 
            this.lblApellidos.AutoSize = true;
            this.lblApellidos.Location = new System.Drawing.Point(15, 105);
            this.lblApellidos.Name = "lblApellidos";
            this.lblApellidos.TabIndex = 5;
            this.lblApellidos.Text = "Apellidos:";
            // 
            // txtApellidos
            // 
            this.txtApellidos.Location = new System.Drawing.Point(110, 102);
            this.txtApellidos.Name = "txtApellidos";
            this.txtApellidos.Size = new System.Drawing.Size(170, 23);
            this.txtApellidos.TabIndex = 6;
            // 
            // lblCorreo
            // 
            this.lblCorreo.AutoSize = true;
            this.lblCorreo.Location = new System.Drawing.Point(15, 140);
            this.lblCorreo.Name = "lblCorreo";
            this.lblCorreo.TabIndex = 7;
            this.lblCorreo.Text = "Correo:";
            // 
            // txtCorreo
            // 
            this.txtCorreo.Location = new System.Drawing.Point(110, 137);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(170, 23);
            this.txtCorreo.TabIndex = 8;
            // 
            // lblCurso
            // 
            this.lblCurso.AutoSize = true;
            this.lblCurso.Location = new System.Drawing.Point(15, 175);
            this.lblCurso.Name = "lblCurso";
            this.lblCurso.TabIndex = 9;
            this.lblCurso.Text = "Curso:";
            // 
            // txtCurso
            // 
            this.txtCurso.Location = new System.Drawing.Point(110, 172);
            this.txtCurso.Name = "txtCurso";
            this.txtCurso.Size = new System.Drawing.Size(170, 23);
            this.txtCurso.TabIndex = 10;
            // 
            // chkActivo
            // 
            this.chkActivo.AutoSize = true;
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.Location = new System.Drawing.Point(110, 207);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.TabIndex = 11;
            this.chkActivo.Text = "Activo";
            // 
            // chkBloqueado
            // 
            this.chkBloqueado.AutoSize = true;
            this.chkBloqueado.Location = new System.Drawing.Point(110, 232);
            this.chkBloqueado.Name = "chkBloqueado";
            this.chkBloqueado.TabIndex = 12;
            this.chkBloqueado.Text = "Tratamiento bloqueado";
            // 
            // btnNuevo
            // 
            this.btnNuevo.Location = new System.Drawing.Point(15, 275);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(125, 32);
            this.btnNuevo.TabIndex = 13;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(155, 275);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(125, 32);
            this.btnGuardar.TabIndex = 14;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnDesactivar
            // 
            this.btnDesactivar.Location = new System.Drawing.Point(15, 317);
            this.btnDesactivar.Name = "btnDesactivar";
            this.btnDesactivar.Size = new System.Drawing.Size(265, 32);
            this.btnDesactivar.TabIndex = 15;
            this.btnDesactivar.Text = "Desactivar estudiante";
            this.btnDesactivar.UseVisualStyleBackColor = true;
            this.btnDesactivar.Click += new System.EventHandler(this.btnDesactivar_Click);
            // 
            // btnExportarDatos
            // 
            this.btnExportarDatos.Location = new System.Drawing.Point(15, 359);
            this.btnExportarDatos.Name = "btnExportarDatos";
            this.btnExportarDatos.Size = new System.Drawing.Size(265, 32);
            this.btnExportarDatos.TabIndex = 16;
            this.btnExportarDatos.Text = "Exportar datos del titular (JSON)";
            this.btnExportarDatos.UseVisualStyleBackColor = true;
            this.btnExportarDatos.Click += new System.EventHandler(this.btnExportarDatos_Click);
            // 
            // lblModo
            // 
            this.lblModo.ForeColor = System.Drawing.Color.DimGray;
            this.lblModo.Location = new System.Drawing.Point(15, 403);
            this.lblModo.Name = "lblModo";
            this.lblModo.Size = new System.Drawing.Size(265, 35);
            this.lblModo.TabIndex = 17;
            this.lblModo.Text = "Modo: nuevo estudiante";
            // 
            // lblAyudaRut
            // 
            this.lblAyudaRut.ForeColor = System.Drawing.Color.DimGray;
            this.lblAyudaRut.Location = new System.Drawing.Point(15, 443);
            this.lblAyudaRut.Name = "lblAyudaRut";
            this.lblAyudaRut.Size = new System.Drawing.Size(265, 35);
            this.lblAyudaRut.TabIndex = 18;
            this.lblAyudaRut.Text = "RUT sin puntos y con guion, por ejemplo 12345678-5.";
            // 
            // lblBuscar
            // 
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Location = new System.Drawing.Point(330, 20);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.TabIndex = 19;
            this.lblBuscar.Text = "Buscar:";
            // 
            // txtBuscar
            // 
            this.txtBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscar.Location = new System.Drawing.Point(390, 17);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(300, 23);
            this.txtBuscar.TabIndex = 20;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // dgvEstudiantes
            // 
            this.dgvEstudiantes.AllowUserToAddRows = false;
            this.dgvEstudiantes.AllowUserToDeleteRows = false;
            this.dgvEstudiantes.AllowUserToResizeRows = false;
            this.dgvEstudiantes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvEstudiantes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEstudiantes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEstudiantes.MultiSelect = false;
            this.dgvEstudiantes.RowHeadersVisible = false;
            this.dgvEstudiantes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEstudiantes.ReadOnly = true;
            this.dgvEstudiantes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colId,
                        this.colRut,
                        this.colNombres,
                        this.colApellidos,
                        this.colCorreo,
                        this.colCurso,
                        this.colActivo,
                        this.colBloqueado});
            this.dgvEstudiantes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEstudiantes.Location = new System.Drawing.Point(330, 50);
            this.dgvEstudiantes.Name = "dgvEstudiantes";
            this.dgvEstudiantes.Size = new System.Drawing.Size(555, 395);
            this.dgvEstudiantes.TabIndex = 21;
            this.dgvEstudiantes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEstudiantes_CellClick);
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
            // colRut
            // 
            this.colRut.DataPropertyName = "Rut";
            this.colRut.FillWeight = 90F;
            this.colRut.HeaderText = "RUT";
            this.colRut.Name = "colRut";
            this.colRut.ReadOnly = true;
            // 
            // colNombres
            // 
            this.colNombres.DataPropertyName = "Nombres";
            this.colNombres.FillWeight = 100F;
            this.colNombres.HeaderText = "Nombres";
            this.colNombres.Name = "colNombres";
            this.colNombres.ReadOnly = true;
            // 
            // colApellidos
            // 
            this.colApellidos.DataPropertyName = "Apellidos";
            this.colApellidos.FillWeight = 110F;
            this.colApellidos.HeaderText = "Apellidos";
            this.colApellidos.Name = "colApellidos";
            this.colApellidos.ReadOnly = true;
            // 
            // colCorreo
            // 
            this.colCorreo.DataPropertyName = "Correo";
            this.colCorreo.FillWeight = 100F;
            this.colCorreo.HeaderText = "Correo";
            this.colCorreo.Name = "colCorreo";
            this.colCorreo.ReadOnly = true;
            // 
            // colCurso
            // 
            this.colCurso.DataPropertyName = "Curso";
            this.colCurso.FillWeight = 70F;
            this.colCurso.HeaderText = "Curso";
            this.colCurso.Name = "colCurso";
            this.colCurso.ReadOnly = true;
            // 
            // colActivo
            // 
            this.colActivo.DataPropertyName = "Activo";
            this.colActivo.FillWeight = 50F;
            this.colActivo.HeaderText = "Activo";
            this.colActivo.Name = "colActivo";
            this.colActivo.ReadOnly = true;
            // 
            // colBloqueado
            // 
            this.colBloqueado.DataPropertyName = "TratamientoBloqueado";
            this.colBloqueado.FillWeight = 85F;
            this.colBloqueado.HeaderText = "Bloqueado";
            this.colBloqueado.Name = "colBloqueado";
            this.colBloqueado.ReadOnly = true;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Location = new System.Drawing.Point(785, 490);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 22;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnRecargar
            // 
            this.btnRecargar.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            this.btnRecargar.Location = new System.Drawing.Point(785, 14);
            this.btnRecargar.Name = "btnRecargar";
            this.btnRecargar.Size = new System.Drawing.Size(100, 29);
            this.btnRecargar.TabIndex = 50;
            this.btnRecargar.Text = "Recargar";
            this.btnRecargar.UseVisualStyleBackColor = true;
            this.btnRecargar.Click += new System.EventHandler(this.btnRecargar_Click);
            // 
            // FormEstudiantes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 535);
            this.MinimumSize = new System.Drawing.Size(916, 574); // no se puede achicar más que el diseño original
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnRecargar);
            this.Controls.Add(this.dgvEstudiantes);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.lblBuscar);
            this.Controls.Add(this.grpDatos);
            this.KeyPreview = true; // F5 = Recargar
            this.Name = "FormEstudiantes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de estudiantes";
            this.Load += new System.EventHandler(this.FormEstudiantes_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormEstudiantes_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstudiantes)).EndInit();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.Label lblRut;
        private System.Windows.Forms.TextBox txtRut;
        private System.Windows.Forms.Label lblNombres;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.Label lblApellidos;
        private System.Windows.Forms.TextBox txtApellidos;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Label lblCurso;
        private System.Windows.Forms.TextBox txtCurso;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.CheckBox chkBloqueado;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnDesactivar;
        private System.Windows.Forms.Button btnExportarDatos;
        private System.Windows.Forms.Label lblModo;
        private System.Windows.Forms.Label lblAyudaRut;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.DataGridView dgvEstudiantes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRut;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombres;
        private System.Windows.Forms.DataGridViewTextBoxColumn colApellidos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCorreo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurso;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colActivo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colBloqueado;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnRecargar;
    }
}