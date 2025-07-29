data "azurerm_subscription" "current" {}

output "subscription_id" {
  value = data.azurerm_subscription.current.id
}