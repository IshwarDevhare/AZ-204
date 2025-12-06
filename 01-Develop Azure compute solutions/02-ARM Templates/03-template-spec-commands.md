# Azure Template Spec Commands

## Overview

Template Specs provide a way to store and share ARM templates as Azure resources. They enable centralized template management, versioning, and controlled access through Azure RBAC.

## Prerequisites

```bash
# Ensure you're logged in to Azure
az login

# Set your subscription (if you have multiple)
az account set --subscription "Your-Subscription-Name"

# Create a resource group for template specs (if not exists)
az group create --name rg-templatespecs --location eastus
```

## Creating Template Specs

### Create a Template Spec from JSON file

```bash
# Create a template spec from an ARM template file
az ts create \
  --name storageSpec \
  --version "1.0" \
  --resource-group rg-templatespecs \
  --location "eastus" \
  --template-file "./01-azuredeploy.json" \
  --description "Storage account template spec v1.0"
```

### Create Template Spec with Parameters

```bash
# Create template spec with parameterized storage account
az ts create \
  --name storageSpecParam \
  --version "1.0" \
  --resource-group rg-templatespecs \
  --location "eastus" \
  --template-file "./02-azuredeployspec.json" \
  --description "Parameterized storage account template"
```

### Create a New Version

```bash
# Add a new version to existing template spec
az ts create \
  --name storageSpec \
  --version "2.0" \
  --resource-group rg-templatespecs \
  --location "eastus" \
  --template-file "./01-azuredeploy.json" \
  --description "Storage account template spec v2.0 - Updated configuration"
```

### Create Template Spec with Display Name

```bash
# Create with custom display name
az ts create \
  --name storageSpec \
  --version "1.0" \
  --resource-group rg-templatespecs \
  --location "eastus" \
  --template-file "./01-azuredeploy.json" \
  --display-name "Standard Storage Account Template" \
  --description "Deploys a standard LRS storage account"
```

## Listing Template Specs

### List All Template Specs in Subscription

```bash
# List all template specs in the current subscription
az ts list
```

### List Template Specs in Resource Group

```bash
# List template specs in a specific resource group
az ts list --resource-group rg-templatespecs
```

### List with Output Formatting

```bash
# List with table output
az ts list --resource-group rg-templatespecs --output table

# List with JSON output
az ts list --resource-group rg-templatespecs --output json
```

## Viewing Template Spec Details

### Show Template Spec Information

```bash
# Show details of a template spec
az ts show \
  --name storageSpec \
  --resource-group rg-templatespecs \
  --version "1.0"
```

### Show Latest Version

```bash
# Show latest version of template spec
az ts show \
  --name storageSpec \
  --resource-group rg-templatespecs
```

### Export Template Spec

```bash
# Export template spec to a file
az ts export \
  --name storageSpec \
  --version "1.0" \
  --resource-group rg-templatespecs \
  --output-folder "./exported-templates"
```

## Deploying Using Template Specs

### Deploy Template Spec to Resource Group

```bash
# Deploy using template spec ID
az deployment group create \
  --resource-group rg-demo \
  --template-spec "/subscriptions/{subscription-id}/resourceGroups/rg-templatespecs/providers/Microsoft.Resources/templateSpecs/storageSpec/versions/1.0"
```

### Deploy with Resource ID Variable

```bash
# Store template spec ID in variable
TEMPLATE_SPEC_ID=$(az ts show \
  --name storageSpec \
  --resource-group rg-templatespecs \
  --version "1.0" \
  --query "id" \
  --output tsv)

# Deploy using the variable
az deployment group create \
  --resource-group rg-demo \
  --template-spec $TEMPLATE_SPEC_ID
```

### Deploy Latest Version

```bash
# Deploy latest version (omit version in path)
az deployment group create \
  --resource-group rg-demo \
  --template-spec "/subscriptions/{subscription-id}/resourceGroups/rg-templatespecs/providers/Microsoft.Resources/templateSpecs/storageSpec"
```

### Deploy with Parameters

