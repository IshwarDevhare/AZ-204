# Create Windows VM with Azure PowerShell

## Prerequisites
- Azure PowerShell module installed (`Install-Module -Name Az -AllowClobber -Scope CurrentUser`)
- Logged in to Azure (`Connect-AzAccount`)
- Active Azure subscription

## Quick Start

### 1. Create Resource Group
```powershell
New-AzResourceGroup `
  -Name "myResourceGroup" `
  -Location "EastUS"
```

### 2. Create Credentials for VM
```powershell
# Create credential object for Windows VM
$securePassword = ConvertTo-SecureString 'P@ssw0rd123!' -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential ('azureuser', $securePassword)
```

> **Note**: For production, use a strong password. The password must be 12-123 characters long and meet complexity requirements (3 of: lowercase, uppercase, digit, special character).

### 3. Create Windows VM
```powershell
New-AzVM `
  -ResourceGroupName "myResourceGroup" `
  -Name "myWindowsVM" `
  -Location "EastUS" `
  -Image "Win2022Datacenter" `
  -Size "Standard_B2s" `
  -Credential $cred `
  -OpenPorts 3389 `
  -PublicIpAddressName "myPublicIP"
```

### 4. Get Public IP Address
```powershell
Get-AzPublicIpAddress `
  -ResourceGroupName "myResourceGroup" `
  -Name "myPublicIP" | Select-Object IpAddress
```

### 5. Connect via RDP
```powershell
# Get the public IP
$publicIp = (Get-AzPublicIpAddress -ResourceGroupName "myResourceGroup" -Name "myPublicIP").IpAddress

# Connect via RDP (Windows)
mstsc /v:$publicIp

# Or save RDP file
$publicIp | Out-File -FilePath "$env:USERPROFILE\Desktop\vm-connection.rdp"
```

## Common VM Operations

### Start VM
```powershell
Start-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
```

### Stop VM (deallocate to stop billing)
```powershell
Stop-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM" -Force
```

### Restart VM
```powershell
Restart-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
```

### Get VM Status
```powershell
Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM" -Status
```

### Delete VM
```powershell
Remove-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM" -Force
```

### Delete Resource Group (deletes all resources)
```powershell
Remove-AzResourceGroup -Name "myResourceGroup" -Force
```

## Advanced VM Creation with Custom Network Settings

### Create VM with Full Network Configuration
```powershell
# Create a subnet configuration
$subnetConfig = New-AzVirtualNetworkSubnetConfig `
  -Name "mySubnet" `
  -AddressPrefix "10.0.1.0/24"

# Create a virtual network
$vnet = New-AzVirtualNetwork `
  -ResourceGroupName "myResourceGroup" `
  -Location "EastUS" `
  -Name "myVnet" `
  -AddressPrefix "10.0.0.0/16" `
  -Subnet $subnetConfig

# Create a public IP address
$pip = New-AzPublicIpAddress `
  -ResourceGroupName "myResourceGroup" `
  -Location "EastUS" `
  -Name "myPublicIP" `
  -AllocationMethod Static `
  -Sku Standard

# Create a network security group rule for RDP
$nsgRuleRDP = New-AzNetworkSecurityRuleConfig `
  -Name "myNSGRuleRDP" `
  -Protocol Tcp `
  -Direction Inbound `
  -Priority 1000 `
  -SourceAddressPrefix * `
  -SourcePortRange * `
  -DestinationAddressPrefix * `
  -DestinationPortRange 3389 `
  -Access Allow

# Create a network security group
$nsg = New-AzNetworkSecurityGroup `
  -ResourceGroupName "myResourceGroup" `
  -Location "EastUS" `
  -Name "myNSG" `
  -SecurityRules $nsgRuleRDP

# Create a virtual network interface card
$nic = New-AzNetworkInterface `
  -Name "myNIC" `
  -ResourceGroupName "myResourceGroup" `
  -Location "EastUS" `
  -SubnetId $vnet.Subnets[0].Id `
  -PublicIpAddressId $pip.Id `
  -NetworkSecurityGroupId $nsg.Id

# Create credential object
$securePassword = ConvertTo-SecureString 'P@ssw0rd123!' -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential ('azureuser', $securePassword)

# Create VM configuration
$vmConfig = New-AzVMConfig -VMName "myWindowsVM" -VMSize "Standard_B2s" | `
  Set-AzVMOperatingSystem -Windows -ComputerName "myWindowsVM" -Credential $cred | `
  Set-AzVMSourceImage -PublisherName "MicrosoftWindowsServer" -Offer "WindowsServer" -Skus "2022-Datacenter" -Version "latest" | `
  Add-AzVMNetworkInterface -Id $nic.Id

# Create the VM
New-AzVM -ResourceGroupName "myResourceGroup" -Location "EastUS" -VM $vmConfig
```

