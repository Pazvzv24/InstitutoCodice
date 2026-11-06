# Plan de respuesta ante incidentes de seguridad de datos personales

**Sistema:** Instituto Códice - Sistema de Asistencia y Notas
**Marco de referencia:** Ley N° 21.719 de protección de datos personales
**Versión del documento:** 1.0 - noviembre de 2026
**Estado:** borrador técnico para revisión de la institución

> Proyecto académico y de portafolio. Este plan describe cómo actuar ante un incidente usando las herramientas del sistema, pero **no constituye asesoría legal**. Los criterios y plazos de notificación deben validarse con asesoría jurídica y con la normativa vigente al momento del incidente.

---

## 1. Qué es un incidente

Una **vulneración de seguridad** es cualquier destrucción, pérdida, alteración, divulgación o acceso no autorizado a datos personales. Ejemplos en este sistema:

| Tipo | Ejemplo |
| :--- | :--- |
| Acceso no autorizado | Alguien ingresa con la cuenta de un docente cuya contraseña fue robada |
| Divulgación | Un informe exportado (Excel, PDF o JSON) se envía por error a una persona externa o queda en una carpeta compartida |
| Alteración | Se modifican notas sin autorización |
| Credencial expuesta | La contraseña de la cuenta `app_codice` se publica en un repositorio o un chat |
| Pérdida | Se pierde o roban un computador con archivos exportados |

---

## 2. Roles

| Rol | Quién | Responsabilidad |
| :--- | :--- | :--- |
| **Responsable del incidente** | Administrador del sistema | Coordina la respuesta, contiene y registra |
| **Responsable de los datos** | Dirección Académica *(a definir por la institución)* | Decide las notificaciones y comunica a la comunidad |
| **Delegado de protección de datos** | *(Opcional, a definir por la institución)* | Asesora sobre las obligaciones legales |
| **Administrador de base de datos** | *(A definir por la institución)* | Ejecuta las acciones en SQL Server |

Cualquier persona que sospeche un incidente debe avisar **de inmediato** al responsable del incidente. Reportar un falso positivo no tiene consecuencias; no reportar un incidente real sí las puede tener.

---

## 3. Fases de la respuesta

### Fase 1: Detectar

Fuentes de alerta:

- **Bitácora de auditoría** (menú *Bitácora de auditoría*, solo administrador). Señales de alerta:
  - Muchos `LoginFallido`, `CuentaBloqueada` o `LoginEnCuentaBloqueada` en poco tiempo.
  - `InformeExportado` o `DatosTitularExportados` hechos por un usuario que normalmente no exporta, o en horarios inusuales.
  - `NotaModificada` en asignaturas que el usuario no dicta, o muchas modificaciones seguidas.
- Avisos de usuarios: "yo no hice ese cambio", "no puedo entrar a mi cuenta".
- Credenciales o archivos con datos encontrados donde no corresponde.

Consultas de apoyo en SSMS (solo lectura):

```sql
-- Intentos de ingreso fallidos en las últimas 24 horas, por usuario
SELECT NombreUsuario, COUNT(*) AS Intentos, MIN(FechaUtc) AS Primero, MAX(FechaUtc) AS Ultimo
FROM dbo.Auditoria
WHERE Accion IN ('LoginFallido', 'CuentaBloqueada', 'LoginEnCuentaBloqueada')
  AND FechaUtc >= DATEADD(HOUR, -24, SYSUTCDATETIME())
GROUP BY NombreUsuario
ORDER BY Intentos DESC;

-- Datos personales que salieron del sistema en los últimos 7 días
SELECT FechaUtc, NombreUsuario, Accion, Detalle, ValorNuevo AS Formato
FROM dbo.Auditoria
WHERE Accion IN ('InformeExportado', 'DatosTitularExportados')
  AND FechaUtc >= DATEADD(DAY, -7, SYSUTCDATETIME())
ORDER BY FechaUtc DESC;

-- Todo lo que hizo un usuario sospechoso (reemplazar el nombre)
SELECT FechaUtc, Accion, Detalle, ValorAnterior, ValorNuevo
FROM dbo.Auditoria
WHERE NombreUsuario = N'usuario_sospechoso'
ORDER BY FechaUtc DESC;
```

> La bitácora es **inmutable**: nadie puede borrar ni modificar sus registros, por lo que sirve como evidencia del incidente.

### Fase 2: Contener (primeras horas)

Objetivo: detener el daño **sin destruir evidencia**.

| Situación | Acción |
| :--- | :--- |
| Cuenta de usuario comprometida | Desactivarla desde **Gestión de docentes** (queda en la bitácora). Si después se reactiva, asignarle una contraseña nueva: el sistema obliga a cambiarla en el siguiente ingreso |
| Contraseña de `app_codice` expuesta | Deshabilitar la cuenta en SQL Server, cambiar la contraseña y actualizar la variable de entorno `INSTITUTOCODICE_DB_CLAVE` en cada equipo (ver consultas abajo) |
| Archivo exportado divulgado | Pedir al receptor que lo elimine y lo confirme por escrito; retirar el archivo de carpetas compartidas o sincronizadas |
| Equipo perdido o robado | Cambiar las contraseñas de los usuarios que lo usaban; si tenía la variable de entorno de `app_codice`, cambiar también esa contraseña |

