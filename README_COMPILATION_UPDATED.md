# Compilation Instructions for CtrlClickCancel Mod

This guide will help you compile the CtrlClickCancel mod on Linux Mint 22.1.

## Setup Overview

I've already set up most of the compilation environment for you:

1. ✅ **RimWorld DLLs**: Successfully copied from your RimWorld directory to `./References/RimWorld/`
2. ❓ **Harmony DLL**: Created a placeholder, but you need the actual Harmony DLL
3. ❓ **Mono Compiler**: You need to install the Mono compiler

## Steps to Complete Setup

### 1. Install Dependencies

Run the installation script to install the Mono compiler:

```bash
./install_dependencies.sh
```

This will install the Mono development tools needed for compilation.

### 2. Find and Copy Harmony DLL

Run the script to automatically find and copy the Harmony DLL:

```bash
./find_harmony.sh
```

If the script can't find the Harmony DLL automatically, you'll need to manually copy it:

1. Find the Harmony mod in your Steam workshop directory:
   ```
   ~/.steam/steam/steamapps/workshop/content/294100/2009463077/Current/Assemblies/0Harmony.dll
   ```

2. Copy it to the References directory:
   ```bash
   cp [path_to_harmony_dll] ./References/Harmony/0Harmony.dll
   ```

### 3. Compile the Mod

Once you have all the dependencies, run the compilation script:

```bash
./compile_mod.sh
```

This will compile the mod and place the DLL in both the `Assemblies` and `1.5/Assemblies` directories.

## Troubleshooting

- If you get errors about missing Harmony, make sure you've found and copied the correct 0Harmony.dll file.
- If you get compilation errors, check the error messages for details on what went wrong.
- If you need to modify the compilation process, edit the `compile_mod.sh` script.

## Manual Compilation

If you prefer to compile manually after installing Mono:

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
