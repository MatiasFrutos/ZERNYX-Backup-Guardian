:: Archivo: scripts\build-release.bat

@echo off
chcp 65001 >nul
title ZERNYX Backup Guardian - Build Release

echo ============================================================
echo  ZERNYX BACKUP GUARDIAN - BUILD RELEASE
echo ============================================================
echo.

cd /d "%~dp0.."

echo [1/3] Verificando proyecto...

if not exist "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj" (
    echo [ERROR] No se encontro el archivo:
    echo src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj
    echo.
    pause
    exit /b 1
)

echo [2/3] Restaurando paquetes...
dotnet restore "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj"

if errorlevel 1 (
    echo.
    echo [ERROR] Fallo dotnet restore.
    pause
    exit /b 1
)

echo.
echo [3/3] Compilando Release...
dotnet build "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj" -c Release --no-restore

if errorlevel 1 (
    echo.
    echo [ERROR] Fallo dotnet build.
    pause
    exit /b 1
)

echo.
echo ============================================================
echo  BUILD FINALIZADO CORRECTAMENTE
echo ============================================================
echo.
pause