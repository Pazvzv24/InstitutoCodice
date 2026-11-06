# Instituto Códice - Sistema de Asistencia y Notas

Aplicación de escritorio para gestión académica: estudiantes, asignaturas, inscripciones, asistencia, calificaciones e informes, con control de acceso por rol.
Tarea 3 - PRO205 Taller de Programación.

**Stack:** C# / .NET 8 / Windows Forms / SQL Server / MSTest

---

### Requisitos

- Visual Studio 2022 (v17.8+)
- Workload: Desarrollo de escritorio de .NET
- SQL Server Express o LocalDB (SSMS opcional)

### Instalación rápida

1. Abrir `InstitutoCodice.sln` y presionar **F5** (el proyecto de inicio es `InstitutoCodice.App`).
2. En la primera ejecución, la app detecta que la base no existe y ofrece crearla con los datos de demostración.
3. Ingresar con `admin` / `Admin123`.

**Elegir el servidor SQL**

El servidor se configura en `InstitutoCodice.App/appsettings.json` (en la versión publicada, junto al `.exe`), sin recompilar:

```json
{
  "BaseDatos": {
    "Servidor": "localhost\\SQLEXPRESS",
    "Nombre": "InstitutoCodiceDB",
    "CrearSiNoExiste": true
  }
}
```

- SQL Server Express: `localhost\\SQLEXPRESS`
- LocalDB (incluido con Visual Studio): `(localdb)\\MSSQLLocalDB`

La conexión usa autenticación de Windows: no se guardan usuarios ni contraseñas de SQL.

**Cuenta de la aplicación con mínimo privilegio (opcional, recomendado)**

Por defecto la app usa autenticación de Windows, necesaria para instalar la base. Para el uso diario puede conectarse con una cuenta limitada:

1. Activar en SQL Server el modo de autenticación "Windows y SQL Server" y reiniciar el servicio.
2. Como administrador, crear el login (la contraseña nunca se guarda en el repositorio):
   ```sql
   CREATE LOGIN app_codice WITH PASSWORD = N'...', DEFAULT_DATABASE = InstitutoCodiceDB, CHECK_POLICY = ON;
   ```
3. Ejecutar `BaseDatos/07_Rol_App_Codice.sql` (crea el rol `rol_app_codice` y asigna el usuario).
4. Crear en Windows la variable de entorno de usuario `INSTITUTOCODICE_DB_CLAVE` con la contraseña.
5. Crear `InstitutoCodice.App/appsettings.local.json` (está en `.gitignore`):
   ```json
   { "BaseDatos": { "Autenticacion": "Sql" } }
   ```
6. Opcional: ejecutar `BaseDatos/08_PruebaPermisos.sql` para comprobar los bloqueos.

**Instalación manual (opcional)**

También se puede crear la base desde SSMS ejecutando en orden:

- `BaseDatos/01_InstitutoCodiceDB.sql` -> Crea la BD y las 9 tablas.
- `BaseDatos/02_DatosEjemplo.sql` -> Carga datos de prueba.

**Actualización (solo instalaciones anteriores a la v1.1)**

- `BaseDatos/03_ActualizarHashesPBKDF2.sql` -> Migra las contraseñas de las cuentas demo de SHA-256 a PBKDF2 y agrega la restricción de formato del hash.

**Actualización (solo instalaciones anteriores a la v1.3)**

- `BaseDatos/04_BloqueoDeCuenta.sql` -> Agrega las columnas de bloqueo de cuenta. Incluye el comando para desbloquear una cuenta manualmente.

**Actualización (solo instalaciones anteriores a la v1.4)**

- `BaseDatos/05_CambioDeContrasena.sql` -> Agrega el cambio de contraseña obligatorio.

**Actualización (solo instalaciones anteriores a la v1.5)**

- `BaseDatos/06_Auditoria.sql` -> Crea la bitácora de auditoría.

**Pruebas**
- Menú **Prueba -> Ejecutar todas las pruebas** (25 pruebas unitarias, no requieren base de datos).

### Cuentas de prueba

> Credenciales ficticias de demostración, solo para entorno local. Las cuentas demo no exigen cambio de contraseña al ingresar.

| Usuario | Contraseña | Rol | Asignaturas |
| :--- | :--- | :--- | :--- |
| **admin** | Admin123 | Administrador | Todas |
| **pmunoz** | Docente123 | Docente | POO101, TPR205 |
| **cvaldes** | Docente123 | Docente | BDD201 |

La contraseña distingue mayúsculas. Al 3er intento fallido la app se cierra y la cuenta queda bloqueada 15 minutos.

### Seguridad

