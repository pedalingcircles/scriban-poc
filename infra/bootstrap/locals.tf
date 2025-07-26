locals {
  # Generate consistent naming
  name_prefix = "${var.project_name}-${var.environment}"
  
  # Storage account name (must be globally unique, alphanumeric only, max 24 chars)
  storage_account_name = replace("${substr(local.name_prefix, 0, 20)}${random_string.suffix.result}", "-", "")
  
  # Resource names
  resource_group_name = "${local.name_prefix}-bootstrap-rg"
  
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
}