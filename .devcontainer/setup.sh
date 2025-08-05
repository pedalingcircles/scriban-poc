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

# Function to add aliases to a shell config file
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

# Docker aliases
alias dk='docker'
alias dkc='docker-compose'
alias dkps='docker ps'
alias dki='docker images'
alias dkrm='docker rm'
alias dkrmi='docker rmi'

# Azure Emulator aliases
alias sb-status='echo "Service Bus Status:" && (nc -z servicebus-emulator 5672 2>/dev/null && echo "  AMQP (5672): ✅" || echo "  AMQP (5672): ❌") && (nc -z servicebus-emulator 5300 2>/dev/null && echo "  Management (5300): ✅" || echo "  Management (5300): ❌")'
alias eh-status='echo "Event Hub Status:" && (nc -z eventhub-emulator 9092 2>/dev/null && echo "  Kafka (9092): ✅" || echo "  Kafka (9092): ❌") && (nc -z eventhub-emulator 5672 2>/dev/null && echo "  AMQP (5672): ✅" || echo "  AMQP (5672): ❌")'
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
alias cdc='cd /workspaces/scriban-poc'
alias cdi='cd /workspaces/scriban-poc/infra'
alias cds='cd /workspaces/scriban-poc/src'
alias cdt='cd /workspaces/scriban-poc/tests'

EOF
}

# Set up aliases for both bash and zsh
echo "🔧 Setting up shell aliases..."

# Setup for bash
if [ -f ~/.bashrc ]; then
    setup_shell_aliases ~/.bashrc "bash"
else
    echo "  ⚠️ ~/.bashrc not found, skipping bash aliases"
fi

# Setup for zsh
if [ -f ~/.zshrc ]; then
    setup_shell_aliases ~/.zshrc "zsh"
elif command -v zsh >/dev/null 2>&1; then
    echo "  📝 Creating ~/.zshrc for zsh aliases..."
    touch ~/.zshrc
    setup_shell_aliases ~/.zshrc "zsh"
else
    echo "  ℹ️ zsh not installed, skipping zsh aliases"
fi

# Configure zsh-specific settings if zsh is available
if command -v zsh >/dev/null 2>&1; then
    echo "🐚 Configuring zsh..."
    
    # Add zsh-specific configurations
    cat >> ~/.zshrc << 'EOF'

# ==============================
# Zsh-specific configurations
# ==============================

# Enable auto-completion
autoload -Uz compinit && compinit

# History settings
HISTSIZE=10000
SAVEHIST=10000
HISTFILE=~/.zsh_history
setopt HIST_VERIFY
setopt SHARE_HISTORY
setopt APPEND_HISTORY
setopt INC_APPEND_HISTORY
setopt HIST_IGNORE_DUPS
setopt HIST_IGNORE_ALL_DUPS
setopt HIST_REDUCE_BLANKS
setopt HIST_IGNORE_SPACE

# Directory navigation
setopt AUTO_CD
setopt AUTO_PUSHD
setopt PUSHD_IGNORE_DUPS

# Completion settings
setopt COMPLETE_ALIASES
setopt GLOB_COMPLETE
setopt MENU_COMPLETE

# Color support
if [ -x /usr/bin/dircolors ]; then
    test -r ~/.dircolors && eval "$(dircolors -b ~/.dircolors)" || eval "$(dircolors -b)"
fi

EOF

    # Set zsh as default shell if not already set
    if [ "$SHELL" != "/bin/zsh" ] && [ "$SHELL" != "/usr/bin/zsh" ]; then
        echo "  ⚙️ Setting zsh as default shell for user"
        sudo chsh -s "$(which zsh)" vscode 2>/dev/null || echo "  ⚠️ Could not change default shell (may require container rebuild)"
    fi
fi

# Create PowerShell profile if PowerShell is available
if command -v pwsh >/dev/null 2>&1; then
    echo "💙 Setting up PowerShell aliases..."
    
    # Create PowerShell profile directory if it doesn't exist
    mkdir -p ~/.config/powershell
    
    cat > ~/.config/powershell/Microsoft.PowerShell_profile.ps1 << 'EOF'
# PowerShell Development Aliases

# Navigation
Set-Alias -Name ll -Value Get-ChildItem
function la { Get-ChildItem -Force }
function .. { Set-Location .. }
function ... { Set-Location ../.. }

# .NET aliases
function drun { dotnet run @args }
function dbuild { dotnet build @args }
function dtest { dotnet test @args }
function dclean { dotnet clean @args }
function drestore { dotnet restore @args }

# Git aliases
function gs { git status @args }
function ga { git add @args }
function gc { git commit @args }
function gp { git push @args }
function gl { git pull @args }

# Azure aliases
function azlogin { az login @args }
function azaccount { az account list @args }