| Control | Implementación |
| :--- | :--- |
| Hash de contraseñas | PBKDF2-SHA256, 600.000 iteraciones, sal aleatoria de 16 bytes (recomendación OWASP) |
| Formato versionado | `PBKDF2$iteraciones$sal$hash`: permite subir iteraciones o cambiar de algoritmo sin invalidar hashes |
| Comparación segura | `CryptographicOperations.FixedTimeEquals` (tiempo constante, evita ataques de temporización) |
| Defensa en la BD | Restricción `CK_Usuario_HashFormato`: la base rechaza contraseñas en texto plano |
| Límite de intentos | Al 3er intento fallido la aplicación se cierra; mensaje genérico "usuario o contraseña incorrectos" |
| Bloqueo de cuenta | 3 intentos fallidos bloquean la cuenta 15 minutos, guardado en la BD (persiste al reiniciar la app); durante el bloqueo no se verifica la contraseña |
| Control de acceso | Un docente solo ve estudiantes, asignaturas e informes de los cursos que dicta; filtrado en la interfaz y en el servicio (defensa en profundidad) |
| Política de contraseñas | Mínimo 8 caracteres con mayúscula, minúscula y número; no puede contener el usuario ni repetir la actual |
| Cambio obligatorio | Una contraseña asignada o restablecida por el administrador debe cambiarse en el siguiente ingreso |
| Bitácora de auditoría | Registra ingresos, fallos, bloqueos, cambios de contraseña, desactivaciones y cada nota con su valor anterior y nuevo |
| Mínimo privilegio | La app se conecta con `app_codice` (rol `rol_app_codice`): solo lee, inserta y actualiza; borra solo inscripciones; no puede modificar la bitácora, crear tablas ni desactivar triggers |
| Secretos fuera del código | La contraseña de la cuenta de la app se lee desde una variable de entorno; la configuración local está en `.gitignore` |
| Bitácora inmutable | Un trigger en la BD rechaza UPDATE y DELETE sobre la tabla Auditoria; las notas se auditan en la misma transacción |
| Inyección SQL | Todas las consultas son parametrizadas |
| Configuración | Conexión en `appsettings.json` con autenticación de Windows, sin credenciales en el código |
| Instalación segura | La creación automática solo actúa si la base no existe y solo ejecuta los scripts incluidos con la app |
| Integridad de datos | Reglas de negocio validadas en el código y en la BD (CHECK, UNIQUE, FOREIGN KEY) |

### Arquitectura (3 capas)

- **Negocio** (`InstitutoCodice.Negocio`): Modelos UML, lógica de informes, seguridad y validación de RUT.
- **Datos** (`InstitutoCodice.Datos`): Conexión a SQL y repositorios.
- **Presentación** (`InstitutoCodice.App`): Formularios Windows Forms.
- **Pruebas** (`InstitutoCodice.Pruebas`): Pruebas unitarias MSTest con patrón AAA y fuente de datos falsa.
- **BaseDatos**: Scripts SQL de instalación y actualización.

### Trazabilidad

| Requerimiento | Código |
| :--- | :--- |
| RF-01, RF-02 (Estudiantes) | `FormEstudiantes`, `ValidarRut()`, `Desactivar()` |
| RF-03 (Asignaturas) | `FormAsignaturas` |
| RF-04 (Inscripciones) | `FormInscripciones`, `EsDuplicada()` |
| RF-05, RF-06 (Asistencia) | `FormAsistencia`, `ValidarFecha()` |
| RF-07, RF-08 (Notas) | `FormCalificaciones`, `ValidarNota()` |
| RF-09, RF-10, RF-11 (Informes) | `FormInformes`, `ServicioInformes`, `EsDefinitivo()` |
| RF-12 (Login) | `FormLogin`, `VerificarContrasena()`, límite de 3 intentos, `PoliticaBloqueo` |
| RNF-03 (Seguridad) | `Seguridad` (PBKDF2-SHA256 con sal) + restricción de formato en BD + queries parametrizadas |
| RNF-03 (Contraseñas) | `PoliticaContrasena`, `FormCambiarContrasena` |
| Auditoría | `Bitacora`, `AuditoriaRepositorio`, `FormAuditoria`, tabla `Auditoria` |
| RNF-07 (Errores) | `Conexion.Probar()` y handler global en `Program.cs` |
| Instalación | `InstaladorBaseDatos`, `appsettings.json` |

### Reglas del negocio

- RUT validado con módulo 11.
- Notas entre 1.0 y 7.0.
- Ponderaciones por asignatura no superan 100%.
- No se permite asistencia en fecha futura.
- No se elimina historial: usuarios se desactivan.
- Aprobación: Promedio >= 4.0 y Asistencia >= 75%.
- Resultado **Parcial**: si las evaluaciones con nota no suman el 100% de la ponderación, el estado no es definitivo y no se informa como aprobado ni reprobado.
- Un docente solo accede a la información de sus propias asignaturas; el administrador accede a todo.

### Historial de versiones

| Versión | Cambios |
| :--- | :--- |
| v1.0 | Versión entregada en la evaluación. |
| v1.1 | Scripts de instalación incluidos en el proyecto; límite de 3 intentos en login; control de acceso en informes individuales; estado Parcial; migración a PBKDF2; 5 pruebas nuevas (autenticación y resultado parcial). |
| v1.2 | Configuración en `appsettings.json`; soporte para LocalDB; creación automática de la base en la primera ejecución. |
| v1.3 | Bloqueo de cuenta persistente (3 intentos = 15 minutos); 4 pruebas nuevas del bloqueo. |
| v1.4 | Política de contraseñas; formulario para cambiar la contraseña; cambio obligatorio tras asignación del administrador; 4 pruebas nuevas. |
| v1.5 | Bitácora de auditoría inmutable con pantalla de consulta para el administrador; 2 pruebas nuevas. |
| v1.6 | Mínimo privilegio: cuenta de la aplicación con rol limitado; contraseña desde variable de entorno; configuración local fuera del repositorio. |