```bash
# Deploy with inline parameters
az deployment group create \
  --resource-group rg-demo \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters storageAccountName=mystorageacct001 location=eastus
```

### Deploy with Parameter File

```bash
# Create a parameters file (parameters.json)
# {
#   "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
#   "contentVersion": "1.0.0.0",
#   "parameters": {
#     "storageAccountName": {
#       "value": "mystorageacct001"
#     },
#     "location": {
#       "value": "eastus"
#     }
#   }
# }

# Deploy using parameter file
az deployment group create \
  --resource-group rg-demo \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters @parameters.json
```

### Deploy with What-If Analysis

```bash
# Preview changes before deployment
az deployment group what-if \
  --resource-group rg-demo \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters storageAccountName=mystorageacct001
```

### Deploy with Custom Name

```bash
# Deploy with custom deployment name
az deployment group create \
  --name "storage-deployment-prod" \
  --resource-group rg-demo \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters storageAccountName=prodstorage001
```

## Deployment Validation

### Validate Template Spec Before Deployment

```bash
# Validate the template spec deployment without actually deploying
az deployment group validate \
  --resource-group rg-demo \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters storageAccountName=mystorageacct001
```

### Check Deployment Status

```bash
# Show deployment details
az deployment group show \
  --name "storage-deployment-prod" \
  --resource-group rg-demo

# List all deployments in resource group
az deployment group list \
  --resource-group rg-demo \
  --output table
```

## Updating Template Specs

### Update Template Spec (Create New Version)

```bash
# Update by creating a new version
az ts create \
  --name storageSpec \
  --version "1.1" \
  --resource-group rg-templatespecs \
  --location "eastus" \
  --template-file "./01-azuredeploy.json" \
  --description "Bug fixes and improvements"
```

### Update Existing Version

```bash
# Update an existing version (overwrites)
az ts update \
  --name storageSpec \
  --version "1.0" \
  --resource-group rg-templatespecs \
  --template-file "./01-azuredeploy.json" \
  --description "Updated description"
```

## Managing Template Spec Versions

### List All Versions

```bash
# List all versions of a template spec
az ts list \
  --name storageSpec \
  --resource-group rg-templatespecs
```

### Delete a Specific Version

```bash
# Delete a specific version
az ts delete \
  --name storageSpec \
  --version "1.0" \
  --resource-group rg-templatespecs \
  --yes
```

### Delete Template Spec (All Versions)

```bash
# Delete entire template spec with all versions
az ts delete \
  --name storageSpec \
  --resource-group rg-templatespecs \
  --yes
```

## Access Control and Permissions

### Grant Read Access to Template Spec

```bash
# Assign Template Spec Reader role
az role assignment create \
  --role "Template Spec Reader" \
  --assignee user@example.com \
  --scope "/subscriptions/{subscription-id}/resourceGroups/rg-templatespecs/providers/Microsoft.Resources/templateSpecs/storageSpec"
```

### Grant Contributor Access

```bash
# Assign Template Spec Contributor role
az role assignment create \
  --role "Template Spec Contributor" \
  --assignee user@example.com \
  --scope "/subscriptions/{subscription-id}/resourceGroups/rg-templatespecs"
```

## Advanced Scenarios

### Deploy to Subscription Level

```bash
# Create subscription-level template spec
az ts create \
  --name subscriptionSpec \
  --version "1.0" \
  --resource-group rg-templatespecs \
  --location "eastus" \
  --template-file "./subscription-template.json"

# Deploy at subscription level
az deployment sub create \
  --location "eastus" \
  --template-spec $TEMPLATE_SPEC_ID
```

### Deploy to Management Group

```bash
# Deploy template spec at management group level
az deployment mg create \
  --management-group-id "mg-production" \
  --location "eastus" \
  --template-spec $TEMPLATE_SPEC_ID
```

### Deploy with Tags

```bash
# Deploy with resource tags
az deployment group create \
  --resource-group rg-demo \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters storageAccountName=mystorageacct001 \
  --tags environment=production department=IT
```

## PowerShell Commands

### Create Template Spec (PowerShell)

