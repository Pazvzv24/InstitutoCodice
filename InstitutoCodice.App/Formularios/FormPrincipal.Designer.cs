namespace InstitutoCodice.App.Formularios
{
    partial class FormPrincipal
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
            this.lblBienvenida = new System.Windows.Forms.Label();
            this.lblRol = new System.Windows.Forms.Label();
            this.grpGestion = new System.Windows.Forms.GroupBox();
            this.btnEstudiantes = new System.Windows.Forms.Button();
            this.btnDocentes = new System.Windows.Forms.Button();
            this.btnAsignaturas = new System.Windows.Forms.Button();
            this.btnInscripciones = new System.Windows.Forms.Button();
            this.grpDocencia = new System.Windows.Forms.GroupBox();
            this.btnAsistencia = new System.Windows.Forms.Button();
            this.btnCalificaciones = new System.Windows.Forms.Button();
            this.btnInformes = new System.Windows.Forms.Button();
            this.btnCambiarContrasena = new System.Windows.Forms.Button(); // NUEVO
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.grpGestion.SuspendLayout();
            this.grpDocencia.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBienvenida
            // 
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblBienvenida.Location = new System.Drawing.Point(20, 18);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.TabIndex = 0;
            this.lblBienvenida.Text = "Bienvenido(a)";
            // 
            // lblRol
            // 
            this.lblRol.AutoSize = true;
            this.lblRol.ForeColor = System.Drawing.Color.DimGray;
            this.lblRol.Location = new System.Drawing.Point(22, 48);
            this.lblRol.Name = "lblRol";
            this.lblRol.TabIndex = 1;
            this.lblRol.Text = "Rol:";
            // 
            // grpGestion
            // 
            this.grpGestion.Controls.Add(this.btnEstudiantes);
            this.grpGestion.Controls.Add(this.btnDocentes);
            this.grpGestion.Controls.Add(this.btnAsignaturas);
            this.grpGestion.Controls.Add(this.btnInscripciones);
            this.grpGestion.Location = new System.Drawing.Point(20, 85);
            this.grpGestion.Name = "grpGestion";
            this.grpGestion.Size = new System.Drawing.Size(230, 230);
            this.grpGestion.TabIndex = 2;
            this.grpGestion.Text = "Gestión (Administrador)";
            // 
            // btnEstudiantes
            // 
            this.btnEstudiantes.Location = new System.Drawing.Point(15, 30);
            this.btnEstudiantes.Name = "btnEstudiantes";
            this.btnEstudiantes.Size = new System.Drawing.Size(200, 38);
            this.btnEstudiantes.TabIndex = 3;
            this.btnEstudiantes.Text = "Estudiantes";
            this.btnEstudiantes.UseVisualStyleBackColor = true;
            this.btnEstudiantes.Click += new System.EventHandler(this.btnEstudiantes_Click);
            // 
            // btnDocentes
            // 
            this.btnDocentes.Location = new System.Drawing.Point(15, 78);
            this.btnDocentes.Name = "btnDocentes";
            this.btnDocentes.Size = new System.Drawing.Size(200, 38);
            this.btnDocentes.TabIndex = 4;
            this.btnDocentes.Text = "Docentes";
            this.btnDocentes.UseVisualStyleBackColor = true;
            this.btnDocentes.Click += new System.EventHandler(this.btnDocentes_Click);
            // 
            // btnAsignaturas
            // 
            this.btnAsignaturas.Location = new System.Drawing.Point(15, 126);
            this.btnAsignaturas.Name = "btnAsignaturas";
            this.btnAsignaturas.Size = new System.Drawing.Size(200, 38);
            this.btnAsignaturas.TabIndex = 5;
            this.btnAsignaturas.Text = "Asignaturas";
            this.btnAsignaturas.UseVisualStyleBackColor = true;
            this.btnAsignaturas.Click += new System.EventHandler(this.btnAsignaturas_Click);
            // 
            // btnInscripciones
            // 
            this.btnInscripciones.Location = new System.Drawing.Point(15, 174);
            this.btnInscripciones.Name = "btnInscripciones";
            this.btnInscripciones.Size = new System.Drawing.Size(200, 38);
            this.btnInscripciones.TabIndex = 6;
            this.btnInscripciones.Text = "Inscripciones";
            this.btnInscripciones.UseVisualStyleBackColor = true;
            this.btnInscripciones.Click += new System.EventHandler(this.btnInscripciones_Click);
            // 
            // grpDocencia
            // 
            this.grpDocencia.Controls.Add(this.btnAsistencia);
            this.grpDocencia.Controls.Add(this.btnCalificaciones);
            this.grpDocencia.Controls.Add(this.btnInformes);
            this.grpDocencia.Location = new System.Drawing.Point(270, 85);
            this.grpDocencia.Name = "grpDocencia";
            this.grpDocencia.Size = new System.Drawing.Size(230, 230);
            this.grpDocencia.TabIndex = 7;
            this.grpDocencia.Text = "Docencia";
            // 
            // btnAsistencia
            // 
            this.btnAsistencia.Location = new System.Drawing.Point(15, 30);
            this.btnAsistencia.Name = "btnAsistencia";
            this.btnAsistencia.Size = new System.Drawing.Size(200, 38);
            this.btnAsistencia.TabIndex = 8;
            this.btnAsistencia.Text = "Registrar asistencia";
            this.btnAsistencia.UseVisualStyleBackColor = true;
            this.btnAsistencia.Click += new System.EventHandler(this.btnAsistencia_Click);
            // 
            // btnCalificaciones
            // 
            this.btnCalificaciones.Location = new System.Drawing.Point(15, 78);
            this.btnCalificaciones.Name = "btnCalificaciones";
            this.btnCalificaciones.Size = new System.Drawing.Size(200, 38);
            this.btnCalificaciones.TabIndex = 9;
            this.btnCalificaciones.Text = "Evaluaciones y notas";
            this.btnCalificaciones.UseVisualStyleBackColor = true;
            this.btnCalificaciones.Click += new System.EventHandler(this.btnCalificaciones_Click);
            // 
            // btnInformes
            // 
            this.btnInformes.Location = new System.Drawing.Point(15, 126);
            this.btnInformes.Name = "btnInformes";
            this.btnInformes.Size = new System.Drawing.Size(200, 38);
            this.btnInformes.TabIndex = 10;
            this.btnInformes.Text = "Informes";
            this.btnInformes.UseVisualStyleBackColor = true;
            this.btnInformes.Click += new System.EventHandler(this.btnInformes_Click);
            // 
            // btnCambiarContrasena (NUEVO: fuera de los grupos, visible para todos los roles)
            // 
            this.btnCambiarContrasena.Location = new System.Drawing.Point(20, 338);
            this.btnCambiarContrasena.Name = "btnCambiarContrasena";
            this.btnCambiarContrasena.Size = new System.Drawing.Size(150, 34);
            this.btnCambiarContrasena.TabIndex = 11;
            this.btnCambiarContrasena.Text = "Cambiar contraseña";
            this.btnCambiarContrasena.UseVisualStyleBackColor = true;
            this.btnCambiarContrasena.Click += new System.EventHandler(this.btnCambiarContrasena_Click);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.Location = new System.Drawing.Point(380, 338);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(120, 34);
            this.btnCerrarSesion.TabIndex = 12;
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.UseVisualStyleBackColor = true;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 390);
            this.Controls.Add(this.btnCerrarSesion);
            this.Controls.Add(this.btnCambiarContrasena); // NUEVO
            this.Controls.Add(this.grpDocencia);
            this.Controls.Add(this.grpGestion);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.lblBienvenida);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Instituto Códice - Menú principal";
            this.Load += new System.EventHandler(this.FormPrincipal_Load);
            this.grpGestion.ResumeLayout(false);
            this.grpDocencia.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblBienvenida;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.GroupBox grpGestion;
        private System.Windows.Forms.Button btnEstudiantes;
        private System.Windows.Forms.Button btnDocentes;
        private System.Windows.Forms.Button btnAsignaturas;
        private System.Windows.Forms.Button btnInscripciones;
        private System.Windows.Forms.GroupBox grpDocencia;
        private System.Windows.Forms.Button btnAsistencia;
        private System.Windows.Forms.Button btnCalificaciones;
        private System.Windows.Forms.Button btnInformes;
        private System.Windows.Forms.Button btnCambiarContrasena; // NUEVO
        private System.Windows.Forms.Button btnCerrarSesion;
    }
}