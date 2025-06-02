# Compilation Instructions for CtrlClickCancel Mod

This guide will help you set up a compilation environment for the CtrlClickCancel mod on Linux Mint 22.1 (based on Ubuntu 24 Noble).

## Prerequisites

- Linux Mint 22.1 or Ubuntu 24.04
- Access to your RimWorld installation files
- Access to the Harmony mod files

## Setup Steps

1. **Run the setup script**:
   ```bash
   ./setup_compilation.sh
   ```
   This will install the necessary tools and create the directory structure.

2. **Copy RimWorld DLLs**:
   
   You need to copy the following files from your RimWorld installation:
   
   From `RimWorldLinux_Data/Managed/` directory:
   - `Assembly-CSharp.dll`
   - `UnityEngine.dll`
   - `UnityEngine.CoreModule.dll`
   - `UnityEngine.IMGUIModule.dll`
   - `UnityEngine.TextRenderingModule.dll`
   
   Copy these files to `./References/RimWorld/` in your mod directory.

3. **Copy Harmony DLL**:
   
   Copy `0Harmony.dll` from the Harmony mod to `./References/Harmony/` in your mod directory.
   
   This file is usually located at:
   ```
   ~/.steam/steam/steamapps/workshop/content/294100/2009463077/Current/Assemblies/0Harmony.dll
   ```

4. **Compile the mod**:
   ```bash
   ./compile_mod.sh
   ```
   This will compile the mod and place the DLL in both the `Assemblies` and `1.5/Assemblies` directories.

## Troubleshooting

- If you get errors about missing references, make sure you've copied all the required DLLs to the correct locations.
- If you get compilation errors, check the error messages for details on what went wrong.
- Make sure the paths in the scripts match your actual RimWorld installation paths.

## Manual Compilation

If you prefer to compile manually, you can use the Mono C# compiler directly:

```bash
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
```