```sql
-- Ver las sesiones abiertas con la cuenta de la aplicación
SELECT session_id, login_name, host_name, program_name, login_time
FROM sys.dm_exec_sessions
WHERE login_name = N'app_codice';

-- Deshabilitar la cuenta de la aplicación (la app deja de conectarse)
ALTER LOGIN app_codice DISABLE;

-- Cerrar una sesión sospechosa (reemplazar 55 por el session_id)
-- KILL 55;

-- Cuando el incidente esté controlado: nueva contraseña y volver a habilitar
-- ALTER LOGIN app_codice WITH PASSWORD = N'<nueva contraseña robusta>';
-- ALTER LOGIN app_codice ENABLE;
```

> Nunca escribas la contraseña nueva en este documento, en el repositorio ni en un chat.

### Fase 3: Evaluar

Responder y dejar por escrito:

1. **Qué datos** se vieron afectados (RUT, nombres, correos, notas, asistencia, cuentas de usuario).
2. **De cuántas personas**, aproximadamente.
3. **Cómo ocurrió** y **desde cuándo**: la bitácora permite reconstruir la línea de tiempo.
4. **Si los datos salieron del control** de la institución o solo hubo riesgo.
5. **Qué consecuencias** pueden tener las personas afectadas.

Datos que trata este sistema y su nivel de riesgo:

| Dato | Riesgo si se expone |
| :--- | :--- |
| RUT, nombres y correo | Medio: permiten suplantación y correos engañosos (phishing) |
| Notas y asistencia | Medio: información académica privada |
| Hash de contraseñas | Bajo: PBKDF2 con 600.000 iteraciones y sal; igual se recomienda cambiar las contraseñas |
| Datos sensibles (salud, origen, creencias, etc.) | No los trata el sistema |
| Datos de menores de edad | Se asume que no hay (educación superior). Si los hubiera, el riesgo y las obligaciones aumentan |

### Fase 4: Notificar

| A quién | Cuándo | Cómo |
| :--- | :--- | :--- |
| **Agencia de Protección de Datos Personales** | Cuando la vulneración genere un **riesgo razonable** para los derechos de los titulares, por los medios más rápidos posibles y sin demoras injustificadas | Informe con la naturaleza del incidente, los datos y el número aproximado de personas afectadas, las posibles consecuencias y las medidas adoptadas |
| **Titulares afectados** | Obligatorio cuando el incidente involucre **datos sensibles, datos de niños, niñas o adolescentes, o datos sobre obligaciones económicas, financieras, bancarias o comerciales**. En los demás casos, la institución evalúa hacerlo como buena práctica | Mensaje en lenguaje claro: qué pasó, qué datos se vieron afectados, qué riesgos tiene y qué puede hacer la persona |
| **Comunidad interna** | Si el incidente afecta el funcionamiento del sistema | Aviso de la Dirección Académica |

> **Ley 21.663 (Ley Marco de Ciberseguridad):** si la institución fuera calificada como prestador de servicios esenciales u operador de importancia vital, tendría además obligaciones de reporte al CSIRT Nacional, con plazos propios. Debe evaluarse caso a caso.

**Modelo de mensaje a los titulares:**

> Estimado/a [nombre]:
>
> El [fecha] detectamos un incidente de seguridad en el Sistema de Asistencia y Notas del Instituto Códice que afectó [descripción breve]. Los datos involucrados fueron: [lista]. **No** se vieron afectados: [lista].
>
> Ya tomamos estas medidas: [lista]. Le recomendamos [acciones concretas, por ejemplo: desconfiar de correos que pidan sus datos a nombre del instituto].
>
> Para consultas o para ejercer sus derechos, contacte a [canal de la Dirección Académica].

### Fase 5: Recuperar

- Corregir la causa (por ejemplo: cambiar contraseñas, revisar permisos o corregir el error).
- Si hubo alteración de notas, restaurar los valores correctos usando la columna **ValorAnterior** de la bitácora. Cada corrección queda registrada como un cambio nuevo.
- Volver a habilitar cuentas y servicios solo cuando la causa esté corregida.
- Vigilar la bitácora con más frecuencia durante las semanas siguientes.

### Fase 6: Aprender

Dentro de las dos semanas siguientes, hacer una revisión sin buscar culpables:

- ¿Qué permitió el incidente? ¿Se detectó a tiempo?
- ¿Qué cambios evitan que se repita? Agregarlos al plan de acción del [registro de tratamiento](RegistroTratamiento.md).
- ¿Este plan funcionó? Actualizarlo si es necesario.

---

## 4. Registro de incidentes

**Todo incidente se registra, aunque no se notifique a nadie.** El registro permite demostrar que la institución actuó con diligencia y justificar por qué decidió notificar o no.

| Campo | Contenido |
| :--- | :--- |
| Código | INC-AAAA-NNN (por ejemplo, INC-2026-001) |
| Fechas | Ocurrencia (si se conoce), detección y cierre |
| Descripción | Qué pasó y cómo se detectó |
| Datos y personas afectadas | Tipos de datos y número aproximado de titulares |
| Evaluación del riesgo | Bajo, medio o alto, con su justificación |
| Notificaciones | A quién se notificó y cuándo, o **por qué no** se notificó |
| Medidas | De contención, de recuperación y para que no se repita |
| Evidencia | Consultas de la bitácora, capturas y correos, guardados en un lugar de acceso restringido |
| Responsable | Quién lideró la respuesta |

---

## 5. Revisión del plan

- Revisar este plan **una vez al año** o después de cada incidente.
- Hacer un **simulacro** al menos una vez al año (por ejemplo: "la contraseña de `app_codice` apareció en un repositorio público") y medir cuánto se demora el equipo en contenerlo.

| Versión | Fecha | Cambios |
| :--- | :--- | :--- |
| 1.0 | Noviembre de 2026 | Versión inicial |