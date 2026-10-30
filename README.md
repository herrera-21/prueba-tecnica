# Prueba Técnica - Gestión de Clientes

Aplicación web en **ASP.NET MVC 5 (C#)** con **SQL Server** para autenticación, mantenimiento de clientes y bitácora de cambios.

## Requisitos

- Visual Studio con la carga de trabajo **ASP.NET and web development**
- SQL Server

## Cómo ejecutarla

1. Ejecutar `Database/script.sql` en SQL Server. Crea la base `GestionClientes`, las tablas `Usuarios`, `Clientes` y `Bitacora`, y un usuario `admin` para entrar.
2. Abrir `GestionClientes.sln` en Visual Studio.
3. Presionar **F5**. Se abre en `http://localhost:50500/`.
4. Iniciar sesión con el usuario `admin` y la contraseña `admin123`.

## Cadena de conexión

Está en `GestionClientes/Web.config`:

```xml
<add name="GestionClientesDb"
     connectionString="Server=.\SQLEXPRESS;Database=GestionClientes;Integrated Security=True;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

Si su instancia tiene otro nombre (por ejemplo `localhost` o `.\MSSQLSERVER`), solo cambie la parte de `Server=`.

## Qué incluye

- **Login** validado contra la tabla `Usuarios`. Contraseñas guardadas con PBKDF2 (SHA256, 100.000 iteraciones) y sal aleatoria por usuario.
- **Clientes**: ver, agregar, editar y eliminar. Todas las pantallas requieren sesión iniciada.
- **Bitácora**: registra cada `AGREGAR`, `EDITAR` y `ELIMINAR` con el ID del cliente, el usuario y la fecha/hora. El registro se guarda en la misma transacción que el cambio del cliente.
- Consultas **parametrizadas** (sin concatenar SQL), token anti-forgery en todos los formularios y eliminación solo por POST.

## Estructura

```
GestionClientes.sln
Database/script.sql
GestionClientes/
  Controllers/   Account, Clientes, Bitacora, Home
  Models/        Usuario, Cliente, Bitacora, LoginViewModel
  Data/          Conexion, UsuarioDao, ClienteDao, BitacoraDao
  Seguridad/     Hash (PBKDF2)
  Views/         Account, Clientes, Bitacora, Shared
  Web.config
```

## Notas

- Desarrollado en **C#** con autorización previa; el enunciado pedía VB.NET.
- La bitácora no tiene llave foránea a `Clientes` a propósito, para conservar el historial cuando un cliente se elimina.
- El script se puede volver a ejecutar: borra las tablas y las crea de nuevo.
- Si al abrirlo en otro equipo F5 no arranca, clic derecho en el proyecto **GestionClientes** → **Establecer como proyecto de inicio**.