# Terraform aliases
function tf { terraform @args }
function tfi { terraform init @args }
function tfp { terraform plan @args }
function tfa { terraform apply @args }

# Utility functions
function cls { Clear-Host }
function grep { Select-String @args }

Write-Host "🚀 PowerShell development environment loaded!" -ForegroundColor Green
EOF
    
    echo "  ✅ PowerShell profile created"
fi

# Create PowerShell profile if PowerShell is available
if command -v pwsh >/dev/null 2>&1; then
    echo "💙 Setting up PowerShell aliases and ALZ module..."
    
    # Create PowerShell profile directory if it doesn't exist
    mkdir -p ~/.config/powershell
    
    cat > ~/.config/powershell/Microsoft.PowerShell_profile.ps1 << 'EOF'
# PowerShell Development Aliases

# Navigation
Set-Alias -Name ll -Value Get-ChildItem
function la { Get-ChildItem -Force }
function .. { Set-Location .. }
function ... { Set-Location ../.. }

# .NET aliases
function drun { dotnet run @args }
function dbuild { dotnet build @args }
function dtest { dotnet test @args }
function dclean { dotnet clean @args }
function drestore { dotnet restore @args }

# Git aliases
function gs { git status @args }
function ga { git add @args }
function gc { git commit @args }
function gp { git push @args }
function gl { git pull @args }

# Azure aliases
function azlogin { az login @args }
function azaccount { az account list @args }

# Terraform aliases
function tf { terraform @args }
function tfi { terraform init @args }
function tfp { terraform plan @args }
function tfa { terraform apply @args }

# Utility functions
function cls { Clear-Host }
function grep { Select-String @args }

# Check if ALZ module is installed
if (Get-InstalledModule -Name ALZ -ErrorAction SilentlyContinue) {
    $alzVersion = (Get-InstalledModule -Name ALZ).Version
    Write-Host "✅ ALZ Module installed - Version: $alzVersion" -ForegroundColor Green
}

Write-Host "🚀 PowerShell development environment loaded!" -ForegroundColor Green
EOF
    
    echo "  ✅ PowerShell profile created"
    
    # Install ALZ PowerShell module
    echo "📦 Installing ALZ PowerShell module..."
    pwsh -c "
        try {
            Set-PSRepository -Name PSGallery -InstallationPolicy Trusted
            Install-Module -Name ALZ -RequiredVersion 4.2.12 -Force -Scope CurrentUser
            \$installedModule = Get-InstalledModule -Name ALZ -ErrorAction SilentlyContinue
            if (\$installedModule) {
                Write-Host \"✅ ALZ module installed - Version: \$(\$installedModule.Version)\" -ForegroundColor Green
            } else {
                Write-Host \"❌ Failed to install ALZ module\" -ForegroundColor Red
            }
        } catch {
            Write-Host \"❌ Error installing ALZ module: \$_\" -ForegroundColor Red
        }
    "
fi

# Set up networking test script
echo "🌐 Creating network diagnostic script..."
cat > ~/netdiag << 'EOF'
#!/bin/bash
echo "=== Network Diagnostics ==="
echo "Hostname: $(hostname)"
echo "IP Configuration:"
ip addr show | grep -E '^[0-9]+:|inet '
echo
echo "Route Table:"
ip route
echo
echo "DNS Configuration:"
cat /etc/resolv.conf
echo
echo "Testing connectivity:"
ping -c 3 8.8.8.8
EOF
chmod +x ~/netdiag

# Create Azure emulator status check script
echo "☁️ Creating Azure emulator status script..."
cat > ~/emulator-status << 'EOF'
#!/bin/bash
echo "=== Azure Emulator Status ==="
echo

echo "🔍 SQL Server (Service Bus Dependency):"
if nc -z sqlserver 1433 2>/dev/null; then
    echo "  ✅ Running - Port 1433 accessible"
else
    echo "  ❌ Not accessible on port 1433"
fi
echo

echo "🔍 Service Bus Emulator:"
if nc -z servicebus-emulator 5672 2>/dev/null; then
    echo "  ✅ Running - AMQP port accessible"
    echo "  📊 AMQP Endpoint: servicebus-emulator:5672"
    echo "  📊 Connection String: Endpoint=sb://servicebus-emulator;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
    
    if nc -z servicebus-emulator 5300 2>/dev/null; then
        echo "  📊 Management UI: http://localhost:5300 (host) / http://servicebus-emulator:5300 (container)"
    fi
else
    echo "  ❌ AMQP port (5672) not accessible"
    echo "  💡 Service Bus emulator may still be starting up or have SQL Server connectivity issues"
fi
echo

