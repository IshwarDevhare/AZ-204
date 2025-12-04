# Create Windows VM with Azure CLI

## Prerequisites
- Azure CLI installed and logged in (`az login`)
- Active Azure subscription

## Quick Start

### 1. Create Resource Group
```bash
az group create \
  --name myResourceGroup \
  --location eastus
```

### 2. Create Windows VM
```bash
az vm create \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --image Win2022Datacenter \
  --size Standard_B2s \
  --admin-username azureuser \
  --admin-password 'P@ssw0rd123!' \
  --public-ip-sku Standard
```

> **Note**: For production, use a strong password or SSH keys. The password must be 12-123 characters long and meet complexity requirements (3 of: lowercase, uppercase, digit, special character).

### 3. Open Port 3389 (RDP)
```bash
az vm open-port \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --port 3389
```

### 4. Get Public IP Address
```bash
az vm show \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --show-details \
  --query publicIps \
  --output tsv
```

### 5. Connect via RDP
```bash
# On Linux/macOS with xfreerdp
xfreerdp /v:<PUBLIC_IP> /u:azureuser

# On Windows
mstsc /v:<PUBLIC_IP>
```

## Common VM Operations

### Start VM
```bash
az vm start --resource-group myResourceGroup --name myWindowsVM
```

### Stop VM (deallocate to stop billing)
```bash
az vm deallocate --resource-group myResourceGroup --name myWindowsVM
```

### Restart VM
```bash
az vm restart --resource-group myResourceGroup --name myWindowsVM
```

### Delete VM
```bash
az vm delete --resource-group myResourceGroup --name myWindowsVM --yes
```

### Delete Resource Group (deletes all resources)
```bash
az group delete --name myResourceGroup --yes --no-wait
```

## Popular Windows Images
```bash
# Windows Server 2022 Datacenter
--image Win2022Datacenter

# Windows Server 2019 Datacenter
--image Win2019Datacenter

# Windows Server 2016 Datacenter
--image Win2016Datacenter

# Windows 11 Pro
--image Win11

# Windows 10 Pro
--image Win10

# List all Windows images
az vm image list --offer Windows --all --output table
```

## Common VM Sizes
```bash
# Burstable (Dev/Test)
--size Standard_B1s    # 1 vCPU, 1 GB RAM
--size Standard_B2s    # 2 vCPU, 4 GB RAM
--size Standard_B2ms   # 2 vCPU, 8 GB RAM

# General Purpose
--size Standard_D2s_v3 # 2 vCPU, 8 GB RAM
--size Standard_D4s_v3 # 4 vCPU, 16 GB RAM

# List available sizes in a region
az vm list-sizes --location eastus --output table
```

## Useful Commands

### List all VMs
```bash
az vm list --output table
```

### Get VM details
```bash
az vm show --resource-group myResourceGroup --name myWindowsVM
```

### Get VM status
```bash
az vm get-instance-view \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --query instanceView.statuses[1].displayStatus
```

### Run PowerShell command on VM (without RDP)
```bash
# Run a PowerShell command
az vm run-command invoke \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --command-id RunPowerShellScript \
  --scripts "Get-ComputerInfo | Select-Object WindowsProductName, WindowsVersion"

# Install IIS
az vm run-command invoke \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --command-id RunPowerShellScript \
  --scripts "Install-WindowsFeature -name Web-Server -IncludeManagementTools"

# Check Windows version
az vm run-command invoke \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --command-id RunPowerShellScript \
  --scripts "systeminfo | findstr /B /C:'OS Name' /C:'OS Version'"
```

## Advanced VM Creation

### Create VM with Custom Network Settings
```bash
# Create virtual network
az network vnet create \
  --resource-group myResourceGroup \
  --name myVnet \
  --address-prefix 10.0.0.0/16 \
  --subnet-name mySubnet \
  --subnet-prefix 10.0.1.0/24

# Create public IP
az network public-ip create \
  --resource-group myResourceGroup \
  --name myPublicIP \
  --sku Standard \
  --allocation-method Static

# Create network security group
az network nsg create \
  --resource-group myResourceGroup \
  --name myNSG

# Add RDP rule to NSG
az network nsg rule create \
  --resource-group myResourceGroup \
  --nsg-name myNSG \
  --name AllowRDP \
  --priority 1000 \
  --protocol Tcp \
  --destination-port-ranges 3389 \
  --access Allow

# Create network interface
az network nic create \
  --resource-group myResourceGroup \
  --name myNIC \
  --vnet-name myVnet \
  --subnet mySubnet \
  --public-ip-address myPublicIP \
  --network-security-group myNSG

# Create VM with the network interface
az vm create \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --nics myNIC \
  --image Win2022Datacenter \
  --size Standard_B2s \
  --admin-username azureuser \
  --admin-password 'P@ssw0rd123!'
```

### Create VM with Data Disks
```bash
# Create VM with additional data disk
az vm create \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --image Win2022Datacenter \
  --size Standard_B2s \
  --admin-username azureuser \
  --admin-password 'P@ssw0rd123!' \
  --data-disk-sizes-gb 128 256

# Attach existing disk to VM
az vm disk attach \
  --resource-group myResourceGroup \
  --vm-name myWindowsVM \
  --name myDataDisk \
  --new \
  --size-gb 128

# List disks attached to VM
az vm show \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --query storageProfile.dataDisks
```

