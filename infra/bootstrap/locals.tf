locals {

  # Combine multiple values for the seed. This is used to create a unique affix
  # value and acts like a deterministic value. This is used in resources that must have unique names
  # globally such as key vaults and storage accounts. Generally things that uses global DNS.
  seed_inputs = {
    project     = var.project_name
    environment = var.environment
    location    = var.location
    component   = "platform"
    subscription_id = data.azurerm_subscription.current.id
  }

  # Create deterministic affix value from JSON representation
  affix_seed = jsonencode(local.seed_inputs)
  deterministic_affix      = substr(md5(local.affix_seed), 0, 4)

  allowed_environments = [
    "dev",  # The development environment
    "qa",   # The quality assurance environment
    "tst",  # The testing environment
    "stg",  # The staging environment
    "prd",  # The production environment
    "sbx",  # The sandbox environment
    "eph"   # The ephemeral environment
  ]

  # Storage account name (must be globally unique, alphanumeric only, max 24 chars)
  storage_account_name = replace("${local.azure_resource_abbreviations.storage_account}tfstate${local.allowed_environments["prd"]}${locals.deterministic_affix}", "-", "")

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
    key_vault       = "kv"
    app_service_plan     = "asp"
    function_app    = "func"
    virtual_machine = "vm"
    vm_storage_account = "stvm"
    web_app        = "app"
    container_registry = "cr"
    cosmos_db = "cosmos"
    redis_cache = "redis"
    sql_database_server = "sql"
    sql_database = "sqldb"
    application_insights = "appi"
    log_analytics_workspace = "log"
    event_hub_namespace = "evhns"
    event_hub = "evh"
    service_bus_namespace = "sbns"
    service_bus_queue = "sbq"
    service_bus_topic = "sbt"
    service_bus_topic_subscription = "sbts"
    event_hub = "evh"
    event_grid_namespace = "evgns"
  } 
}