#!/bin/bash

# Script to find the Harmony DLL in common locations
echo "Searching for Harmony DLL..."

# Check Steam workshop directory
STEAM_DIR="$HOME/.steam/steam/steamapps/workshop/content/294100/2009463077"
if [ -d "$STEAM_DIR" ]; then
    HARMONY_DLL=$(find "$STEAM_DIR" -name "0Harmony.dll" | head -n 1)
    if [ -n "$HARMONY_DLL" ]; then
        echo "Found Harmony DLL at: $HARMONY_DLL"
        echo "Copying to References/Harmony directory..."
        cp "$HARMONY_DLL" ./References/Harmony/
        echo "Harmony DLL copied successfully."
        exit 0
    fi
fi

# Check common mod directories
MOD_DIRS=(
    "$HOME/.steam/steam/steamapps/common/RimWorld/Mods"
    "$HOME/.steam/steam/steamapps/workshop/content/294100"
    "./RimWorld/Mods"
)

for DIR in "${MOD_DIRS[@]}"; do
    if [ -d "$DIR" ]; then
        echo "Searching in $DIR..."
        HARMONY_DLL=$(find "$DIR" -name "0Harmony.dll" | head -n 1)
        if [ -n "$HARMONY_DLL" ]; then
            echo "Found Harmony DLL at: $HARMONY_DLL"
            echo "Copying to References/Harmony directory..."
            cp "$HARMONY_DLL" ./References/Harmony/
            echo "Harmony DLL copied successfully."
            exit 0
        fi
    fi
done

echo "Harmony DLL not found automatically."
echo "Please manually copy 0Harmony.dll to ./References/Harmony/ directory."
echo "You can usually find it in the Harmony mod folder in your RimWorld workshop directory."
exit 1