### Create VM with Custom Script Extension
```bash
# Create VM and run custom script
az vm extension set \
  --resource-group myResourceGroup \
  --vm-name myWindowsVM \
  --name CustomScriptExtension \
  --publisher Microsoft.Compute \
  --settings '{"commandToExecute":"powershell.exe Install-WindowsFeature -name Web-Server -IncludeManagementTools"}'
```

## Working with VM Extensions

### List available extensions
```bash
az vm extension image list --location eastus --output table
```

### Install Antimalware extension
```bash
az vm extension set \
  --resource-group myResourceGroup \
  --vm-name myWindowsVM \
  --name IaaSAntimalware \
  --publisher Microsoft.Azure.Security \
  --settings '{"AntimalwareEnabled": true, "RealtimeProtectionEnabled": true}'
```

### Install Azure Monitor Agent
```bash
az vm extension set \
  --resource-group myResourceGroup \
  --vm-name myWindowsVM \
  --name AzureMonitorWindowsAgent \
  --publisher Microsoft.Azure.Monitor \
  --enable-auto-upgrade true
```

### List installed extensions
```bash
az vm extension list \
  --resource-group myResourceGroup \
  --vm-name myWindowsVM \
  --output table
```

## Resize VM
```bash
# Stop the VM
az vm deallocate --resource-group myResourceGroup --name myWindowsVM

# Resize the VM
az vm resize \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --size Standard_D2s_v3

# Start the VM
az vm start --resource-group myResourceGroup --name myWindowsVM
```

## Enable Boot Diagnostics
```bash
# Create storage account for boot diagnostics
az storage account create \
  --resource-group myResourceGroup \
  --name mystorageaccount123 \
  --sku Standard_LRS

# Enable boot diagnostics
az vm boot-diagnostics enable \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --storage https://mystorageaccount123.blob.core.windows.net/
```

## Backup and Snapshot

### Create VM snapshot
```bash
# Get OS disk ID
osDiskId=$(az vm show \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --query storageProfile.osDisk.managedDisk.id \
  --output tsv)

# Create snapshot
az snapshot create \
  --resource-group myResourceGroup \
  --name mySnapshot \
  --source $osDiskId
```

### Create VM from snapshot
```bash
# Create disk from snapshot
az disk create \
  --resource-group myResourceGroup \
  --name myNewOSDisk \
  --source mySnapshot

# Create VM from the disk
az vm create \
  --resource-group myResourceGroup \
  --name myRestoredVM \
  --attach-os-disk myNewOSDisk \
  --os-type Windows
```

## Auto-shutdown Configuration
```bash
# Configure auto-shutdown at 7 PM UTC
az vm auto-shutdown \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --time 1900 \
  --timezone "UTC"
```

## Tags and Metadata
```bash
# Add tags to VM
az vm update \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --set tags.Environment=Dev tags.Project=AZ-204 tags.Owner=YourName

# Get VM tags
az vm show \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --query tags
```

## Cost Management

### View VM pricing
```bash
# Get VM size pricing (via Azure Pricing API or Portal)
# Use Azure Pricing Calculator: https://azure.microsoft.com/pricing/calculator/

# Stop VM to avoid compute charges (storage still charged)
az vm deallocate --resource-group myResourceGroup --name myWindowsVM
```

## Troubleshooting

### Reset RDP configuration
```bash
az vm user reset-rdp \
  --resource-group myResourceGroup \
  --name myWindowsVM
```

### Reset admin password
```bash
az vm user update \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --username azureuser \
  --password 'NewP@ssw0rd123!'
```

### Get boot diagnostics logs
```bash
az vm boot-diagnostics get-boot-log \
  --resource-group myResourceGroup \
  --name myWindowsVM
```

### Get serial console output
```bash
az vm get-instance-view \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --query instanceView.bootDiagnostics.consoleScreenshotBlobUri
```

## Tips and Best Practices

1. **Use strong passwords** - At least 12 characters with complexity requirements
2. **Enable Azure Defender** - For enhanced security monitoring
3. **Regular patching** - Enable automatic Windows updates or use Azure Update Management
4. **Use Managed Disks** - Better reliability and easier management (default)
5. **Stop when not in use** - Use auto-shutdown or deallocate manually
6. **Enable backup** - Use Azure Backup for production VMs
7. **Use Standard SKU public IPs** - Better availability for production workloads
8. **Limit RDP access** - Use Just-In-Time access or VPN instead of public RDP
9. **Monitor your VMs** - Enable Azure Monitor and set up alerts
10. **Tag resources** - For better organization and cost tracking

## Security Best Practices

### Configure Network Security Group rules
```bash
# Restrict RDP to specific IP
az network nsg rule update \
  --resource-group myResourceGroup \
  --nsg-name myNSG \
  --name AllowRDP \
  --source-address-prefixes YOUR_IP_ADDRESS/32
```

### Enable Azure Disk Encryption
```bash
# Create Key Vault
az keyvault create \
  --name myKeyVault123 \
  --resource-group myResourceGroup \
  --location eastus \
  --enabled-for-disk-encryption

# Enable encryption on VM
az vm encryption enable \
  --resource-group myResourceGroup \
  --name myWindowsVM \
  --disk-encryption-keyvault myKeyVault123
```

## Clean Up Resources
```bash
# Delete everything in the resource group
az group delete --name myResourceGroup --yes --no-wait
```

## Additional Resources
- [Azure VM Documentation](https://docs.microsoft.com/azure/virtual-machines/)
- [Azure CLI VM Commands](https://docs.microsoft.com/cli/azure/vm)
- [Windows VM Sizes](https://docs.microsoft.com/azure/virtual-machines/sizes)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
