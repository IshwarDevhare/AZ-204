# Create Linux VM with Azure CLI

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

### 2. Create Linux VM
```bash
az vm create \
  --resource-group myResourceGroup \
  --name myLinuxVM \
  --image Ubuntu2204 \
  --size Standard_B2s \
  --admin-username azureuser \
  --generate-ssh-keys \
  --public-ip-sku Standard
```

### 3. Open Port 22 (SSH)
```bash
az vm open-port \
  --resource-group myResourceGroup \
  --name myLinuxVM \
  --port 22
```

### 4. Get Public IP Address
```bash
az vm show \
  --resource-group myResourceGroup \
  --name myLinuxVM \
  --show-details \
  --query publicIps \
  --output tsv
```

### 5. Connect via SSH
```bash
ssh azureuser@<PUBLIC_IP>
```

## Common VM Operations

### Start VM
```bash
az vm start --resource-group myResourceGroup --name myLinuxVM
```

### Stop VM (deallocate to stop billing)
```bash
az vm deallocate --resource-group myResourceGroup --name myLinuxVM
```

### Restart VM
```bash
az vm restart --resource-group myResourceGroup --name myLinuxVM
```

### Delete VM
```bash
az vm delete --resource-group myResourceGroup --name myLinuxVM --yes
```

### Delete Resource Group (deletes all resources)
```bash
az group delete --name myResourceGroup --yes --no-wait
```

## Popular Linux Images
```bash
# Ubuntu 22.04 LTS
--image Ubuntu2204

# Ubuntu 20.04 LTS
--image Ubuntu2004

# Debian 11
--image Debian11

# CentOS 7
--image CentOS85Gen2

# Red Hat Enterprise Linux 8
--image RHEL8
```

## Common VM Sizes
```bash
# Burstable (Dev/Test)
--size Standard_B1s    # 1 vCPU, 1 GB RAM
--size Standard_B2s    # 2 vCPU, 4 GB RAM

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
az vm show --resource-group myResourceGroup --name myLinuxVM
```

### Get VM status
```bash
az vm get-instance-view \
  --resource-group myResourceGroup \
  --name myLinuxVM \
  --query instanceView.statuses[1].displayStatus
```

### List available images
```bash
az vm image list --output table
```

### Run command on VM (without SSH)
```bash
# Run a shell script
az vm run-command invoke \
  --resource-group myResourceGroup \
  --name myLinuxVM \
  --command-id RunShellScript \
  --scripts "echo 'Hello World'"

# Install software
az vm run-command invoke \
  --resource-group myResourceGroup \
  --name myLinuxVM \
  --command-id RunShellScript \
  --scripts "sudo apt-get update && sudo apt-get install -y nginx"

# Check disk usage
az vm run-command invoke \
  --resource-group myResourceGroup \
  --name myLinuxVM \
  --command-id RunShellScript \
  --scripts "df -h"
```

**Use cases:**
- Execute commands without SSH access
- Troubleshoot VMs when SSH is broken
- Install/configure software remotely
- Retrieve system information
- Automate configuration tasks