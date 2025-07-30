
variable "regions" {
  description = "List of Azure regions in order: [primary, secondary, ...replication regions]"
  type        = list(string)
}

variable "environment" {
  description = "Environment name"
  type        = string

  validation {
    condition     = contains(keys(local.allowed_environments), var.environment)
    error_message = "Environment must be one of: ${join(", ", keys(local.allowed_environments))}."
  }
}

variable "project_name" {
  description = "Project name for resource naming"
  type        = string
  default     = "contoso"

  validation {
    condition     = can(regex("^[a-z0-9-]+$", var.project_name))
    error_message = "Project name must be lowercase alphanumeric with hyphens only."
  }
}

variable "tags" {
  description = "Common tags for all resources"
  type        = map(string)
  default = {
    Project   = "scriban-poc"
    ManagedBy = "terraform"
    Component = "bootstrap"
  }
}

variable "enable_versioning" {
  description = "Enable blob versioning on storage account"
  type        = bool
  default     = true
}

variable "enable_soft_delete" {
  description = "Enable soft delete for blobs and containers"
  type        = bool
  default     = true
}

variable "soft_delete_retention_days" {
  description = "Number of days to retain soft deleted items"
  type        = number
  default     = 30

  validation {
    condition     = var.soft_delete_retention_days >= 1 && var.soft_delete_retention_days <= 365
    error_message = "Soft delete retention must be between 1 and 365 days."
  }
}

variable "allowed_ips" {
  description = "List of IP addresses allowed to access storage account"
  type        = list(string)
  default     = []
}