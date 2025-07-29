# Random suffix for storage account uniqueness
resource "random_string" "suffix" {
  length  = 4
  upper   = false
  special = false
}

# Resource Group for Terraform state
resource "azurerm_resource_group" "tfstate" {
  name     = "${local.azure_resource_abbreviations.resource_group}tfstate${local.allowed_environments["prd"]}${locals.affix}"
  location = var.location
  tags     = local.common_tags
}

# Storage Account for Terraform state
resource "azurerm_storage_account" "tfstate" {
  name                     = "${local.azure_resource_abbreviations.storage_account}tfstate${local.allowed_environments["prd"]}${locals.affix}"
  resource_group_name      = azurerm_resource_group.tfstate.name
  location                 = azurerm_resource_group.tfstate.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  
  # Security settings
  min_tls_version                 = "TLS1_2"
  allow_nested_items_to_be_public = false
  public_network_access_enabled   = true # Can be restricted later
  
  # Enable features for state file management
  blob_properties {
    versioning_enabled = var.enable_versioning
    
    dynamic "delete_retention_policy" {
      for_each = var.enable_soft_delete ? [1] : []
      content {
        days = var.soft_delete_retention_days
      }
    }
    
    dynamic "container_delete_retention_policy" {
      for_each = var.enable_soft_delete ? [1] : []
      content {
        days = var.soft_delete_retention_days
      }
    }
  }
  
  tags = local.common_tags
}

# Network access rules (if IPs are specified)
resource "azurerm_storage_account_network_rules" "tfstate" {
  count = length(var.allowed_ips) > 0 ? 1 : 0
  
  storage_account_id = azurerm_storage_account.tfstate.id
  
  default_action = "Deny"
  ip_rules       = var.allowed_ips
  
  # Allow Azure services
  bypass = ["AzureServices"]
}

# Storage containers for different state file types
resource "azurerm_storage_container" "state_containers" {
  for_each = local.state_containers
  
  name                  = each.value
  storage_account_name  = azurerm_storage_account.tfstate.name
  container_access_type = "private"
  
  depends_on = [azurerm_storage_account.tfstate]
}

# Create a management policy for lifecycle management
resource "azurerm_storage_management_policy" "tfstate" {
  storage_account_id = azurerm_storage_account.tfstate.id
  
  rule {
    name    = "tfstate-lifecycle"
    enabled = true
    
    filters {
      prefix_match = ["platform-tfstate/", "solutions-tfstate/", "sandbox-tfstate/"]
      blob_types   = ["blockBlob"]
    }
    
    actions {
      base_blob {
        # Keep current versions for 90 days in hot tier
        tier_to_cool_after_days_since_modification_greater_than = 30
        tier_to_archive_after_days_since_modification_greater_than = 90
      }
      
      version {
        # Clean up old versions after 30 days
        delete_after_days_since_creation = 30
      }
    }
  }
  
  # Separate rule for ephemeral environments (shorter retention)
  rule {
    name    = "ephemeral-lifecycle"
    enabled = true
    
    filters {
      prefix_match = ["ephemeral-tfstate/"]
      blob_types   = ["blockBlob"]
    }
    
    actions {
      base_blob {
        # Shorter retention for ephemeral environments
        delete_after_days_since_modification_greater_than = 7
      }
      
      version {
        delete_after_days_since_creation = 3
      }
    }
  }
}