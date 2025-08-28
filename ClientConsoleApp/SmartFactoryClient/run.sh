#!/bin/bash

# Smart Factory Client Launcher Script

echo ""
echo "🏭 Smart Factory Client Launcher"
echo "================================"
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 9.0 SDK"
    echo "📥 Download from: https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✅ .NET SDK found"
echo ""

# Restore packages if needed
if [ ! -d "bin" ]; then
    echo "📦 Restoring packages..."
    dotnet restore
    if [ $? -ne 0 ]; then
        echo "❌ Failed to restore packages"
        exit 1
    fi
fi

# Build the project
echo "🔨 Building project..."
dotnet build -c Release
if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

echo "✅ Build successful"
echo ""
echo "🚀 Starting Smart Factory Client..."
echo ""

# Run the application
dotnet run -c Release

echo ""
echo "👋 Application ended"
read -p "Press Enter to continue..."