## Popular Windows Images

### List Available Images
```powershell
# List publishers for Windows
Get-AzVMImagePublisher -Location "EastUS" | Where-Object {$_.PublisherName -like "*Microsoft*Windows*"}

# List offers for Windows Server
Get-AzVMImageOffer -Location "EastUS" -PublisherName "MicrosoftWindowsServer"

# List SKUs for Windows Server
Get-AzVMImageSku -Location "EastUS" -PublisherName "MicrosoftWindowsServer" -Offer "WindowsServer"
```

### Common Image URNs
```powershell
# Windows Server 2022 Datacenter
-Image "Win2022Datacenter"
# Or detailed:
-PublisherName "MicrosoftWindowsServer" -Offer "WindowsServer" -Skus "2022-Datacenter" -Version "latest"

# Windows Server 2019 Datacenter
-Image "Win2019Datacenter"
# Or detailed:
-PublisherName "MicrosoftWindowsServer" -Offer "WindowsServer" -Skus "2019-Datacenter" -Version "latest"

# Windows Server 2016 Datacenter
-Image "Win2016Datacenter"
# Or detailed:
-PublisherName "MicrosoftWindowsServer" -Offer "WindowsServer" -Skus "2016-Datacenter" -Version "latest"

# Windows 11 Pro
-PublisherName "MicrosoftWindowsDesktop" -Offer "Windows-11" -Skus "win11-21h2-pro" -Version "latest"

# Windows 10 Pro
-PublisherName "MicrosoftWindowsDesktop" -Offer "Windows-10" -Skus "win10-21h2-pro" -Version "latest"
```

## Common VM Sizes
```powershell
# Burstable (Dev/Test)
-Size "Standard_B1s"    # 1 vCPU, 1 GB RAM
-Size "Standard_B2s"    # 2 vCPU, 4 GB RAM
-Size "Standard_B2ms"   # 2 vCPU, 8 GB RAM

# General Purpose
-Size "Standard_D2s_v3" # 2 vCPU, 8 GB RAM
-Size "Standard_D4s_v3" # 4 vCPU, 16 GB RAM

# List available sizes in a region
Get-AzVMSize -Location "EastUS" | Format-Table
```

## Useful Commands

### List all VMs
```powershell
Get-AzVM | Format-Table Name, ResourceGroupName, Location
```

### Get VM details
```powershell
Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
```

### Get VM with instance view (detailed status)
```powershell
Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM" -Status | Format-List
```

### List all resource groups
```powershell
Get-AzResourceGroup | Format-Table ResourceGroupName, Location
```

### Run PowerShell command on VM (without RDP)
```powershell
# Run a PowerShell command
Invoke-AzVMRunCommand `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -CommandId "RunPowerShellScript" `
  -ScriptString "Get-ComputerInfo | Select-Object WindowsProductName, WindowsVersion"

# Install IIS
Invoke-AzVMRunCommand `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -CommandId "RunPowerShellScript" `
  -ScriptString "Install-WindowsFeature -name Web-Server -IncludeManagementTools"

# Get system information
Invoke-AzVMRunCommand `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -CommandId "RunPowerShellScript" `
  -ScriptString "Get-WmiObject -Class Win32_OperatingSystem | Select-Object Caption, Version, OSArchitecture"
```

### Resize VM
```powershell
# Stop the VM
Stop-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM" -Force

# Resize the VM
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
$vm.HardwareProfile.VmSize = "Standard_D2s_v3"
Update-AzVM -VM $vm -ResourceGroupName "myResourceGroup"

# Start the VM
Start-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
```

## Working with Data Disks

### Add Data Disk to VM
```powershell
# Create a new managed disk
$diskConfig = New-AzDiskConfig `
  -SkuName Premium_LRS `
  -Location "EastUS" `
  -CreateOption Empty `
  -DiskSizeGB 128

$dataDisk = New-AzDisk `
  -DiskName "myDataDisk" `
  -Disk $diskConfig `
  -ResourceGroupName "myResourceGroup"

# Attach disk to VM
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
$vm = Add-AzVMDataDisk `
  -VM $vm `
  -Name "myDataDisk" `
  -CreateOption Attach `
  -ManagedDiskId $dataDisk.Id `
  -Lun 0

Update-AzVM -ResourceGroupName "myResourceGroup" -VM $vm
```

### Initialize Data Disk (Run on VM)
```powershell
# Run this script on the VM to initialize and format the data disk
Invoke-AzVMRunCommand `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -CommandId "RunPowerShellScript" `
  -ScriptString @"
Get-Disk | Where-Object PartitionStyle -eq 'RAW' | 
  Initialize-Disk -PartitionStyle GPT -PassThru | 
  New-Partition -AssignDriveLetter -UseMaximumSize | 
  Format-Volume -FileSystem NTFS -NewFileSystemLabel 'DataDisk' -Confirm:`$false