echo "🔍 Event Hub Emulator:"
if nc -z eventhub-emulator 9092 2>/dev/null; then
    echo "  ✅ Running - Kafka port accessible"
    echo "  📊 Kafka Endpoint: eventhub-emulator:9092"
    
    if nc -z eventhub-emulator 5672 2>/dev/null; then
        echo "  📊 AMQP Endpoint: eventhub-emulator:5672 (port 5673 on host)"
    fi
    
    if nc -z eventhub-emulator 5300 2>/dev/null; then
        echo "  📊 Management: eventhub-emulator:5300 (port 5301 on host)"
    fi
    
    echo "  📊 Connection String: Endpoint=sb://eventhub-emulator;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
else
    echo "  ❌ Kafka port (9092) not accessible"
    echo "  💡 Event Hub emulator may still be starting up or have Azurite dependency issues"
fi
echo

echo "🔍 Azurite (Storage Emulator):"
if nc -z azurite 10000 2>/dev/null; then
    echo "  ✅ Running - Blob service accessible"
    echo "  📊 Blob: http://localhost:10000 (host) / http://azurite:10000 (container)"
    echo "  📊 Queue: http://localhost:10001 (host) / http://azurite:10001 (container)"
    echo "  📊 Table: http://localhost:10002 (host) / http://azurite:10002 (container)"
    echo "  📊 Container Connection String: DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://azurite:10000/devstoreaccount1;QueueEndpoint=http://azurite:10001/devstoreaccount1;TableEndpoint=http://azurite:10002/devstoreaccount1;"
else
    echo "  ❌ Blob service port (10000) not accessible"
fi
echo

echo "💡 Use 'sb-status', 'eh-status', or 'azurite-status' for individual service checks"
echo "💡 From host machine use localhost:PORT, from container use SERVICE-NAME:PORT"
echo "💡 Service Bus depends on SQL Server, Event Hub depends on Azurite"
EOF
chmod +x ~/emulator-status

# Create connection strings file for easy reference
echo "📝 Creating connection strings reference..."
cat > ~/azure-emulator-connections.md << 'EOF'
# Azure Emulator Connection Strings

## Service Bus Emulator
```
Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;
```

## Event Hub Emulator
```
Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;
```

## Azurite (Storage Emulator)
```
DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://localhost:10000/devstoreaccount1;QueueEndpoint=http://localhost:10001/devstoreaccount1;TableEndpoint=http://localhost:10002/devstoreaccount1;
```

## Service Endpoints
- Service Bus Management UI: http://localhost:15672
- Event Hub Management: http://localhost:8080
- Azurite Blob: http://localhost:10000
- Azurite Queue: http://localhost:10001
- Azurite Table: http://localhost:10002

## Sample .NET Configuration

### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "ServiceBus": "Endpoint=sb://servicebus-emulator;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;",
    "EventHub": "Endpoint=sb://eventhub-emulator;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;",
    "Storage": "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://azurite:10000/devstoreaccount1;QueueEndpoint=http://azurite:10001/devstoreaccount1;TableEndpoint=http://azurite:10002/devstoreaccount1;"
  }
}
```
EOF

# Wait for emulators to be ready
echo "⏳ Waiting for Azure emulators to start..."
sleep 20

# Check emulator status
~/emulator-status

# Verify installations
echo "✅ Verifying installations..."
echo "📍 .NET SDK: $(dotnet --version)"
echo "📍 Terraform: $(terraform --version | head -1)"
echo "📍 Azure CLI: $(az --version | grep azure-cli | awk '{print $2}')"

# Show current shell info
echo "📍 Current shell: $SHELL"
echo "📍 Available shells: $(cat /etc/shells | grep -E '(bash|zsh|pwsh)' | tr '\n' ' ')"

# Check ALZ module
if command -v pwsh >/dev/null 2>&1; then
    echo "🏗️ Checking ALZ PowerShell module..."
    pwsh -c "
        \$alz = Get-InstalledModule -Name ALZ -ErrorAction SilentlyContinue
        if (\$alz) {
            Write-Host \"✅ ALZ Module: \$(\$alz.Version)\" -ForegroundColor Green
        } else {
            Write-Host \"❌ ALZ Module: Not installed\" -ForegroundColor Red
        }
    "
fi

echo ""
echo "🎉 Development environment setup complete!"
echo "💡 Run '~/netdiag' to test networking utilities"
echo "💡 Run '~/emulator-status' to check Azure emulator status"
echo "💡 Connection strings available in '~/azure-emulator-connections.md'"
echo "💡 Available networking tools: ping, traceroute, netstat, ifconfig, ip, nslookup, tcpdump"
echo ""
echo "🐚 Shell aliases configured for:"
echo "   - Bash (if ~/.bashrc exists)"
echo "   - Zsh (if zsh is installed)"
echo "   - PowerShell (if pwsh is available)"
echo ""
echo "💡 To reload aliases in current session:"
echo "   Bash: source ~/.bashrc"
echo "   Zsh:  source ~/.zshrc"
echo "   PowerShell: . \$PROFILE"