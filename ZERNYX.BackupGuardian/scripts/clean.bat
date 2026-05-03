:: Archivo: scripts\clean.bat

@echo off
chcp 65001 >nul
title ZERNYX Backup Guardian - Clean

echo ============================================================
echo  ZERNYX BACKUP GUARDIAN - LIMPIAR COMPILACION
echo ============================================================
echo.

cd /d "%~dp0.."

echo [1/4] Verificando proyecto...

if not exist "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj" (
    echo [ERROR] No se encontro el archivo:
    echo src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj
    echo.
    pause
    exit /b 1
)

echo [2/4] Ejecutando dotnet clean...
dotnet clean "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj"

echo.
echo [3/4] Eliminando bin y obj...

if exist "src\ZERNYX.BackupGuardian.App\bin" (
    rmdir /s /q "src\ZERNYX.BackupGuardian.App\bin"
)

if exist "src\ZERNYX.BackupGuardian.App\obj" (
    rmdir /s /q "src\ZERNYX.BackupGuardian.App\obj"
)

echo.
echo [4/4] Limpiando publish...

if exist "publish" (
    rmdir /s /q "publish"
)

mkdir "publish" 2>nul

echo.
echo ============================================================
echo  LIMPIEZA FINALIZADA
echo ============================================================
echo.
pause