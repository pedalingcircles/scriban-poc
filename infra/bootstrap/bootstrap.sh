#!/bin/bash
set -euo pipefail

# Bootstrap deployment script
# This script handles the initial deployment of the storage account for Terraform state

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV=${1:-dev}

echo "🚀 Deploying bootstrap infrastructure for environment: $ENV"

# Check if Azure CLI is logged in
if ! az account show &>/dev/null; then
    echo "❌ Please login to Azure CLI first: az login"
    exit 1
fi

# Check if terraform.tfvars exists
if [[ ! -f "$SCRIPT_DIR/terraform.tfvars" ]]; then
    echo "⚠️  terraform.tfvars not found. Creating from example..."
    cp "$SCRIPT_DIR/terraform.tfvars.example" "$SCRIPT_DIR/terraform.tfvars"
    echo "📝 Please edit terraform.tfvars with your values"
    exit 1
fi

cd "$SCRIPT_DIR"

echo "📦 Initializing Terraform..."
terraform init

echo "🔍 Planning deployment..."
terraform plan -var="environment=$ENV" -out="tfplan"

echo "🎯 Applying changes..."
terraform apply "tfplan"

echo "📋 Deployment complete! Backend configuration:"
terraform output -raw backend_config_example

echo ""
echo "💡 Next steps:"
echo "1. Copy the backend configuration above"
echo "2. Add it to your platform and solution modules"
echo "3. Run 'terraform init' in those modules to migrate state"