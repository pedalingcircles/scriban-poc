locals {

  # Load Azure regions data
  azure_regions = yamldecode(file("${path.module}/../azure-regions.yaml"))

  # Create region map with abbreviations
  region_map = {
    for region in var.regions :
    region => {
      name                     = region
      display_name            = local.azure_regions[region].display_name
      regional_display_name   = local.azure_regions[region].regional_display_name
      abbreviation           = local.azure_regions[region].abbreviation
    }
  }

  primary_region   = var.regions[0]
  secondary_region = var.regions[1]
  extra_regions    = slice(var.regions, 2, length(var.regions))  # all after index 1

  # Resource group mapping with abbreviated region names
  rg_map = {
    for region in var.regions :
    region => {
      name     = "${var.project_name}-${local.azure_resource_abbreviations.resource_group}-${local.azure_regions[region].abbreviation}"
      location = region
    }
  }

  # Combine multiple values for the seed. This is used to create a unique affix
  # value and acts like a deterministic value. This is used in resources that must have unique names
  # globally such as key vaults and storage accounts. Generally things that uses global DNS.
  seed_inputs = {
    project         = var.project_name
    environment     = var.environment
    location        = local.primary_region
    component       = "platform"
    subscription_id = data.azurerm_subscription.current.id
  }

  # Create deterministic affix value from JSON representation
  affix_seed          = jsonencode(local.seed_inputs)
  deterministic_affix = substr(md5(local.affix_seed), 0, 4)

  # Load Azure resource abbreviations from external YAML file
  azure_resource_abbreviations = yamldecode(file("${path.module}/../azure-resource-abbreviations.yaml"))

  allowed_environments = {
    dev = "dev" # The development environment
    qa  = "qa"  # The quality assurance environment
    tst = "tst" # The testing environment
    stg = "stg" # The staging environment
    prd = "prd" # The production environment
    sbx = "sbx" # The sandbox environment
    eph = "eph" # The ephemeral environment
  }

  # Storage account name (must be globally unique, alphanumeric only, max 24 chars)
  storage_account_name = replace("${local.azure_resource_abbreviations.storage_account}tfstate${local.allowed_environments["prd"]}${local.deterministic_affix}", "-", "")

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
}