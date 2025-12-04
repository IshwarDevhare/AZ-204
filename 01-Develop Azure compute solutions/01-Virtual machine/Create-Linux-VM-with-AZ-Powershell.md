# Create Linux VM with Azure PowerShell

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
# Create credential object for Linux VM
$securePassword = ConvertTo-SecureString 'P@ssw0rd123!' -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential ('azureuser', $securePassword)
```

### 3. Create Linux VM
```powershell
New-AzVM `
  -ResourceGroupName "myResourceGroup" `
  -Name "myLinuxVM" `
  -Location "EastUS" `
  -Image "Ubuntu2204" `
  -Size "Standard_B2s" `
  -Credential $cred `
  -OpenPorts 22 `
  -PublicIpAddressName "myPublicIP"
```

### 4. Get Public IP Address
```powershell
Get-AzPublicIpAddress `
  -ResourceGroupName "myResourceGroup" `
  -Name "myPublicIP" | Select-Object IpAddress
```

### 5. Connect via SSH
```powershell
# Get the public IP
$publicIp = (Get-AzPublicIpAddress -ResourceGroupName "myResourceGroup" -Name "myPublicIP").IpAddress
# Connect
ssh azureuser@$publicIp
```

## Common VM Operations

### Start VM
```powershell
Start-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM"
```

### Stop VM (deallocate to stop billing)
```powershell
Stop-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM" -Force
```

### Restart VM
```powershell
Restart-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM"
```

### Get VM Status
```powershell
Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM" -Status
```

### Delete VM
```powershell
Remove-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM" -Force
```

### Delete Resource Group (deletes all resources)
```powershell
Remove-AzResourceGroup -Name "myResourceGroup" -Force
```

## Advanced VM Creation with More Options

### Create VM with Custom Network Settings
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

# Create a network security group rule for SSH
$nsgRuleSSH = New-AzNetworkSecurityRuleConfig `
  -Name "myNSGRuleSSH" `
  -Protocol Tcp `
  -Direction Inbound `
  -Priority 1000 `
  -SourceAddressPrefix * `
  -SourcePortRange * `
  -DestinationAddressPrefix * `
  -DestinationPortRange 22 `
  -Access Allow

# Create a network security group
$nsg = New-AzNetworkSecurityGroup `
  -ResourceGroupName "myResourceGroup" `
  -Location "EastUS" `
  -Name "myNSG" `
  -SecurityRules $nsgRuleSSH

# Create a virtual network interface card
$nic = New-AzNetworkInterface `
  -Name "myNIC" `
  -ResourceGroupName "myResourceGroup" `
  -Location "EastUS" `
  -SubnetId $vnet.Subnets[0].Id `
  -PublicIpAddressId $pip.Id `
  -NetworkSecurityGroupId $nsg.Id

# Create VM configuration
$vmConfig = New-AzVMConfig -VMName "myLinuxVM" -VMSize "Standard_B2s" | `
  Set-AzVMOperatingSystem -Linux -ComputerName "myLinuxVM" -Credential $cred | `
  Set-AzVMSourceImage -PublisherName "Canonical" -Offer "0001-com-ubuntu-server-jammy" -Skus "22_04-lts-gen2" -Version "latest" | `
  Add-AzVMNetworkInterface -Id $nic.Id

# Create the VM
New-AzVM -ResourceGroupName "myResourceGroup" -Location "EastUS" -VM $vmConfig
```

## Popular Linux Images

### List Available Images
```powershell
# List publishers
Get-AzVMImagePublisher -Location "EastUS" | Where-Object {$_.PublisherName -like "*Canonical*" -or $_.PublisherName -like "*RedHat*"}

# List offers for Canonical
Get-AzVMImageOffer -Location "EastUS" -PublisherName "Canonical"

# List SKUs for Ubuntu
Get-AzVMImageSku -Location "EastUS" -PublisherName "Canonical" -Offer "0001-com-ubuntu-server-jammy"
```

### Common Image URNs
```powershell
# Ubuntu 22.04 LTS
-Image "Ubuntu2204"
# Or detailed:
-PublisherName "Canonical" -Offer "0001-com-ubuntu-server-jammy" -Skus "22_04-lts-gen2"

# Ubuntu 20.04 LTS
-Image "Ubuntu2004"
# Or detailed:
-PublisherName "Canonical" -Offer "0001-com-ubuntu-server-focal" -Skus "20_04-lts-gen2"

# Debian 11
-PublisherName "Debian" -Offer "debian-11" -Skus "11-gen2"

# CentOS 7
-PublisherName "OpenLogic" -Offer "CentOS" -Skus "7.5"

# Red Hat Enterprise Linux 8
-PublisherName "RedHat" -Offer "RHEL" -Skus "8-lvm-gen2"
```

