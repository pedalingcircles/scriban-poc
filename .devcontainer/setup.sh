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
setup_git_config() {
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
}

setup_shell_aliases() {
    local shell_config=$1
    local shell_name=$2
    
    echo "  🔧 Setting up aliases for $shell_name..."
    
    cat >> "$shell_config" << 'EOF'
# ==============================
# Development Aliases
# ==============================

# Navigation aliases
alias ll='ls -alF'
alias la='ls -A'
alias l='ls -CF'
alias ..='cd ..'
alias ...='cd ../..'
alias ....='cd ../../..'

# .NET aliases
alias drun='dotnet run'
alias dbuild='dotnet build'
alias dtest='dotnet test'
alias dclean='dotnet clean'
alias drestore='dotnet restore'
alias dwatch='dotnet watch run'
alias dpublish='dotnet publish'

# Azure aliases
alias azlogin='az login'
alias azaccount='az account list'
alias azaks='az aks'
alias azrg='az group'
alias azvm='az vm'

# Terraform aliases
alias tf='terraform'
alias tfi='terraform init'
alias tfp='terraform plan'
alias tfa='terraform apply'
alias tfd='terraform destroy'
alias tfv='terraform validate'
alias tff='terraform fmt'
alias tfw='terraform workspace'
alias tfs='terraform state'

# Git aliases
alias gs='git status'
alias ga='git add'
alias gaa='git add .'
alias gc='git commit'
alias gcm='git commit -m'
alias gca='git commit --amend'
alias gp='git push'
alias gpl='git pull'
alias gl='git log --oneline'
alias gb='git branch'
alias gba='git branch -a'
alias gco='git checkout'
alias gcb='git checkout -b'
alias gm='git merge'
alias gr='git rebase'
alias gd='git diff'
alias gdc='git diff --cached'
alias gst='git stash'
alias gstp='git stash pop'

# Azure Emulator aliases
alias azurite-status='echo "Azurite Status:" && (nc -z azurite 10000 2>/dev/null && echo "  Blob (10000): ✅" || echo "  Blob (10000): ❌") && (nc -z azurite 10001 2>/dev/null && echo "  Queue (10001): ✅" || echo "  Queue (10001): ❌")'
alias emulator-status='~/emulator-status'

# Utility aliases
alias grep='grep --color=auto'
alias fgrep='fgrep --color=auto'
alias egrep='egrep --color=auto'
alias cls='clear'
alias h='history'
alias j='jobs'
alias path='echo -e ${PATH//:/\\n}'
alias now='date +"%T"'
alias nowdate='date +"%d-%m-%Y"'

# Safety aliases
alias rm='rm -i'
alias cp='cp -i'
alias mv='mv -i'

# Quick navigation to common directories
alias cdw='cd /workspaces'
alias cdc='cd /workspaces/${COMPOSE_PROJECT_NAME}'
alias cdi='cd /workspaces/${COMPOSE_PROJECT_NAME}/infra'
alias cds='cd /workspaces/${COMPOSE_PROJECT_NAME}/src'
alias cdt='cd /workspaces/${COMPOSE_PROJECT_NAME}/tests'

EOF
}

# Set up git configuration
setup_git_config

# Set up aliases for both bash and zsh
echo "🔧 Setting up shell aliases..."
# bash
if [ -f ~/.bashrc ]; then
    setup_shell_aliases ~/.bashrc "bash"
else
    echo "  ⚠️ ~/.bashrc not found, skipping bash aliases"
fi
# zsh
if [ -f ~/.zshrc ]; then
    setup_shell_aliases ~/.zshrc "zsh"
elif command -v zsh >/dev/null 2>&1; then
    echo "  📝 Creating ~/.zshrc for zsh aliases..."
    touch ~/.zshrc
    setup_shell_aliases ~/.zshrc "zsh"
else
    echo "  ℹ️ zsh not installed, skipping zsh aliases"
fi

# Verify installations
echo "✅ Verifying installations..."
echo "📍 .NET SDK: $(dotnet --version)"
echo "📍 Terraform: $(terraform --version | head -1)"
echo "📍 Azure CLI: $(az --version | grep azure-cli | awk '{print $2}')"

# Show current shell info
echo "📍 Current shell: $SHELL"
echo "📍 Available shells: $(cat /etc/shells | grep -E '(bash|zsh|pwsh)' | tr '\n' ' ')"
