# Registro de actividades de tratamiento de datos personales

**Sistema:** Instituto Códice - Sistema de Asistencia y Notas (aplicación de escritorio)
**Marco de referencia:** Ley N° 21.719, que regula la protección y el tratamiento de los datos personales (modifica la Ley N° 19.628)
**Versión del documento:** 1.4 - noviembre de 2026
**Estado:** borrador técnico para revisión de la institución

> Este documento es parte de un proyecto académico y de portafolio. Describe cómo el sistema trata los datos personales para facilitar el cumplimiento de la ley, pero **no constituye asesoría legal**. Las decisiones marcadas como *"a definir por la institución"* corresponden al responsable de los datos y deben validarse con asesoría jurídica.

---

## 1. Responsable del tratamiento

| Campo | Valor |
| :--- | :--- |
| Responsable | Instituto Códice *(institución ficticia del caso de estudio)* |
| Contacto para ejercer derechos | *A definir por la institución* (correo o mesa de ayuda) |
| Delegado de protección de datos | *A definir por la institución* (designación voluntaria) |
| Encargado del tratamiento (proveedor) | No aplica: el sistema se instala y opera en la infraestructura de la institución |

---

## 2. Titulares de los datos

| Titular | Descripción |
| :--- | :--- |
| **Estudiantes** | Personas inscritas en las asignaturas del instituto |
| **Docentes** | Usuarios del sistema que dictan asignaturas |
| **Administradores** | Usuarios del sistema que gestionan la información académica |

> **Menores de edad:** el sistema está pensado para educación superior, por lo que se asume que los estudiantes son mayores de edad. Si la institución atendiera a menores, la ley exige resguardos adicionales que este documento no cubre.

---

## 3. Actividades de tratamiento

### 3.1 Gestión académica de estudiantes

| Campo | Detalle |
| :--- | :--- |
| **Finalidad** | Registrar a los estudiantes, inscribirlos en asignaturas, controlar asistencia, registrar calificaciones y emitir informes de rendimiento |
| **Base de licitud** | Ejecución de la relación educacional entre el estudiante y la institución, y cumplimiento de las obligaciones legales de la institución sobre registros académicos *(a validar por la institución)* |
| **Datos tratados** | RUT, nombres, apellidos, correo (opcional), curso, estado activo/inactivo, marca de tratamiento bloqueado, inscripciones, asistencia por fecha y calificaciones |
| **Uso del correo** | Comunicaciones académicas de la institución (avisos, citaciones y resultados), que se realizan fuera del sistema. La aplicación no envía correos |
| **Datos sensibles** | No se tratan datos sensibles (salud, origen étnico, creencias, etc.). Las notas y la asistencia son datos personales, pero no sensibles |
| **Tablas** | `Estudiante`, `Inscripcion`, `Asistencia`, `Calificacion` |
| **Quién accede** | Administrador: todos los estudiantes. Docente: solo los estudiantes inscritos en las asignaturas que dicta |
| **Origen de los datos** | El propio estudiante, al matricularse (ingresados por el administrador); asistencia y notas, registradas por el docente |
| **Comunicaciones a terceros** | Ninguna desde el sistema. Los informes exportados (Excel/PDF) son de uso interno |
| **Transferencia internacional** | Ninguna: la base de datos está en un servidor de la institución |
| **Conservación** | *A definir por la institución.* Propuesta: mientras el estudiante esté vigente, más el plazo legal de conservación de registros académicos. Los estudiantes no se borran: se **desactivan**, para conservar su historial académico |

### 3.2 Gestión de usuarios y control de acceso

| Campo | Detalle |
| :--- | :--- |
| **Finalidad** | Autenticar a los usuarios, controlar qué información puede ver cada uno y proteger las cuentas |
| **Base de licitud** | Relación laboral o contractual con la institución, y deber de seguridad del responsable |
| **Datos tratados** | Nombre de usuario, nombre, rol, hash de la contraseña (nunca la contraseña), estado activo, intentos fallidos, fecha de término del bloqueo y obligación de cambiar la contraseña. Docentes: además RUT y correo |
| **Tablas** | `Usuario`, `Docente` |
| **Quién accede** | Administrador (gestión de docentes). Cada usuario puede cambiar su propia contraseña |
| **Conservación** | Mientras exista la relación con la institución. Los usuarios no se borran: se **desactivan**, porque sus acciones quedan asociadas en la bitácora |

