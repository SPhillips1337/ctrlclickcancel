#!/bin/bash

# This script helps compile the CtrlClickCancel mod using Mono or .NET SDK
# It will attempt to detect which tools are available and use the appropriate one

echo "CtrlClickCancel Mod Compilation Script"
echo "====================================="

# Check for .NET SDK
if command -v dotnet &> /dev/null; then
    echo "Found .NET SDK, attempting to compile with dotnet..."
    dotnet build CtrlClickCancel.csproj
    if [ $? -eq 0 ]; then
        echo "Compilation successful!"
        exit 0
    else
        echo "Compilation with dotnet failed."
    fi
fi

# Check for MSBuild
if command -v msbuild &> /dev/null; then
    echo "Found MSBuild, attempting to compile with msbuild..."
    msbuild CtrlClickCancel.csproj
    if [ $? -eq 0 ]; then
        echo "Compilation successful!"
        exit 0
    else
        echo "Compilation with msbuild failed."
    fi
fi

# Check for Mono
if command -v mono &> /dev/null; then
    echo "Found Mono, attempting to compile with mono..."
    mono --version
    
    # Check for xbuild (older Mono build tool)
    if command -v xbuild &> /dev/null; then
        echo "Using xbuild..."
        xbuild CtrlClickCancel.csproj
        if [ $? -eq 0 ]; then
            echo "Compilation successful!"
            exit 0
        else
            echo "Compilation with xbuild failed."
        fi
    else
        echo "xbuild not found."
    fi
fi

echo ""
echo "====================================="
echo "Compilation failed. No suitable build tools found."
echo "Please install one of the following:"
echo "- .NET SDK (dotnet)"
echo "- Mono (with msbuild or xbuild)"
echo "- Visual Studio (on Windows)"
echo ""
echo "See compile_instructions.txt for more details."
exit 1