"@
```

### List Data Disks
```powershell
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
$vm.StorageProfile.DataDisks | Format-Table Name, DiskSizeGB, Lun
```

## VM Extensions

### Install IIS using Custom Script Extension
```powershell
Set-AzVMExtension `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -Name "InstallIIS" `
  -Publisher "Microsoft.Compute" `
  -Type "CustomScriptExtension" `
  -TypeHandlerVersion "1.10" `
  -SettingString '{"commandToExecute":"powershell Install-WindowsFeature -name Web-Server -IncludeManagementTools"}'
```

### Install Antimalware Extension
```powershell
$settingsString = @"
{
  "AntimalwareEnabled": true,
  "RealtimeProtectionEnabled": true,
  "ScheduledScanSettings": {
    "isEnabled": true,
    "day": "7",
    "time": "120",
    "scanType": "Quick"
  }
}
"@

Set-AzVMExtension `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -Name "IaaSAntimalware" `
  -Publisher "Microsoft.Azure.Security" `
  -Type "IaaSAntimalware" `
  -TypeHandlerVersion "1.5" `
  -SettingString $settingsString
```

### Install Azure Monitor Agent
```powershell
Set-AzVMExtension `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -Name "AzureMonitorWindowsAgent" `
  -Publisher "Microsoft.Azure.Monitor" `
  -Type "AzureMonitorWindowsAgent" `
  -TypeHandlerVersion "1.0" `
  -EnableAutomaticUpgrade $true
```

### List Installed Extensions
```powershell
Get-AzVMExtension `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" | Format-Table Name, Publisher, ExtensionType
```

## Backup and Snapshot

### Create VM Snapshot
```powershell
# Get the VM
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"

# Create snapshot configuration
$snapshotConfig = New-AzSnapshotConfig `
  -SourceUri $vm.StorageProfile.OsDisk.ManagedDisk.Id `
  -Location "EastUS" `
  -CreateOption Copy

# Create the snapshot
New-AzSnapshot `
  -ResourceGroupName "myResourceGroup" `
  -SnapshotName "mySnapshot" `
  -Snapshot $snapshotConfig
```

### Create VM from Snapshot
```powershell
# Get the snapshot
$snapshot = Get-AzSnapshot -ResourceGroupName "myResourceGroup" -SnapshotName "mySnapshot"

# Create disk from snapshot
$diskConfig = New-AzDiskConfig `
  -Location "EastUS" `
  -SourceResourceId $snapshot.Id `
  -CreateOption Copy

$disk = New-AzDisk `
  -Disk $diskConfig `
  -ResourceGroupName "myResourceGroup" `
  -DiskName "myRestoredOSDisk"

# Create VM configuration
$vmConfig = New-AzVMConfig -VMName "myRestoredVM" -VMSize "Standard_B2s"
$vmConfig = Set-AzVMOSDisk -VM $vmConfig -ManagedDiskId $disk.Id -CreateOption Attach -Windows

# Create the VM
New-AzVM -ResourceGroupName "myResourceGroup" -Location "EastUS" -VM $vmConfig
```

## Enable Boot Diagnostics
```powershell
# Create storage account for boot diagnostics
$storageAccount = New-AzStorageAccount `
  -ResourceGroupName "myResourceGroup" `
  -Name "mystorageaccount123" `
  -Location "EastUS" `
  -SkuName Standard_LRS

