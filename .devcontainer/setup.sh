#!/bin/bash
# filepath: .devcontainer/setup.sh

echo "🚀 Setting up development environment..."

# Update package lists
sudo apt-get update

# Verify dotnet installation and restore solution
echo "📦 Restoring .NET packages..."
dotnet restore

# Set up git (if not already configured)
if [ -z "$(git config --global user.name)" ]; then
    echo "⚠️  Please configure git:"
    echo "git config --global user.name 'Your Name'"
    echo "git config --global user.email 'your.email@example.com'"
fi

# Create useful aliases
echo "🔧 Setting up bash aliases..."
cat >> ~/.bashrc << 'EOF'

# Custom aliases for development
alias ll='ls -alF'
alias la='ls -A'
alias l='ls -CF'
alias ..='cd ..'
alias ...='cd ../..'

# .NET aliases
alias drun='dotnet run'
alias dbuild='dotnet build'
alias dtest='dotnet test'
alias dclean='dotnet clean'
alias drestore='dotnet restore'

# Azure aliases
alias azlogin='az login'
alias azaccount='az account list'
alias azaks='az aks'

# Terraform aliases
alias tf='terraform'
alias tfi='terraform init'
alias tfp='terraform plan'
alias tfa='terraform apply'
alias tfd='terraform destroy'
alias tfv='terraform validate'
alias tff='terraform fmt'

# Git aliases
alias gs='git status'
alias ga='git add'
alias gc='git commit'
alias gp='git push'
alias gl='git pull'
alias gb='git branch'
alias gco='git checkout'
EOF

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

# Verify installations
echo "✅ Verifying installations..."
echo "📍 .NET SDK: $(dotnet --version)"
echo "📍 Terraform: $(terraform --version | head -1)"
echo "📍 Azure CLI: $(az --version | grep azure-cli | awk '{print $2}')"

echo "🎉 Development environment setup complete!"
echo "💡 Run '~/netdiag' to test networking utilities"
echo "💡 Available networking tools: ping, traceroute, netstat, ifconfig, ip, nslookup, tcpdump"
