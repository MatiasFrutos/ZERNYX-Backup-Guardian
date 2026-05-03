:: Archivo: scripts\reset-database.bat

@echo off
chcp 65001 >nul
title ZERNYX Backup Guardian - Reset Database

echo ============================================================
echo  ZERNYX BACKUP GUARDIAN - RESET DATABASE
echo ============================================================
echo.

cd /d "%~dp0.."

echo [INFO] Se eliminaran las bases SQLite locales.
echo.

set "DB_DEV=src\ZERNYX.BackupGuardian.App\data\zernyx_backup_guardian.db"
set "DB_PUBLISH=publish\data\zernyx_backup_guardian.db"

if exist "%DB_DEV%" (
    del /f /q "%DB_DEV%"
    echo [OK] Eliminada DB desarrollo.
) else (
    echo [INFO] No existe DB desarrollo.
)

if exist "%DB_DEV%-shm" del /f /q "%DB_DEV%-shm"
if exist "%DB_DEV%-wal" del /f /q "%DB_DEV%-wal"

if exist "%DB_PUBLISH%" (
    del /f /q "%DB_PUBLISH%"
    echo [OK] Eliminada DB publish.
) else (
    echo [INFO] No existe DB publish.
)

if exist "%DB_PUBLISH%-shm" del /f /q "%DB_PUBLISH%-shm"
if exist "%DB_PUBLISH%-wal" del /f /q "%DB_PUBLISH%-wal"

echo.
echo ============================================================
echo  BASE DE DATOS RESETEADA
echo ============================================================
echo.
echo Al abrir la app se creara nuevamente.
echo.
pause