# Enable boot diagnostics
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
Set-AzVMBootDiagnostic -VM $vm -Enable -ResourceGroupName "myResourceGroup" -StorageAccountName "mystorageaccount123"
Update-AzVM -ResourceGroupName "myResourceGroup" -VM $vm
```

## Auto-shutdown Configuration
```powershell
# Configure auto-shutdown at 7 PM (1900)
$properties = @{
    status = "Enabled"
    taskType = "ComputeVmShutdownTask"
    dailyRecurrence = @{time = "1900"}
    timeZoneId = "UTC"
    notificationSettings = @{
        status = "Disabled"
    }
    targetResourceId = (Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM").Id
}

New-AzResource `
  -ResourceId ("/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.devtestlab/schedules/shutdown-computevm-{2}" -f (Get-AzContext).Subscription.Id, "myResourceGroup", "myWindowsVM") `
  -Location "EastUS" `
  -Properties $properties `
  -Force
```

## Tags and Metadata
```powershell
# Add tags to VM
$tags = @{
    Environment = "Dev"
    Project = "AZ-204"
    Owner = "YourName"
    CostCenter = "IT"
}

$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
Update-AzVM -ResourceGroupName "myResourceGroup" -VM $vm -Tag $tags

# Get VM tags
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
$vm.Tags
```

## Troubleshooting

### Reset RDP Configuration
```powershell
Set-AzVMAccessExtension `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -Name "ResetRDP" `
  -Location "EastUS"
```

### Reset Admin Password
```powershell
$securePassword = ConvertTo-SecureString 'NewP@ssw0rd123!' -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential ('azureuser', $securePassword)

Set-AzVMAccessExtension `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -Name "ResetPassword" `
  -Credential $cred `
  -Location "EastUS"
```

### Get Boot Diagnostics Screenshot
```powershell
Get-AzVMBootDiagnosticsData `
  -ResourceGroupName "myResourceGroup" `
  -Name "myWindowsVM" `
  -Windows
```

## Security Best Practices

### Restrict RDP Access to Specific IP
```powershell
# Update NSG rule to allow RDP only from your IP
$nsg = Get-AzNetworkSecurityGroup -ResourceGroupName "myResourceGroup" -Name "myNSG"
$nsgRule = Get-AzNetworkSecurityRuleConfig -NetworkSecurityGroup $nsg -Name "myNSGRuleRDP"
$nsgRule.SourceAddressPrefix = "YOUR_IP_ADDRESS/32"
Set-AzNetworkSecurityRuleConfig -NetworkSecurityGroup $nsg -Name "myNSGRuleRDP" -SourceAddressPrefix "YOUR_IP_ADDRESS/32"
Set-AzNetworkSecurityGroup -NetworkSecurityGroup $nsg
```

### Enable Azure Disk Encryption
```powershell
# Create Key Vault
$keyVault = New-AzKeyVault `
  -ResourceGroupName "myResourceGroup" `
  -VaultName "myKeyVault123" `
  -Location "EastUS" `
  -EnabledForDiskEncryption

# Enable encryption on VM
Set-AzVMDiskEncryptionExtension `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM" `
  -DiskEncryptionKeyVaultUrl $keyVault.VaultUri `
  -DiskEncryptionKeyVaultId $keyVault.ResourceId `
  -VolumeType All `
  -Force
```

### Check Encryption Status
```powershell
Get-AzVMDiskEncryptionStatus `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myWindowsVM"
```

## Cost Management

### View VM Cost Information
```powershell
# Get VM details including size (affects cost)
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM"
Write-Host "VM Size: $($vm.HardwareProfile.VmSize)"
Write-Host "Use Azure Pricing Calculator for cost estimates"

# Stop VM to avoid compute charges (storage still charged)
Stop-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM" -Force
```

### List All Costs by Tag
```powershell
# Get all VMs with specific tag
Get-AzVM | Where-Object {$_.Tags.Project -eq "AZ-204"} | Format-Table Name, ResourceGroupName, Location
```

## Performance Monitoring

### Get VM Performance Metrics
```powershell
# Get CPU usage metrics
Get-AzMetric `
  -ResourceId (Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myWindowsVM").Id `
  -MetricName "Percentage CPU" `
  -TimeGrain 00:05:00 `
  -StartTime (Get-Date).AddHours(-1)
```

## Tips and Best Practices

1. **Use strong passwords** - At least 12 characters with complexity requirements
2. **Enable Azure Defender** - For enhanced security monitoring
3. **Regular patching** - Enable automatic Windows updates or use Azure Update Management
4. **Use Managed Disks** - Better reliability and easier management (default)
5. **Stop when not in use** - Use auto-shutdown or deallocate manually to save costs
6. **Enable backup** - Use Azure Backup for production VMs
7. **Use Standard SKU public IPs** - Better availability for production workloads
8. **Limit RDP access** - Use Just-In-Time access, Azure Bastion, or VPN instead of public RDP
9. **Monitor your VMs** - Enable Azure Monitor and set up alerts for CPU, memory, and disk
10. **Tag resources** - For better organization and cost tracking
11. **Use availability sets or zones** - For high availability in production
12. **Enable boot diagnostics** - Essential for troubleshooting startup issues
13. **Regular snapshots** - Create snapshots before major changes
14. **Use Premium SSD** - For production workloads requiring better performance

## Clean Up Resources
```powershell
# Delete everything in the resource group
Remove-AzResourceGroup -Name "myResourceGroup" -Force
```

## Additional Resources
- [Azure VM Documentation](https://docs.microsoft.com/azure/virtual-machines/)
- [Azure PowerShell VM Commands](https://docs.microsoft.com/powershell/module/az.compute/)
- [Windows VM Sizes](https://docs.microsoft.com/azure/virtual-machines/sizes)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
- [Azure Update Management](https://docs.microsoft.com/azure/automation/update-management/overview)
