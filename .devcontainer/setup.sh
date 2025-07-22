#!/bin/bash
# Setup script for development environment

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

# Azure Emulator aliases
alias sb-status='curl -s http://localhost:15672/api/overview | jq .'
alias eh-status='curl -s http://localhost:8080/v1/metadata'
alias azurite-status='curl -s http://localhost:10000/'
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

# Create Azure emulator status check script
echo "☁️ Creating Azure emulator status script..."
cat > ~/emulator-status << 'EOF'
#!/bin/bash
echo "=== Azure Emulator Status ==="
echo

echo "🔍 Service Bus Emulator:"
if curl -s http://localhost:15672/api/overview >/dev/null 2>&1; then
    echo "  ✅ Running - Management UI: http://localhost:15672"
    echo "  📊 Connection String: Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
else
    echo "  ❌ Not running"
fi
echo

echo "🔍 Event Hub Emulator:"
if curl -s http://localhost:8080/v1/metadata >/dev/null 2>&1; then
    echo "  ✅ Running - Kafka endpoint: localhost:9093"
    echo "  📊 Connection String: Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
else
    echo "  ❌ Not running"
fi
echo

echo "🔍 Azurite (Storage Emulator):"
if curl -s http://localhost:10000/ >/dev/null 2>&1; then
    echo "  ✅ Running"
    echo "  📊 Blob: http://localhost:10000"
    echo "  📊 Queue: http://localhost:10001"
    echo "  📊 Table: http://localhost:10002"
    echo "  📊 Connection String: DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://localhost:10000/devstoreaccount1;QueueEndpoint=http://localhost:10001/devstoreaccount1;TableEndpoint=http://localhost:10002/devstoreaccount1;"
else
    echo "  ❌ Not running"
fi
echo

echo "💡 Use 'sb-status', 'eh-status', or 'azurite-status' for individual service checks"
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
    "ServiceBus": "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;",
    "EventHub": "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;",
    "Storage": "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://localhost:10000/devstoreaccount1;QueueEndpoint=http://localhost:10001/devstoreaccount1;TableEndpoint=http://localhost:10002/devstoreaccount1;"
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

echo "🎉 Development environment setup complete!"
echo "💡 Run '~/netdiag' to test networking utilities"
echo "💡 Run '~/emulator-status' to check Azure emulator status"
echo "💡 Connection strings available in '~/azure-emulator-connections.md'"
echo "💡 Available networking tools: ping, traceroute, netstat, ifconfig, ip, nslookup, tcpdump"
