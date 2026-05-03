 ZERNYX BACKUP GUARDIAN
============================================================

Software de escritorio para Windows desarrollado por ZERNYX Tech Studio.

ZERNYX Backup Guardian permite crear copias de seguridad manuales y programadas
de carpetas importantes, mantener un historial operativo y exportar reportes
en formato TXT.

El programa ya se encuentra compilado como EXE portable, por lo que puede
utilizarse directamente en Windows sin necesidad de abrir el código fuente
ni ejecutar comandos de desarrollo.


 ESTADO DEL PRODUCTO
============================================================

Producto:
ZERNYX Backup Guardian

Versión:
v1.0.0

Estado:
EXE portable disponible para uso

Desarrollado por:
ZERNYX Tech Studio

Sistema objetivo:
Windows 10 / Windows 11


 CÓMO USAR EL PROGRAMA
============================================================

1. Abrir la carpeta del proyecto.
2. Ingresar a la carpeta:

   publish

3. Ejecutar el archivo:

   ZERNYX.BackupGuardian.App.exe

4. Crear una nueva tarea de backup.
5. Seleccionar la carpeta de origen.
6. Seleccionar la carpeta de destino.
7. Ejecutar el backup manualmente o programar días y horarios.
8. Revisar el historial operativo desde la aplicación.
9. Exportar reportes TXT si es necesario.


 UBICACIÓN DEL EXE
============================================================

El ejecutable final se encuentra en:

publish\ZERNYX.BackupGuardian.App.exe

Este archivo es el que debe abrir el usuario final para utilizar el sistema.


 FUNCIONES PRINCIPALES
============================================================

- Crear tareas de backup.
- Seleccionar carpeta origen.
- Seleccionar carpeta destino.
- Ejecutar backup manual.
- Programar backups por días y horarios.
- Activar o desactivar tareas.
- Guardar historial operativo en SQLite.
- Exportar historial a TXT.
- Registrar logs técnicos.
- Utilizar el sistema desde un EXE portable.


 BASE DE DATOS
============================================================

La base de datos SQLite se crea automáticamente al iniciar la aplicación.

Archivo generado:

data\zernyx_backup_guardian.db

No es necesario configurar la base de datos manualmente.

 CARPETAS IMPORTANTES
============================================================

database
Carpeta reservada para recursos o scripts relacionados con la base de datos.

docs
Documentación del proyecto.

logs
Registros técnicos generados por la aplicación.

publish
Carpeta donde se encuentra el EXE portable listo para usar.

reports
Carpeta destinada a reportes exportados.

scripts
Scripts de desarrollo, compilación y mantenimiento.

storage
Carpeta auxiliar para almacenamiento interno del sistema.

tools
Herramientas complementarias del proyecto.

src
Código fuente de la aplicación.

 NOTA PARA USUARIOS
============================================================

Para usar ZERNYX Backup Guardian no hace falta instalar Visual Studio,
.NET SDK ni herramientas de desarrollo.

Solo se debe ejecutar:

publish\ZERNYX.BackupGuardian.App.exe

La aplicación está pensada para uso operativo directo en equipos con
Windows 10 o Windows 11.


 NOTA OPERATIVA
============================================================

En la versión 1.0, los backups automáticos funcionan mientras el programa
está abierto.

Si el programa se cierra, las tareas programadas no se ejecutan hasta que
la aplicación vuelva a abrirse.

Para una próxima versión se puede incorporar:

- Inicio automático con Windows.
- Minimizar a bandeja.
- Servicio de Windows.
- Backup aunque la aplicación esté cerrada.
- Backup ZIP.
- Backup incremental.
- Cifrado.
- Restauración de archivos.

 INFORMACIÓN TÉCNICA
============================================================

Lenguaje:
C#

Framework:
.NET 8

Interfaz:
Windows Forms

Base de datos:
SQLite

Sistema objetivo:
Windows 10 / Windows 11

 EJECUTAR EN DESARROLLO
============================================================

Esta sección es solo para desarrolladores.

Desde la carpeta raíz:

scripts\run-dev.bat

O manualmente:

dotnet run --project src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj

 COMPILAR EL PROYECTO
============================================================

Esta sección es solo para desarrolladores.

scripts\build-release.bat

O manualmente:

dotnet build src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj -c Release

 GENERAR EXE PORTABLE
============================================================

Esta sección es solo para desarrolladores.

scripts\publish-exe.bat

El EXE final se genera en:

publish\ZERNYX.BackupGuardian.App.exe

 LIMPIAR COMPILACIÓN
============================================================

Esta sección es solo para desarrolladores.

scripts\clean.bat


 SOPORTE Y MANTENIMIENTO
============================================================

Ante errores o comportamientos inesperados, revisar:

- La carpeta logs.
- El historial operativo dentro del sistema.
- Que las carpetas de origen y destino sigan existiendo.
- Que el usuario de Windows tenga permisos de lectura y escritura.
- Que el programa permanezca abierto para ejecutar backups programados.

 DESARROLLO
============================================================

Desarrollado por:
ZERNYX Tech Studio

Producto:
ZERNYX Backup Guardian

Versión:
v1.0.0
