locals {

  # Combine multiple values for the seed. This is used to create a unique affix
  # value and acts like a deterinistic hash. Used in resources that must have unique names
  # globally such as key vaults and storage accounts.
  seed_inputs = {
    project     = var.project_name
    environment = var.environment
    location    = var.location
    component   = "platform"
    subscription_id = data.azurerm_subscription.current.id
  }

  # Create deterministic affix from JSON representation
  affix_seed = jsonencode(local.seed_inputs)
  affix      = substr(md5(local.affix_seed), 0, 4)

  allowed_environments = ["dev", "tst", "stg", "prd", "sbx", "eph"]

  # Storage account name (must be globally unique, alphanumeric only, max 24 chars)
  storage_account_name = replace("${local.azure_resource_abbreviations.storage_account}tfstate${local.allowed_environments["prd"]}${locals.affix}", "-", "")

  # Resource group names
  tfstate_resource_group_name = "${local.azure_resource_abbreviations.resource_group}-tfstate-${local.allowed_environments["prd"]}"

  # Combined tags
  common_tags = merge(var.tags, {
    Environment = var.environment
    CreatedDate = timestamp()
  })
  
  # Container names for different purposes
  state_containers = {
    # Platform shared resources
    platform = "platform-tfstate"
    
    # Application/solution specific
    solutions = "solutions-tfstate"
    
    # Ephemeral environments (feature branches, PRs)
    ephemeral = "ephemeral-tfstate"
    
    # Sandbox environments
    sandbox = "sandbox-tfstate"
  }

# https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations#storage
  azure_resource_abbreviations = {
    storage_account = "st"
    resource_group  = "rg"
  } 
}