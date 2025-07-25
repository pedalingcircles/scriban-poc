output "storage_account_name" {
  description = "Name of the storage account for Terraform state"
  value       = azurerm_storage_account.tfstate.name
}

output "storage_account_id" {
  description = "ID of the storage account for Terraform state"
  value       = azurerm_storage_account.tfstate.id
}

output "resource_group_name" {
  description = "Name of the bootstrap resource group"
  value       = azurerm_resource_group.bootstrap.name
}

output "state_containers" {
  description = "Map of state container names"
  value       = local.state_containers
}

output "backend_config" {
  description = "Backend configuration for other Terraform modules"
  value = {
    storage_account_name = azurerm_storage_account.tfstate.name
    resource_group_name  = azurerm_resource_group.bootstrap.name
    containers = {
      for k, v in local.state_containers : k => v
    }
  }
}

# Output the backend configuration as a formatted string for easy copy/paste
output "backend_config_example" {
  description = "Example backend configuration block"
  value = <<-EOT
    terraform {
      backend "azurerm" {
        resource_group_name  = "${azurerm_resource_group.bootstrap.name}"
        storage_account_name = "${azurerm_storage_account.tfstate.name}"
        container_name       = "platform-tfstate"  # or solutions-tfstate, ephemeral-tfstate, etc.
        key                  = "your-module/terraform.tfstate"
      }
    }
  EOT
}