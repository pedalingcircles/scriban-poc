FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# Install networking utilities and common tools
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
        # Networking utilities
        iputils-ping \
        traceroute \
        net-tools \
        iproute2 \
        dnsutils \
        tcpdump \
        netcat-traditional \
        nmap \
        telnet \
        curl \
        wget \
        # Development tools
        jq \
        unzip \
        zip \
        tree \
        htop \
        vim \
        nano \
        # Build tools
        build-essential \
        # Additional utilities
        ca-certificates \
        gnupg \
        lsb-release \
    && apt-get autoremove -y && apt-get clean -y

# Install PowerShell Core
RUN curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg \
    && echo "deb [arch=amd64,arm64,armhf signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/repos/microsoft-debian-bookworm-prod bookworm main" > /etc/apt/sources.list.d/microsoft-prod.list \
    && apt-get update \
    && apt-get install -y powershell \
    && apt-get autoremove -y && apt-get clean -y

# Install Terraform Language Server
RUN curl -fsSL https://releases.hashicorp.com/terraform-ls/0.32.7/terraform-ls_0.32.7_linux_amd64.zip -o terraform-ls.zip \
    && unzip terraform-ls.zip \
    && mv terraform-ls /usr/local/bin/ \
    && rm terraform-ls.zip

# Install tflint for Terraform linting
RUN curl -s https://raw.githubusercontent.com/terraform-linters/tflint/master/install_linux.sh | bash

# Set up bash completion
RUN echo "source /usr/share/bash-completion/bash_completion" >> /home/vscode/.bashrc \
    && echo "complete -C /usr/local/bin/terraform terraform" >> /home/vscode/.bashrc

# Install dotnet tools globally
RUN dotnet tool install --global dotnet-ef \
    && dotnet tool install --global dotnet-outdated-tool \
    && dotnet tool install --global dotnet-format \
    && dotnet tool install --global GitVersion.Tool

# Ensure tools are in PATH
ENV PATH="${PATH}:/home/vscode/.dotnet/tools"

# Set up PowerShell profile and modules for vscode user
USER vscode
RUN pwsh -Command "Install-Module -Name Az -Force -AllowClobber -Scope CurrentUser" \
    && pwsh -Command "Install-Module -Name Microsoft.PowerShell.PSResourceGet -Force -Scope CurrentUser" \
    && pwsh -Command "Install-Module -Name PSReadLine -Force -Scope CurrentUser"

# Create PowerShell profile with useful aliases
RUN mkdir -p ~/.config/powershell \
    && echo '# PowerShell Profile for Development' > ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-PSReadLineOption -PredictionSource History' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-PSReadLineOption -EditMode Emacs' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo '' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo '# Aliases for .NET development' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-Alias -Name drun -Value "dotnet run"' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-Alias -Name dbuild -Value "dotnet build"' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-Alias -Name dtest -Value "dotnet test"' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-Alias -Name dclean -Value "dotnet clean"' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-Alias -Name drestore -Value "dotnet restore"' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo '' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo '# Kubernetes aliases' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'Set-Alias -Name k -Value kubectl' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo '' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo '# Functions' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'function Get-DotNetInfo { dotnet --info }' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1 \
    && echo 'function Get-KubePods { kubectl get pods }' >> ~/.config/powershell/Microsoft.PowerShell_profile.ps1

USER vscode