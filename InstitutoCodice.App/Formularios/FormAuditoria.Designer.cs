namespace InstitutoCodice.App.Formularios
{
    partial class FormAuditoria
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
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblAccion = new System.Windows.Forms.Label();
            this.cmbAccion = new System.Windows.Forms.ComboBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvAuditoria = new System.Windows.Forms.DataGridView();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetalle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAnterior = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNuevo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDesde
            // 
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(15, 23);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.TabIndex = 0;
            this.lblDesde.Text = "Desde:";
            // 
            // dtpDesde
            // 
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(65, 20);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(120, 23);
            this.dtpDesde.TabIndex = 1;
            // 
            // lblHasta
            // 
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(200, 23);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.TabIndex = 2;
            this.lblHasta.Text = "Hasta:";
            // 
            // dtpHasta
            // 
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(250, 20);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(120, 23);
            this.dtpHasta.TabIndex = 3;
            // 
            // lblAccion
            // 
            this.lblAccion.AutoSize = true;
            this.lblAccion.Location = new System.Drawing.Point(385, 23);
            this.lblAccion.Name = "lblAccion";
            this.lblAccion.TabIndex = 4;
            this.lblAccion.Text = "Acción:";
            // 
            // cmbAccion
            // 
            this.cmbAccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccion.Location = new System.Drawing.Point(440, 20);
            this.cmbAccion.Name = "cmbAccion";
            this.cmbAccion.Size = new System.Drawing.Size(180, 23);
            this.cmbAccion.TabIndex = 5;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(635, 23);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.TabIndex = 6;
            this.lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(695, 20);
            this.txtUsuario.MaxLength = 50;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(140, 23);
            this.txtUsuario.TabIndex = 7;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(850, 16);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(115, 32);
            this.btnBuscar.TabIndex = 8;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dgvAuditoria
            // 
            this.dgvAuditoria.AllowUserToAddRows = false;
            this.dgvAuditoria.AllowUserToDeleteRows = false;
            this.dgvAuditoria.AllowUserToResizeRows = false;
            this.dgvAuditoria.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvAuditoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuditoria.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAuditoria.MultiSelect = false;
            this.dgvAuditoria.RowHeadersVisible = false;
            this.dgvAuditoria.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAuditoria.ReadOnly = true;
            this.dgvAuditoria.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colFecha,
                        this.colUsuario,
                        this.colAccion,
                        this.colDetalle,
                        this.colAnterior,
                        this.colNuevo});
            this.dgvAuditoria.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvAuditoria.Location = new System.Drawing.Point(15, 60);
            this.dgvAuditoria.Name = "dgvAuditoria";
            this.dgvAuditoria.Size = new System.Drawing.Size(950, 440);
            this.dgvAuditoria.TabIndex = 9;
            // 
            // colFecha
            // 
            this.colFecha.FillWeight = 85F;
            this.colFecha.HeaderText = "Fecha y hora";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            // 
            // colUsuario
            // 
            this.colUsuario.FillWeight = 60F;
            this.colUsuario.HeaderText = "Usuario";
            this.colUsuario.Name = "colUsuario";
            this.colUsuario.ReadOnly = true;
            // 
            // colAccion
            // 
            this.colAccion.FillWeight = 90F;
            this.colAccion.HeaderText = "Acción";
            this.colAccion.Name = "colAccion";
            this.colAccion.ReadOnly = true;
            // 
            // colDetalle
            // 
            this.colDetalle.FillWeight = 200F;
            this.colDetalle.HeaderText = "Detalle";
            this.colDetalle.Name = "colDetalle";
            this.colDetalle.ReadOnly = true;
            // 
            // colAnterior
            // 
            this.colAnterior.FillWeight = 45F;
            this.colAnterior.HeaderText = "Anterior";
            this.colAnterior.Name = "colAnterior";
            this.colAnterior.ReadOnly = true;
            // 
            // colNuevo
            // 
            this.colNuevo.FillWeight = 45F;
            this.colNuevo.HeaderText = "Nuevo";
            this.colNuevo.Name = "colNuevo";
            this.colNuevo.ReadOnly = true;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.ForeColor = System.Drawing.Color.DimGray;
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.lblTotal.Location = new System.Drawing.Point(15, 517);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.TabIndex = 10;
            this.lblTotal.Text = "Registros: 0";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.btnCerrar.Location = new System.Drawing.Point(865, 510);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 11;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormAuditoria
            // 
            this.AcceptButton = this.btnBuscar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 555);
            this.MinimumSize = new System.Drawing.Size(996, 594); // no se puede achicar más que el diseño original
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvAuditoria);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.cmbAccion);
            this.Controls.Add(this.lblAccion);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Name = "FormAuditoria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bitácora de auditoría";
            this.Load += new System.EventHandler(this.FormAuditoria_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblAccion;
        private System.Windows.Forms.ComboBox cmbAccion;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvAuditoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetalle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAnterior;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNuevo;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCerrar;
    }
}