### 3.3 Bitácora de auditoría

| Campo | Detalle |
| :--- | :--- |
| **Finalidad** | Dejar trazabilidad de los accesos y de los cambios sobre datos personales, para detectar usos indebidos y poder demostrar cumplimiento |
| **Base de licitud** | Deber de seguridad y de responsabilidad proactiva del responsable |
| **Datos tratados** | Fecha y hora (UTC), usuario que actuó, acción, detalle, valor anterior y nuevo. En los cambios de nota, el estudiante se identifica **solo por RUT** (minimización: sin nombres) |
| **Tabla** | `Auditoria` |
| **Quién accede** | Solo el administrador, en modo de solo lectura |
| **Protección** | La tabla es inmutable: un trigger rechaza cualquier modificación o borrado, y la cuenta de la aplicación tiene denegado UPDATE y DELETE |
| **Conservación** | *A definir por la institución.* Propuesta: 2 años, con depuración controlada por un administrador de base de datos y documentada |

### 3.4 Exportación de informes

| Campo | Detalle |
| :--- | :--- |
| **Finalidad** | Entregar informes de rendimiento por estudiante o por curso, en Excel o PDF |
| **Datos tratados** | Los mismos que se ven en pantalla: RUT, nombre, notas, promedio, asistencia y estado |
| **Controles** | Solo se exporta lo que el usuario tiene permitido ver. Cada exportación queda en la bitácora (quién, qué informe y en qué formato). Los documentos incluyen la leyenda *"Documento con datos personales. Uso interno"* |
| **Riesgo residual** | Una vez exportado, el archivo queda fuera del control del sistema. Se recomienda guardarlo en ubicaciones autorizadas por la institución y no en carpetas sincronizadas con servicios personales en la nube |

---

## 4. Medidas de seguridad implementadas

| Medida | Implementación |
| :--- | :--- |
| Contraseñas | PBKDF2-SHA256 con 600.000 iteraciones y sal aleatoria; la base rechaza contraseñas sin hash |
| Protección de cuentas | Bloqueo de 15 minutos tras 3 intentos fallidos; política de contraseñas; cambio obligatorio de contraseñas asignadas |
| Control de acceso | Por rol y por asignatura; validado en la interfaz y en la capa de negocio; si no hay permisos definidos, no se muestra nada (falla cerrada); cubierto por pruebas automáticas |
| Mínimo privilegio | La aplicación se conecta con una cuenta de base de datos limitada (`app_codice`) que no puede borrar datos (salvo inscripciones), modificar la bitácora ni cambiar la estructura |
| Secretos | La contraseña de la base de datos se lee de una variable de entorno y no se guarda en el código ni en el repositorio |
| Integridad | Validaciones en el código y en la base (CHECK, UNIQUE, FOREIGN KEY); consultas parametrizadas contra inyección SQL |
| Trazabilidad | Bitácora inmutable de accesos, fallos, bloqueos, cambios de notas, desactivaciones y exportaciones |
| Cifrado en tránsito | La conexión a SQL Server siempre va cifrada. Con `CertificadoServidor` configurado, la app solo acepta el certificado del servidor verdadero; sin él (modo desarrollo) no lo verifica. Procedimiento en [CertificadoSQL.md](CertificadoSQL.md) |

---

## 5. Derechos de los titulares

La ley reconoce a los titulares, entre otros, los derechos de **acceso, rectificación, supresión, oposición, portabilidad y bloqueo** de sus datos.

