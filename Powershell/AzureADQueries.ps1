# Requirements AzureRM module and AzureADPreview installed

# Connect to Azure Subscription
Login-AzureRmAccount

# Get list of Azure Subscriptions
$subscription = Get-AzureRmSubscription -SubscriptionName "Visual Studio Enterprise – MPN"

# Select the right Subscription
$subscriptionId = $subscription.Id
Select-AzureRmSubscription -SubscriptionId $subscriptionId

# Get Azure Tenants for actual account
Get-AzureRmTenant

# Directory Id Tenants
$tenantId="########-####-####-####-############"

# Connect to Azure AD for given AD
Connect-AzureAD -TenantId $tenantId

#Get Azure Tenant Detail
Get-AzureADTenantDetail

# Get Azure Directory Roles
Get-AzureADDirectoryRole

$administatorRoleId = "########-####-####-####-############"
Get-AzureADDirectoryRoleMember -ObjectId $administatorRoleId


# Get list of Azure AD Users
Get-AzureADUser 
$adUser = Get-AzureADUser -Filter "startswith(userPrincipalName,'cpim')"

$userObjectId = $adUser.ObjectId;

Set-AzureADUser -ObjectId $userObjectId -DisplayName ("LinkedIn " + $adUser.DisplayName)

# Get List of Azure AD Applications
Get-AzureADApplication 
