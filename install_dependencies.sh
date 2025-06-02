#!/bin/bash

# Install required dependencies for RimWorld mod compilation
echo "Installing required dependencies for RimWorld mod compilation..."

# Update package lists
sudo apt-get update

# Install Mono development tools
sudo apt-get install -y mono-complete

echo "Dependencies installed successfully."
echo "You can now run ./compile_mod.sh to compile the mod."
