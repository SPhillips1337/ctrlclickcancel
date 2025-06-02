#!/bin/bash

# Compilation script for RimWorld mod using dotnet
echo "Compiling CtrlClickCancel mod using dotnet..."

# Create a temporary project directory
TEMP_DIR="./temp_project"
mkdir -p "$TEMP_DIR"

# Create a simple project file
cat > "$TEMP_DIR/CtrlClickCancel.csproj" << EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <LangVersion>7.3</LangVersion>
    <AssemblyName>CtrlClickCancel</AssemblyName>
    <RootNamespace>CtrlClickCancel</RootNamespace>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <OutputPath>../Assemblies</OutputPath>
  </PropertyGroup>
  
  <ItemGroup>
    <Reference Include="Assembly-CSharp">
      <HintPath>../References/RimWorld/Assembly-CSharp.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="UnityEngine">
      <HintPath>../References/RimWorld/UnityEngine.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>../References/RimWorld/UnityEngine.CoreModule.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="UnityEngine.IMGUIModule">
      <HintPath>../References/RimWorld/UnityEngine.IMGUIModule.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="UnityEngine.TextRenderingModule">
      <HintPath>../References/RimWorld/UnityEngine.TextRenderingModule.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="0Harmony">
      <HintPath>../References/Harmony/0Harmony.dll</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>
  
  <ItemGroup>
    <Compile Include="../Source/CtrlClickCancel.cs" />
    <Compile Include="../Source/Properties/AssemblyInfo.cs" />
  </ItemGroup>
</Project>
EOF

# Create output directories
mkdir -p ./Assemblies
mkdir -p ./1.5/Assemblies

# Compile the mod using dotnet
echo "Compiling with dotnet..."
cd "$TEMP_DIR"
dotnet build -c Release

# Check if compilation was successful
if [ $? -eq 0 ]; then
    echo "Compilation successful!"
    
    # Copy the DLL to the 1.5 directory
    cd ..
    cp ./Assemblies/CtrlClickCancel.dll ./1.5/Assemblies/
    
    echo "DLL copied to 1.5/Assemblies directory."
    echo "Mod is ready to use!"
    
    # Clean up
    rm -rf "$TEMP_DIR"
else
    echo "Compilation failed!"
    cd ..
fi
