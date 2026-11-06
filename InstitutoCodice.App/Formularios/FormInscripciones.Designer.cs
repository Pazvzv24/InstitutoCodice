namespace InstitutoCodice.App.Formularios
{
    partial class FormInscripciones
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
            this.lblEstudiante = new System.Windows.Forms.Label();
            this.cmbEstudiante = new System.Windows.Forms.ComboBox();
            this.btnInscribir = new System.Windows.Forms.Button();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.lblInscritos = new System.Windows.Forms.Label();
            this.dgvInscritos = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstudiante = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInscritos)).BeginInit();
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
            this.cmbAsignatura.Location = new System.Drawing.Point(110, 17);
            this.cmbAsignatura.Name = "cmbAsignatura";
            this.cmbAsignatura.Size = new System.Drawing.Size(300, 23);
            this.cmbAsignatura.TabIndex = 1;
            this.cmbAsignatura.SelectedIndexChanged += new System.EventHandler(this.cmbAsignatura_SelectedIndexChanged);
            // 
            // lblEstudiante
            // 
            this.lblEstudiante.AutoSize = true;
            this.lblEstudiante.Location = new System.Drawing.Point(15, 55);
            this.lblEstudiante.Name = "lblEstudiante";
            this.lblEstudiante.TabIndex = 2;
            this.lblEstudiante.Text = "Estudiante:";
            // 
            // cmbEstudiante
            // 
            this.cmbEstudiante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstudiante.Location = new System.Drawing.Point(110, 52);
            this.cmbEstudiante.Name = "cmbEstudiante";
            this.cmbEstudiante.Size = new System.Drawing.Size(300, 23);
            this.cmbEstudiante.TabIndex = 3;
            // 
            // btnInscribir
            // 
            this.btnInscribir.Location = new System.Drawing.Point(425, 49);
            this.btnInscribir.Name = "btnInscribir";
            this.btnInscribir.Size = new System.Drawing.Size(140, 29);
            this.btnInscribir.TabIndex = 4;
            this.btnInscribir.Text = "Inscribir";
            this.btnInscribir.UseVisualStyleBackColor = true;
            this.btnInscribir.Click += new System.EventHandler(this.btnInscribir_Click);
            // 
            // btnQuitar
            // 
            this.btnQuitar.Location = new System.Drawing.Point(575, 49);
            this.btnQuitar.Name = "btnQuitar";
            this.btnQuitar.Size = new System.Drawing.Size(165, 29);
            this.btnQuitar.TabIndex = 5;
            this.btnQuitar.Text = "Quitar inscripción";
            this.btnQuitar.UseVisualStyleBackColor = true;
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            // 
            // lblInscritos
            // 
            this.lblInscritos.AutoSize = true;
            this.lblInscritos.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblInscritos.Location = new System.Drawing.Point(15, 95);
            this.lblInscritos.Name = "lblInscritos";
            this.lblInscritos.TabIndex = 6;
            this.lblInscritos.Text = "Estudiantes inscritos en la asignatura:";
            // 
            // dgvInscritos
            // 
            this.dgvInscritos.AllowUserToAddRows = false;
            this.dgvInscritos.AllowUserToDeleteRows = false;
            this.dgvInscritos.AllowUserToResizeRows = false;
            this.dgvInscritos.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvInscritos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInscritos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInscritos.MultiSelect = false;
            this.dgvInscritos.RowHeadersVisible = false;
            this.dgvInscritos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInscritos.ReadOnly = true;
            this.dgvInscritos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colId,
                        this.colRut,
                        this.colEstudiante,
                        this.colCurso,
                        this.colFecha});
            this.dgvInscritos.Location = new System.Drawing.Point(15, 120);
            this.dgvInscritos.Name = "dgvInscritos";
            this.dgvInscritos.Size = new System.Drawing.Size(725, 318);
            this.dgvInscritos.TabIndex = 7;
            // 
            // colId
            // 
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colId.Visible = false;
            // 
            // colRut
            // 
            this.colRut.FillWeight = 80F;
            this.colRut.HeaderText = "RUT";
            this.colRut.Name = "colRut";
            this.colRut.ReadOnly = true;
            // 
            // colEstudiante
            // 
            this.colEstudiante.FillWeight = 200F;
            this.colEstudiante.HeaderText = "Estudiante";
            this.colEstudiante.Name = "colEstudiante";
            this.colEstudiante.ReadOnly = true;
            // 
            // colCurso
            // 
            this.colCurso.FillWeight = 70F;
            this.colCurso.HeaderText = "Curso";
            this.colCurso.Name = "colCurso";
            this.colCurso.ReadOnly = true;
            // 
            // colFecha
            // 
            this.colFecha.FillWeight = 90F;
            this.colFecha.HeaderText = "Fecha de inscripción";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(15, 452);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.TabIndex = 8;
            this.lblTotal.Text = "Total inscritos: 0";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(640, 450);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 9;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormInscripciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 500);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvInscritos);
            this.Controls.Add(this.lblInscritos);
            this.Controls.Add(this.btnQuitar);
            this.Controls.Add(this.btnInscribir);
            this.Controls.Add(this.cmbEstudiante);
            this.Controls.Add(this.lblEstudiante);
            this.Controls.Add(this.cmbAsignatura);
            this.Controls.Add(this.lblAsignatura);
            this.Name = "FormInscripciones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inscripción de estudiantes en asignaturas";
            this.Load += new System.EventHandler(this.FormInscripciones_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInscritos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblAsignatura;
        private System.Windows.Forms.ComboBox cmbAsignatura;
        private System.Windows.Forms.Label lblEstudiante;
        private System.Windows.Forms.ComboBox cmbEstudiante;
        private System.Windows.Forms.Button btnInscribir;
        private System.Windows.Forms.Button btnQuitar;
        private System.Windows.Forms.Label lblInscritos;
        private System.Windows.Forms.DataGridView dgvInscritos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRut;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstudiante;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurso;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCerrar;
    }
}