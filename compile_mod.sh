#!/bin/bash

# Compilation script for RimWorld mod
echo "Compiling CtrlClickCancel mod..."

# Check if reference files exist
if [ ! -f "./References/RimWorld/Assembly-CSharp.dll" ]; then
    echo "Error: RimWorld references not found. Please copy the required DLLs first."
    echo "See setup_compilation.sh for instructions."
    exit 1
fi

if [ ! -f "./References/Harmony/0Harmony.dll" ]; then
    echo "Error: Harmony DLL not found. Please copy 0Harmony.dll first."
    echo "See setup_compilation.sh for instructions."
    exit 1
fi

# Create output directories
mkdir -p ./Assemblies
mkdir -p ./1.5/Assemblies

# Compile the mod using Mono C# compiler
echo "Compiling with Mono C# compiler..."

mcs -target:library \
    -out:./Assemblies/CtrlClickCancel.dll \
    -r:./References/RimWorld/Assembly-CSharp.dll \
    -r:./References/RimWorld/UnityEngine.dll \
    -r:./References/RimWorld/UnityEngine.CoreModule.dll \
    -r:./References/RimWorld/UnityEngine.IMGUIModule.dll \
    -r:./References/RimWorld/UnityEngine.TextRenderingModule.dll \
    -r:./References/Harmony/0Harmony.dll \
    -r:System.dll \
    -r:System.Core.dll \
    -r:System.Xml.dll \
    ./Source/CtrlClickCancel.cs \
    ./Source/Properties/AssemblyInfo.cs

# Check if compilation was successful
if [ $? -eq 0 ]; then
    echo "Compilation successful!"
    
    # Copy the DLL to the 1.5 directory
    cp ./Assemblies/CtrlClickCancel.dll ./1.5/Assemblies/
    
    echo "DLL copied to 1.5/Assemblies directory."
    echo "Mod is ready to use!"
else
    echo "Compilation failed!"
fi
