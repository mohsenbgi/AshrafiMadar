@echo off
title Smart Factory Client
echo.
echo 🏭 Smart Factory Client Launcher
echo ================================
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET SDK not found. Please install .NET 9.0 SDK
    echo 📥 Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo ✅ .NET SDK found
echo.

REM Restore packages if needed
if not exist "bin" (
    echo 📦 Restoring packages...
    dotnet restore
    if %errorlevel% neq 0 (
        echo ❌ Failed to restore packages
        pause
        exit /b 1
    )
)

REM Build the project
echo 🔨 Building project...
dotnet build -c Release
if %errorlevel% neq 0 (
    echo ❌ Build failed
    pause
    exit /b 1
)

echo ✅ Build successful
echo.
echo 🚀 Starting Smart Factory Client...
echo.

REM Run the application
dotnet run -c Release

echo.
echo 👋 Application ended
pause