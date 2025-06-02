#!/bin/bash

# Compile the SimpleCompile.cs file
echo "Compiling CtrlClickCancel mod..."

# Create output directories if they don't exist
mkdir -p ../Assemblies
mkdir -p ../1.5/Assemblies

# Compile using dotnet
dotnet build -c Release

# Check if compilation was successful
if [ $? -eq 0 ]; then
    echo "Compilation successful!"
    
    # Copy the DLL to the appropriate directories
    cp ../Assemblies/CtrlClickCancel.dll ../1.5/Assemblies/
    
    echo "DLL copied to 1.5/Assemblies directory."
    echo "Mod is ready to use!"
else
    echo "Compilation failed!"
fi
