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

# Install Azure CLI extensions commonly used with AKS
RUN az extension add --name aks-preview \
    && az extension add --name application-insights \
    && az extension add --name containerapp \
    && az extension add --name k8s-extension \
    && az extension add --name connectedk8s

# Install additional Kubernetes tools
RUN curl -fsSL https://get.helm.sh/helm-v3.14.0-linux-amd64.tar.gz | tar -xz -C /tmp \
    && mv /tmp/linux-amd64/helm /usr/local/bin/helm \
    && chmod +x /usr/local/bin/helm

# Install kubectx and kubens for easier context switching
RUN git clone https://github.com/ahmetb/kubectx /opt/kubectx \
    && ln -s /opt/kubectx/kubectx /usr/local/bin/kubectx \
    && ln -s /opt/kubectx/kubens /usr/local/bin/kubens

# Install k9s for terminal-based Kubernetes management
RUN curl -sS https://webinstall.dev/k9s | bash \
    && mv ~/.local/bin/k9s /usr/local/bin/

# Install Terraform Language Server
RUN curl -fsSL https://releases.hashicorp.com/terraform-ls/0.32.7/terraform-ls_0.32.7_linux_amd64.zip -o terraform-ls.zip \
    && unzip terraform-ls.zip \
    && mv terraform-ls /usr/local/bin/ \
    && rm terraform-ls.zip

# Install tflint for Terraform linting
RUN curl -s https://raw.githubusercontent.com/terraform-linters/tflint/master/install_linux.sh | bash

# Install terragrunt
RUN curl -fsSL https://github.com/gruntwork-io/terragrunt/releases/download/v0.55.1/terragrunt_linux_amd64 -o /usr/local/bin/terragrunt \
    && chmod +x /usr/local/bin/terragrunt

# Set up bash completion
RUN echo "source /usr/share/bash-completion/bash_completion" >> /home/vscode/.bashrc \
    && echo "complete -C /usr/local/bin/terraform terraform" >> /home/vscode/.bashrc \
    && echo "source <(kubectl completion bash)" >> /home/vscode/.bashrc \
    && echo "source <(helm completion bash)" >> /home/vscode/.bashrc

# Install dotnet tools globally
RUN dotnet tool install --global dotnet-ef \
    && dotnet tool install --global dotnet-outdated-tool \
    && dotnet tool install --global dotnet-format \
    && dotnet tool install --global GitVersion.Tool

# Ensure tools are in PATH
ENV PATH="${PATH}:/home/vscode/.dotnet/tools"

USER vscode