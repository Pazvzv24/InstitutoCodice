# Política de seguridad

Gracias por ayudar a mantener seguro este proyecto. 🔒

## Versiones con soporte

| Versión | Soporte de seguridad |
| :--- | :---: |
| Última versión publicada (rama `master`) | ✅ |
| Versiones anteriores | ❌ |

## Cómo reportar una vulnerabilidad

**No abras un *issue* público** para reportar una vulnerabilidad: cualquier persona podría aprovecharla antes de que se corrija.

Usa el **reporte privado de vulnerabilidades** de GitHub:

1. Ve a la pestaña **Security** de este repositorio.
2. Haz clic en **Report a vulnerability**.
3. Completa el formulario. El reporte solo lo ve la mantenedora del proyecto.

Incluye, si es posible:

- Qué componente está afectado (por ejemplo: login, informes, exportación, scripts SQL, workflow).
- Los pasos para reproducir el problema.
- El impacto que podría tener.
- Una propuesta de corrección, si la tienes.

> **No incluyas datos personales reales** en el reporte. El sistema trae datos de demostración ficticios, que puedes usar para reproducir el problema.

Se aceptan reportes en español o en inglés.

## Qué puedes esperar

| Etapa | Plazo objetivo |
| :--- | :--- |
| Confirmación de recepción | 7 días |
| Evaluación inicial | 14 días |
| Corrección o plan de acción | Según la gravedad |

Este es un proyecto académico y de portafolio mantenido por una sola persona, por lo que los plazos son un objetivo y no un compromiso contractual. Cuando la vulnerabilidad esté corregida, se publicará el aviso correspondiente y, si lo deseas, se te dará el crédito por el hallazgo.

## Alcance

**Dentro del alcance:**

- El código de la aplicación (`InstitutoCodice.App`, `InstitutoCodice.Datos`, `InstitutoCodice.Negocio`).
- Los scripts de base de datos (`BaseDatos/`).
- El workflow de integración continua (`.github/`).

**Fuera del alcance:**

- Las **cuentas de demostración** documentadas en el README (`admin`, `pmunoz`, `cvaldes`). Sus contraseñas son públicas a propósito y solo sirven para pruebas locales.
- Ataques que requieren acceso de administrador al equipo o al servidor SQL.
- Vulnerabilidades de paquetes de terceros. Repórtalas a su proyecto de origen; si afectan a este sistema, avísanos igual para actualizar la dependencia.

## Controles de seguridad del proyecto

- [Modelo de amenazas (STRIDE)](docs/ModeloAmenazas.md)
- [Registro de actividades de tratamiento (Ley 21.719)](docs/RegistroTratamiento.md)
- [Plan de respuesta ante incidentes](docs/PlanRespuestaIncidentes.md)
- [Verificación del certificado de SQL Server](docs/CertificadoSQL.md)