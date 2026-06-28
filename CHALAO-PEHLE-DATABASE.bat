@echo off
cd /d "%~dp0"
echo One-time: creating LocalDB database + sample users (password123) ...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Run-App.ps1" -SetupDatabase
echo.
echo Done. Now run CHALAO.bat to open the website.
pause
