#!/bin/bash

# Setup script for RimWorld mod compilation environment on Linux Mint 22.1 (Ubuntu Noble)
echo "Setting up RimWorld mod compilation environment..."

# Install .NET SDK if not already installed
if ! command -v dotnet &> /dev/null; then
    echo "Installing .NET SDK..."
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-6.0
fi

# Create directory structure for references
mkdir -p ./References/RimWorld
mkdir -p ./References/Harmony

echo "Directory structure created."
echo ""
echo "NEXT STEPS:"
echo "1. Copy the following files from your RimWorld installation to ./References/RimWorld/:"
echo "   - Assembly-CSharp.dll (from RimWorldLinux_Data/Managed/)"
echo "   - UnityEngine.dll (from RimWorldLinux_Data/Managed/)"
echo "   - UnityEngine.CoreModule.dll (from RimWorldLinux_Data/Managed/)"
echo "   - UnityEngine.IMGUIModule.dll (from RimWorldLinux_Data/Managed/)"
echo "   - UnityEngine.TextRenderingModule.dll (from RimWorldLinux_Data/Managed/)"
echo ""
echo "2. Copy 0Harmony.dll from the Harmony mod to ./References/Harmony/"
echo "   (Usually found in Steam/steamapps/workshop/content/294100/2009463077/Current/Assemblies/)"
echo ""
echo "3. Run the compile script after copying the references:"
echo "   ./compile_mod.sh"
echo ""
echo "Setup complete!"
