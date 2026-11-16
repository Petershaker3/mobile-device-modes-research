@echo off
setlocal
cd /d "%~dp0"

dotnet run --project "src\DeviceToolkit.Desktop\DeviceToolkit.Desktop.csproj" --configuration Release
set "APP_EXIT_CODE=%ERRORLEVEL%"

if not "%APP_EXIT_CODE%"=="0" (
    echo.
    echo The Android Software Technician app could not start. Review the error above.
    pause
)

exit /b %APP_EXIT_CODE%
