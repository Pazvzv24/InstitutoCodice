# Verificación del certificado de SQL Server

**Objetivo:** que la aplicación compruebe que se conecta al servidor SQL verdadero y no a un impostor (ataque *man in the middle*). Esto importa sobre todo cuando la app usa la cuenta `app_codice`, porque su contraseña viaja por la red.

## Cómo funciona

La conexión **siempre va cifrada** (TLS). Lo que cambia es si la app **verifica** el certificado del servidor:

| Modo | Configuración | Comportamiento |
| :--- | :--- | :--- |
| **Desarrollo** | `"CertificadoServidor": ""` | Acepta el certificado autogenerado de SQL Server sin verificarlo. Es cómodo en un equipo local, pero no protege contra un servidor impostor (riesgo aceptado solo en desarrollo) |
| **Producción** | `"CertificadoServidor": "C:\\ruta\\sqlserver.cer"` | Solo acepta **ese** certificado (fijación de certificado). Si el servidor presenta otro, la conexión se rechaza |

El valor se configura en `appsettings.json` o, solo para un equipo, en `appsettings.local.json` (que no se sube al repositorio). Si la ruta no existe, la app avisa al iniciar y no se conecta.

> En una institución real, lo ideal es un certificado emitido por la autoridad certificadora (CA) de la organización. La fijación con un certificado propio, descrita aquí, es una alternativa válida para un servidor único o un laboratorio.

---

## Procedimiento para SQL Server Express en Windows

Requiere una sesión de **administrador**. Toma unos 30 minutos.

### 1. Crear el certificado

Abre **PowerShell como administrador** y ejecuta:

```powershell
$fqdn = [System.Net.Dns]::GetHostEntry($env:COMPUTERNAME).HostName

$cert = New-SelfSignedCertificate `
    -DnsName $fqdn, $env:COMPUTERNAME, "localhost" `
    -CertStoreLocation "Cert:\LocalMachine\My" `
    -FriendlyName "InstitutoCodice SQL Server" `
    -KeyExportPolicy Exportable `
    -KeySpec KeyExchange `
    -Provider "Microsoft RSA SChannel Cryptographic Provider" `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") `
    -NotAfter (Get-Date).AddYears(2)

$cert.Thumbprint
```

- El primer nombre (`$fqdn`) es el nombre completo del equipo: SQL Server solo ofrece certificados cuyo nombre coincide con él.
- `1.3.6.1.5.5.7.3.1` indica que el certificado sirve para **autenticar un servidor**.
- Anota la huella (*Thumbprint*) que aparece al final.

### 2. Dar permiso a SQL Server sobre la clave privada

1. Presiona **Windows + R**, escribe `certlm.msc` y Enter.
2. Ve a **Personal → Certificados** y busca **InstitutoCodice SQL Server**.
3. Clic derecho → **Todas las tareas → Administrar claves privadas**.
4. **Agregar** → escribe `NT Service\MSSQL$SQLEXPRESS` → **Comprobar nombres** → **Aceptar**.
5. Deja marcado solo **Lectura** → **Aceptar**.

### 3. Asignar el certificado a SQL Server

1. Abre **SQL Server Configuration Manager**.
2. **Configuración de red de SQL Server → Protocolos de SQLEXPRESS** → clic derecho → **Propiedades**.
3. Pestaña **Certificado** → elige **InstitutoCodice SQL Server** → **Aceptar**.
4. En **Servicios de SQL Server**, reinicia **SQL Server (SQLEXPRESS)**.

### 4. Exportar la parte pública

En la misma ventana de PowerShell:

```powershell
New-Item -ItemType Directory -Force -Path "C:\InstitutoCodice" | Out-Null
Export-Certificate -Cert $cert -FilePath "C:\InstitutoCodice\sqlserver.cer"
```

El archivo `.cer` contiene **solo la parte pública**: no permite hacerse pasar por el servidor, así que se puede copiar a los equipos que usan la app. La clave privada nunca sale del servidor.

### 5. Configurar la aplicación

En `InstitutoCodice.App/appsettings.local.json`:

```json
{
  "BaseDatos": {
    "Autenticacion": "Sql",
    "CertificadoServidor": "C:\\InstitutoCodice\\sqlserver.cer"
  }
}
```

### 6. Comprobar

1. **Prueba positiva:** abre la app. Debe conectarse normalmente.
2. **Prueba negativa:** cambia temporalmente `CertificadoServidor` por la ruta de otro archivo `.cer` (por ejemplo, uno creado con el paso 1 con otro nombre). La app **debe rechazar** la conexión. Así se comprueba que la verificación funciona.
3. Vuelve a dejar la ruta correcta.

---

## Mantenimiento

- El certificado vence en **2 años** (`-NotAfter`). Antes de esa fecha, crea uno nuevo, repite los pasos 2 a 5 y distribuye el nuevo `.cer`.
- Si el certificado del servidor cambia y la app no se actualiza, la conexión se rechaza: es el comportamiento esperado.
- El archivo `.cer` es propio de cada servidor: no se sube al repositorio, igual que `appsettings.local.json`.