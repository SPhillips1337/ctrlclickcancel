#!/bin/bash

echo "Compiling CtrlClickCancel mod using SimpleCompile.cs..."

# Create output directories if they don't exist
mkdir -p ../Assemblies
mkdir -p ../1.5/Assemblies

# Copy the SimpleCompile.cs to CtrlClickCancel.cs
cp SimpleCompile.cs CtrlClickCancel.cs

# Copy the existing DLL to the 1.5 directory
cp ../Assemblies/CtrlClickCancel.dll ../1.5/Assemblies/

echo "Updated code has been saved to CtrlClickCancel.cs"
echo "Existing DLL has been copied to 1.5/Assemblies directory."
echo "To fully apply the changes, you'll need to compile the mod on a system with RimWorld references."
