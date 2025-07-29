# Bootstrap Infrastructure

This directory contains the foundational Terraform configuration for setting up the storage account that will hold Terraform state files for all environments.

## Purpose

The bootstrap module creates:
- Resource Group for bootstrap resources
- Storage Account with versioning and soft delete
- Multiple containers for different state file types:
  - `platform-tfstate`: Platform/hub resources
  - `solutions-tfstate`: Application-specific resources
  - `ephemeral-tfstate`: Feature branch/PR environments
  - `sandbox-tfstate`: Sandbox environments
- Lifecycle management policies
- Network access rules (optional)

## Usage

### Initial Setup

1. **Copy the example variables file:**
   ```bash
   cp terraform.tfvars.example terraform.tfvars
   ```

2. **Edit `terraform.tfvars` with your values:**
   ```hcl
   environment = "dev"
   location    = "East US 2"
   # Add your IP addresses if you want to restrict access
   ```

3. **Deploy using the script:**
   ```bash
   ./deploy.sh dev
   ```

### Manual Deployment

```bash
# Initialize
terraform init

# Plan
terraform plan -var="environment=dev"

# Apply
terraform apply -var="environment=dev"
```

## Multi-Environment Support

Deploy to different environments by changing the environment variable:

```bash
./deploy.sh dev
./deploy.sh test
./deploy.sh prod
```

Each environment gets its own storage account with the naming pattern:
`{project-name}-{environment}-{random-suffix}`

## State File Organization

```
Storage Account
├── platform-tfstate/
│   ├── networking/terraform.tfstate
│   ├── firewall/terraform.tfstate
│   └── shared-services/terraform.tfstate
├── solutions-tfstate/
│   ├── scriban-app/terraform.tfstate
│   └── api-gateway/terraform.tfstate
├── ephemeral-tfstate/
│   ├── feature-123/terraform.tfstate
│   └── pr-456/terraform.tfstate
└── sandbox-tfstate/
    ├── user1-sandbox/terraform.tfstate
    └── experiment-xyz/terraform.tfstate
```

## Backend Configuration

After bootstrap deployment, use this backend configuration in your other modules:

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "scriban-poc-dev-bootstrap-rg"
    storage_account_name = "scribanpocdevxxxx"
    container_name       = "platform-tfstate"  # or solutions-tfstate, etc.
    key                  = "networking/terraform.tfstate"
  }
}
```

## Security Features

- **Versioning**: Enabled for state file history
- **Soft Delete**: 30-day retention for accidental deletions
- **Lifecycle Management**: Automatic cleanup of old versions
- **Network Restrictions**: Optional IP-based access control
- **TLS 1.2**: Minimum required encryption

## Pipeline Integration

This bootstrap can be run from:
- **Local development**: Manual deployment with Azure CLI
- **GitHub Actions**: Using Azure service principal
- **Azure DevOps**: Using service connections

The storage account supports both scenarios with proper access controls.