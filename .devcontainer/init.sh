#!/bin/bash
# This script runs before the dev container starts (initializeCommand) to check 
# environment variables and configuration files.
# see: https://containers.dev/implementors/json_reference/#lifecycle-scripts

set -e  # Exit on any error

ENV_FILE=".devcontainer/.env"
ENV_EXAMPLE=".devcontainer/.env.example"

echo "🔍 Checking environment configuration..."
# Check if .env file exists
if [ ! -f "$ENV_FILE" ]; then
    echo ""
    echo "❌ ERROR: .env file not found!"
    echo "📁 Expected location: $ENV_FILE"
    echo ""
    echo "🔧 To fix this:"
    echo "   1. Copy the example file: cp $ENV_EXAMPLE $ENV_FILE"
    echo "   2. Edit the .env file with your settings"
    echo "   3. See README.md for detailed setup instructions"
    echo ""
    echo "⚠️  Dev container startup will continue, but services may not work properly"
    exit 1
fi

echo "✅ .env file found"

# Source the environment variables
echo "🔄 Loading environment variables..."
set -a
source "$ENV_FILE"
set +a
echo "📋 Environment variables loaded and exported"