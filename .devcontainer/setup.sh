#!/bin/bash
# This runs after the containers are up and configures and sets up (postCreateCommand)
# the development environment, including restoring .NET packages, setting up git,
# configuring shell aliases, and installing necessary tools.
# see: https://containers.dev/implementors/json_reference/#lifecycle-scripts

echo "🚀 Setting up development environment..."

# Update package lists
sudo apt-get update

# Verify dotnet installation and restore solution
echo "📦 Restoring .NET packages..."
dotnet restore

# Set up git configuration
echo "🔧 Configuring Git..."
if [ -z "$(git config --global user.name)" ]; then
    if [ -n "$GIT_USER_NAME" ] && [ -n "$GIT_USER_EMAIL" ]; then
        echo "  ✅ Setting Git config from environment variables"
        git config --global user.name "$GIT_USER_NAME"
        git config --global user.email "$GIT_USER_EMAIL"
        
        # Optional additional Git configurations
        if [ -n "$GIT_DEFAULT_BRANCH" ]; then
            git config --global init.defaultBranch "$GIT_DEFAULT_BRANCH"
        fi
        
        if [ -n "$GIT_EDITOR" ]; then
            git config --global core.editor "$GIT_EDITOR"
        fi
        
        # Set some sensible defaults
        git config --global pull.rebase false
        git config --global init.defaultBranch main
        git config --global core.autocrlf input
        
    else
        echo "⚠️  Git not configured. Please create .devcontainer/.env (see README.md)"
    fi
else
    echo "  ✅ Git already configured as: $(git config --global user.name) <$(git config --global user.email)>"
fi

# Verify installations
echo "✅ Verifying installations..."
echo "📍 .NET SDK: $(dotnet --version)"
echo "📍 Terraform: $(terraform --version | head -1)"
echo "📍 Azure CLI: $(az --version | grep azure-cli | awk '{print $2}')"

# Show current shell info
echo "📍 Current shell: $SHELL"
echo "📍 Available shells: $(cat /etc/shells | grep -E '(bash|zsh|pwsh)' | tr '\n' ' ')"
