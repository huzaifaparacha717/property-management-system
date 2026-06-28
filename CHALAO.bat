@echo off
cd /d "%~dp0"
echo Starting Property Rental (http://localhost:56182/) ...
start "" powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Run-App.ps1"
exit /b 0