```powershell
# Create template spec using PowerShell
New-AzTemplateSpec `
  -Name "storageSpec" `
  -Version "1.0" `
  -ResourceGroupName "rg-templatespecs" `
  -Location "eastus" `
  -TemplateFile "./01-azuredeploy.json" `
  -Description "Storage account template spec"
```

### Deploy Template Spec (PowerShell)

```powershell
# Get template spec
$templateSpec = Get-AzTemplateSpec `
  -ResourceGroupName "rg-templatespecs" `
  -Name "storageSpec" `
  -Version "1.0"

# Deploy template spec
New-AzResourceGroupDeployment `
  -ResourceGroupName "rg-demo" `
  -TemplateSpecId $templateSpec.Versions[0].Id `
  -storageAccountName "mystorageacct001"
```

### List Template Specs (PowerShell)

```powershell
# List all template specs
Get-AzTemplateSpec -ResourceGroupName "rg-templatespecs"

# Get specific template spec
Get-AzTemplateSpec `
  -ResourceGroupName "rg-templatespecs" `
  -Name "storageSpec" `
  -Version "1.0"
```

## Best Practices

1. **Versioning Strategy**
   - Use semantic versioning (e.g., 1.0.0, 1.1.0, 2.0.0)
   - Create new versions for changes rather than updating existing ones
   - Maintain clear descriptions for each version

2. **Organization**
   - Use dedicated resource group for template specs
   - Use consistent naming conventions
   - Group related template specs together

3. **Security**
   - Apply least-privilege access using RBAC
   - Use Template Spec Reader role for deployment-only access
   - Audit access and deployments regularly

4. **Testing**
   - Validate templates before creating template specs
   - Use what-if operations before actual deployments
   - Test in non-production environments first

5. **Documentation**
   - Include clear descriptions for each version
   - Document parameters and their expected values
   - Maintain change logs for version updates

## Common Use Cases

### Scenario 1: Centralized Template Library

```bash
# Create template specs for common resources
az ts create --name "vm-windows" --version "1.0" \
  --resource-group rg-templatespecs --location eastus \
  --template-file "./vm-windows.json"

az ts create --name "vm-linux" --version "1.0" \
  --resource-group rg-templatespecs --location eastus \
  --template-file "./vm-linux.json"

az ts create --name "sql-database" --version "1.0" \
  --resource-group rg-templatespecs --location eastus \
  --template-file "./sql-database.json"
```

### Scenario 2: Environment-Specific Deployments

```bash
# Deploy to development environment
az deployment group create \
  --resource-group rg-dev \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters environment=dev tier=Standard

# Deploy to production environment
az deployment group create \
  --resource-group rg-prod \
  --template-spec $TEMPLATE_SPEC_ID \
  --parameters environment=prod tier=Premium
```

### Scenario 3: CI/CD Pipeline Integration

```bash
# Example Azure DevOps pipeline step
# - task: AzureCLI@2
#   inputs:
#     azureSubscription: 'Azure-Connection'
#     scriptType: 'bash'
#     scriptLocation: 'inlineScript'
#     inlineScript: |
#       az deployment group create \
#         --resource-group $(resourceGroup) \
#         --template-spec $(templateSpecId) \
#         --parameters @parameters.$(environment).json
```

## Troubleshooting

### Common Errors

1. **Template Spec Not Found**
   ```bash
   # Verify template spec exists
   az ts show --name storageSpec --resource-group rg-templatespecs
   ```

2. **Permission Denied**
   ```bash
   # Check role assignments
   az role assignment list --scope /subscriptions/{subscription-id}/resourceGroups/rg-templatespecs
   ```

3. **Deployment Validation Failed**
   ```bash
   # Validate template before deployment
   az deployment group validate \
     --resource-group rg-demo \
     --template-spec $TEMPLATE_SPEC_ID \
     --parameters @parameters.json
   ```

## References

- [Azure Template Specs Documentation](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-specs)
- [ARM Template Reference](https://docs.microsoft.com/azure/templates/)
- [Azure CLI Reference](https://docs.microsoft.com/cli/azure/ts)
