@echo off
cls

SET START_DIR=%CD%
SET SCRIPT_DIR=%~dp0

echo Starting Paket...

"%SCRIPT_DIR%.paket\paket.bootstrapper.exe"
if errorlevel 1 (
    exit /b %errorlevel%
)

"%SCRIPT_DIR%.paket\paket.exe" "restore"
if errorlevel 1 (
    exit /b %errorlevel%
)
echo Starting Fake...

"%SCRIPT_DIR%packages\build\FAKE\tools\Fake.exe" "%SCRIPT_DIR%tools\make.fsx" %*