## Common VM Sizes
```powershell
# Burstable (Dev/Test)
-Size "Standard_B1s"    # 1 vCPU, 1 GB RAM
-Size "Standard_B2s"    # 2 vCPU, 4 GB RAM

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
Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM"
```

### Get VM with instance view (detailed status)
```powershell
Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM" -Status | Format-List
```

### List all resource groups
```powershell
Get-AzResourceGroup | Format-Table ResourceGroupName, Location
```

### Run command on VM (without SSH)
```powershell
# Run a shell script
Invoke-AzVMRunCommand `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myLinuxVM" `
  -CommandId "RunShellScript" `
  -ScriptString "echo 'Hello World'"

# Install software
Invoke-AzVMRunCommand `
  -ResourceGroupName "myResourceGroup" `
  -VMName "myLinuxVM" `
  -CommandId "RunShellScript" `
  -ScriptString "sudo apt-get update && sudo apt-get install -y nginx"
```

### Resize VM
```powershell
# Stop the VM
Stop-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM" -Force

# Resize the VM
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM"
$vm.HardwareProfile.VmSize = "Standard_D2s_v3"
Update-AzVM -VM $vm -ResourceGroupName "myResourceGroup"

# Start the VM
Start-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM"
```

### Add Data Disk to VM
```powershell
# Create a new managed disk
$diskConfig = New-AzDiskConfig -SkuName Premium_LRS -Location "EastUS" -CreateOption Empty -DiskSizeGB 128
$dataDisk = New-AzDisk -DiskName "myDataDisk" -Disk $diskConfig -ResourceGroupName "myResourceGroup"

# Attach disk to VM
$vm = Get-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM"
$vm = Add-AzVMDataDisk -VM $vm -Name "myDataDisk" -CreateOption Attach -ManagedDiskId $dataDisk.Id -Lun 0
Update-AzVM -ResourceGroupName "myResourceGroup" -VM $vm
```

## SSH Key Authentication (Recommended for Production)

### Generate SSH Key Pair
```powershell
# On Windows with OpenSSH
ssh-keygen -t rsa -b 4096 -f $env:USERPROFILE\.ssh\azure_vm_key
```

### Create VM with SSH Key
```powershell
# Read the public key
$sshPublicKey = Get-Content "$env:USERPROFILE\.ssh\azure_vm_key.pub"

# Create VM configuration with SSH key
$vmConfig = New-AzVMConfig -VMName "myLinuxVM" -VMSize "Standard_B2s" | `
  Set-AzVMOperatingSystem -Linux -ComputerName "myLinuxVM" -DisablePasswordAuthentication | `
  Set-AzVMSourceImage -PublisherName "Canonical" -Offer "0001-com-ubuntu-server-jammy" -Skus "22_04-lts-gen2" -Version "latest" | `
  Add-AzVMNetworkInterface -Id $nic.Id | `
  Add-AzVMSshPublicKey -KeyData $sshPublicKey -Path "/home/azureuser/.ssh/authorized_keys"

# Create the VM
New-AzVM -ResourceGroupName "myResourceGroup" -Location "EastUS" -VM $vmConfig
```

## Tips and Best Practices

1. **Always use `-Force` parameter** when stopping or deleting resources to avoid confirmation prompts in automation
2. **Use Standard SKU for public IPs** for production workloads (better availability)
3. **Stop (deallocate) VMs** when not in use to avoid compute charges
4. **Use SSH keys** instead of passwords for better security
5. **Tag resources** for better organization and cost tracking:
   ```powershell
   New-AzVM -ResourceGroupName "myResourceGroup" -Name "myLinuxVM" -Tag @{Environment="Dev"; Project="AZ-204"}
   ```
6. **Use managed disks** (default) for better reliability and management
7. **Enable boot diagnostics** for troubleshooting:
   ```powershell
   Set-AzVMBootDiagnostic -VM $vm -Enable -ResourceGroupName "myResourceGroup" -StorageAccountName "mystorageaccount"
   ```

## Clean Up Resources
```powershell
# Delete everything in the resource group
Remove-AzResourceGroup -Name "myResourceGroup" -Force
```
