:: Archivo: scripts\publish-exe.bat

@echo off
chcp 65001 >nul
title ZERNYX Backup Guardian - Publicar EXE

echo ============================================================
echo  ZERNYX BACKUP GUARDIAN - CREAR EXE PORTABLE
echo ============================================================
echo.

cd /d "%~dp0.."

echo [1/5] Verificando proyecto...

if not exist "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj" (
    echo [ERROR] No se encontro el archivo:
    echo src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj
    echo.
    pause
    exit /b 1
)

echo [2/5] Limpiando publish...

if exist "publish" (
    rmdir /s /q "publish"
)

mkdir "publish" 2>nul

echo.
echo [3/5] Restaurando paquetes...
dotnet restore "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj"

if errorlevel 1 (
    echo.
    echo [ERROR] Fallo dotnet restore.
    pause
    exit /b 1
)

echo.
echo [4/5] Publicando EXE portable...
dotnet publish "src\ZERNYX.BackupGuardian.App\ZERNYX.BackupGuardian.App.csproj" ^
 -c Release ^
 -r win-x64 ^
 --self-contained true ^
 /p:PublishSingleFile=true ^
 /p:IncludeNativeLibrariesForSelfExtract=true ^
 /p:EnableCompressionInSingleFile=true ^
 -o "publish"

if errorlevel 1 (
    echo.
    echo [ERROR] Fallo dotnet publish.
    pause
    exit /b 1
)

echo.
echo [5/5] Copiando documentacion basica...

if exist "README.txt" copy /y "README.txt" "publish\README.txt" >nul
if exist "LICENSE.txt" copy /y "LICENSE.txt" "publish\LICENSE.txt" >nul

mkdir "publish\data" 2>nul
mkdir "publish\logs" 2>nul
mkdir "publish\reports" 2>nul
mkdir "publish\storage" 2>nul

echo.
echo ============================================================
echo  EXE CREADO CORRECTAMENTE
echo ============================================================
echo.
echo Archivo final:
echo %CD%\publish\ZERNYX.BackupGuardian.App.exe
echo.
pause