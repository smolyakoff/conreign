@echo off
cls

"%~dp0.paket\paket.bootstrapper.exe"
if errorlevel 1 (
    exit /b %errorlevel%
)

"%~dp0.paket\paket.exe" "restore"
if errorlevel 1 (
    exit /b %errorlevel%
)

"%~dp0packages\FAKE\tools\Fake.exe" "%~dp0build.fsx" %*