| Derecho | Cómo lo resuelve hoy el sistema | Estado |
| :--- | :--- | :--- |
| Acceso | El administrador exporta **todos** los datos del estudiante en un archivo JSON; la exportación queda en la bitácora | ✅ v1.9 |
| Rectificación | El administrador corrige los datos personales; los cambios de notas quedan en la bitácora con su valor anterior y nuevo | ✅ |
| Supresión | El historial académico se conserva por obligación legal *(a validar)*; el estudiante se puede desactivar | ⚠️ Requiere criterio de la institución |
| Oposición | No hay tratamientos con fines distintos a la gestión académica (sin marketing ni perfilamiento) | ✅ No aplica |
| Portabilidad | La exportación usa JSON, un formato estructurado y de uso común que otro sistema puede leer | ✅ v1.9 |
| Bloqueo | El administrador marca "Tratamiento bloqueado": el estudiante deja de aparecer en la gestión académica, su historial se conserva y sigue disponible la exportación de sus datos | ✅ v1.9 |

---

## 6. Brechas detectadas y plan de acción

| Id | Brecha | Acción propuesta | Paso del proyecto | Estado |
| :--- | :--- | :--- | :--- | :--- |
| B1 | Los titulares no reciben información sobre el tratamiento de sus datos | Agregar un **aviso de privacidad** accesible desde la aplicación | 9.2 | ✅ Resuelta (v1.9) |
| B2 | No hay forma de entregar al titular todos sus datos (acceso y portabilidad) | Botón **"Exportar datos del estudiante"** con registro en la bitácora | 9.3 | ✅ Resuelta (v1.9) |
| B3 | No se puede bloquear el tratamiento de un estudiante | Marca de **tratamiento bloqueado**, que lo excluye de las operaciones sin borrar su historial | 9.4 | ✅ Resuelta (v1.9) |
| B4 | El correo del estudiante se usa para comunicaciones académicas de la institución fuera del sistema, pero esa finalidad no está informada al titular | Declarar esta finalidad en el aviso de privacidad. Si en el futuro se agrega un portal para estudiantes, actualizar este registro con las nuevas finalidades (inicio de sesión, recuperación de contraseña y notificaciones) | 9.2 | ✅ Resuelta (v1.9) |
| B5 | El certificado del servidor SQL no se verifica | Instalar un certificado de confianza en el servidor y desactivar `TrustServerCertificate` en producción | 9.5 | 🟡 Resuelta en la aplicación (v1.9); falta instalar el certificado en el servidor de producción |
| B6 | No existe un procedimiento ante filtraciones de datos | Redactar un **plan de respuesta ante incidentes** | 9.6 | ✅ Resuelta (v1.9): ver [PlanRespuestaIncidentes.md](PlanRespuestaIncidentes.md) |
| B7 | Los plazos de conservación no están definidos | Definir los plazos con la institución y documentarlos en este registro | Institución | ⏳ Pendiente |

---

## 7. Evolución prevista del sistema

Si se incorpora un **portal web para estudiantes** (registro, consulta de notas y asistencia, y ejercicio de derechos), deberá:

- Ser una aplicación separada que acceda a los datos **a través de una API** con control de acceso por titular. Los estudiantes nunca se conectan directamente a la base de datos.
- Agregarse a este registro como una **nueva actividad de tratamiento**, con sus propias finalidades, datos y plazos de conservación.
- Informar a los estudiantes, al momento del registro, mediante el aviso de privacidad actualizado.

---

## 8. Historial del documento

| Versión | Fecha | Cambios |
| :--- | :--- | :--- |
| 1.0 | Noviembre de 2026 | Versión inicial, elaborada a partir de la estructura de la base de datos (v1.8 del sistema) |
| 1.1 | Noviembre de 2026 | Se declara la finalidad del correo del estudiante (B4) y se agrega la evolución prevista hacia un portal web |
| 1.2 | Noviembre de 2026 | Se resuelven B1 (aviso de privacidad), B2 (acceso y portabilidad), B3 (bloqueo) y B4 (finalidad del correo) |
| 1.3 | Noviembre de 2026 | B5: la aplicación permite verificar el certificado del servidor SQL |
| 1.4 | Noviembre de 2026 | B6: se agrega el plan de respuesta ante incidentes |