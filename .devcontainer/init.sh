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

# Required variables to check
REQUIRED_VARS=(
    "WORKSPACE_ROOT"
    "COMPOSE_PROJECT_NAME"
    "CONTAINER_PREFIX"
    "GIT_USER_NAME"
    "GIT_USER_EMAIL"
    "GIT_DEFAULT_BRANCH"
    "GIT_EDITOR"
    "SQL_WAIT_INTERVAL"
    "MSSQL_SA_PASSWORD"
    "ACCEPT_EULA"
    "AZURITE_ACCOUNT_KEY"
    "SERVICEBUS_SHARED_ACCESS_KEY"
    "EVENTHUB_SHARED_ACCESS_KEY"
)

# Check if all required variables are set and not empty
MISSING_VARS=()
for var in "${REQUIRED_VARS[@]}"; do
    if [ -z "${!var}" ]; then
        MISSING_VARS+=("$var")
    fi
done

if [ ${#MISSING_VARS[@]} -ne 0 ]; then
    echo ""
    echo "❌ ERROR: Missing or empty environment variables:"
    for var in "${MISSING_VARS[@]}"; do
        echo "   - $var"
    done
    echo ""
    echo "🔧 Please update your .env file with the missing variables"
    echo "📖 See .env.example for reference values"
    echo "📚 Check README.md for detailed setup instructions"
    echo ""
    exit 1
fi

echo "✅ Environment configuration check complete!"
echo "🚀 Proceeding with dev container startup..."
