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

USER vscode