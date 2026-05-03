:: Archivo: scripts\run-dev.bat

@echo off
chcp 65001 >nul
title ZERNYX Backup Guardian - Run Dev

echo ============================================================
echo  ZERNYX BACKUP GUARDIAN - MODO DESARROLLO
echo ============================================================
echo.

cd /d "%~dp0.."

echo [1/2] Verificando proyecto...

if not exist "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj" (
    echo [ERROR] No se encontro el archivo:
    echo src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj
    echo.
    pause
    exit /b 1
)

echo [2/2] Ejecutando aplicacion...
echo.

dotnet run --project "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj"

if errorlevel 1 (
    echo.
    echo [ERROR] La aplicacion finalizo con errores.
    pause
    exit /b 1
)

echo.
echo ============================================================
echo  APLICACION CERRADA
echo ============================================================
echo.
pause