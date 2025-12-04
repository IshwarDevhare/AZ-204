# Prerequisites for Connecting to Linux VM in Azure

## Overview
Before connecting to a Linux VM in Azure, ensure you have the following prerequisites in place.

## 1. Azure Account & Subscription
- Active Azure subscription
- Appropriate permissions to create and manage VMs
- Resource group created for organizing resources

## 2. Linux VM Requirements
- Linux VM must be created and running
- VM must have a public IP address (for external access) or be accessible via VPN/ExpressRoute
- Network Security Group (NSG) configured to allow SSH traffic on port 22

## 3. Authentication Methods

### SSH Key Pair (Recommended)
- **Public Key**: Uploaded to Azure during VM creation or added later
- **Private Key**: Stored securely on your local machine (typically in `~/.ssh/`)
- Default location: `~/.ssh/id_rsa` (private) and `~/.ssh/id_rsa.pub` (public)
- Key permissions: Private key should have `chmod 600` permissions

### Username & Password
- Username created during VM provisioning
- Strong password (if password authentication is enabled)
- Note: SSH key authentication is more secure and recommended over password authentication

## 4. Network Configuration
- **Public IP Address**: Required for direct internet access
- **DNS Name** (optional): For easier connection
- **NSG Rules**: 
  - Inbound rule allowing TCP port 22 (SSH)
  - Source IP can be restricted to your IP for added security
- **Virtual Network**: VM must be in a VNet with proper subnet configuration

## 5. Azure Portal Configuration

### Network Security Group (NSG) Rules
When configuring via Azure Portal, ensure the following:

#### Inbound Security Rules:
1. Navigate to: **VM → Networking → Inbound port rules**
2. **Required Rule for SSH**:
   - **Name**: Allow-SSH or similar descriptive name
   - **Priority**: 300-1000 (lower number = higher priority)
   - **Port**: 22
   - **Protocol**: TCP
   - **Source**: 
     - `Any` (not recommended for production)
     - `IP Addresses` (specify your public IP for better security)
     - `Service Tag` (e.g., Internet, VirtualNetwork)
   - **Destination**: Any or specific VM IP
   - **Action**: Allow

#### Outbound Security Rules:
1. Navigate to: **VM → Networking → Outbound port rules**
2. **Default Rules** (usually sufficient):
   - Allow outbound to VirtualNetwork
   - Allow outbound to Internet
   - Allow outbound to AzureLoadBalancer
3. **Custom Rules** (if needed):
   - Restrict specific outbound ports for security compliance
   - Allow specific destinations only

### Public IP Configuration:
1. Navigate to: **VM → Networking → Network Interface**
2. Select the **IP configuration**
3. Ensure **Public IP address** is:
   - **Enabled**
   - **Static** (recommended) or Dynamic
   - Has a **DNS name label** (optional but helpful)

### VM Status & Access:
1. **VM must be running**:
   - Portal: **VM → Overview** - Status should show "Running"
   - If stopped: Click "Start" button
2. **Boot diagnostics** (helpful for troubleshooting):
   - Enable under: **VM → Boot diagnostics**
   - Uses storage account to capture console output

### Connection Information from Portal:
1. Navigate to: **VM → Overview** or **VM → Connect → SSH**
2. Portal provides:
   - Public IP address
   - Private IP address
   - DNS name (if configured)
   - Sample SSH command
   - Username used during creation

### Azure Bastion (Alternative Secure Connection):
1. Navigate to: **VM → Connect → Bastion**
2. If not configured:
   - Click "Deploy Bastion"
   - Requires subnet named "AzureBastionSubnet"
   - Provides browser-based SSH without public IP
   - More secure but requires additional cost

### Identity & Access Management (IAM):
1. Navigate to: **VM → Access control (IAM)**
2. Required roles:
   - **Virtual Machine Contributor**: Manage VMs
   - **Virtual Machine Administrator Login**: SSH with Azure AD credentials
   - **Virtual Machine User Login**: SSH with limited privileges

### Disk & Storage:
1. Navigate to: **VM → Disks**
2. Ensure:
   - OS disk is attached and healthy
   - Encryption settings are configured if required

### Extensions & Applications:
1. Navigate to: **VM → Extensions + applications**
2. Useful extensions:
   - **Azure Monitor Agent**: For monitoring
   - **Custom Script Extension**: Run scripts on VM
   - **Network Watcher**: For connectivity diagnostics

### Resource Group Settings:
1. Verify VM is in correct resource group
2. Check resource group location matches VM region
3. Review tags for organization and cost tracking

## 6. Client Tools

### For Linux/macOS:
- **SSH client**: Pre-installed on most distributions
- Command: `ssh -i /path/to/private-key username@vm-ip-address`

### For Windows:
- **Windows 10/11**: OpenSSH client (built-in)
- **Alternative Tools**:
  - PuTTY (free SSH client)
  - Windows Terminal
  - Azure Cloud Shell
  - VS Code with Remote-SSH extension

## 7. Azure CLI (Optional but Recommended)
- Install Azure CLI for managing VMs
- Login: `az login`
- Useful for getting VM details, starting/stopping VMs, etc.

## 8. Firewall Configuration
- Local firewall must allow outbound connections on port 22
- Corporate networks may require VPN or proxy configuration

## 9. Connection Information Needed
Before connecting, gather the following:
- VM's public IP address or DNS name
- Username (typically set during VM creation)
- Path to SSH private key OR password
- SSH port (default: 22, unless customized)

## 10. Security Best Practices
- Use SSH keys instead of passwords
- Disable password authentication after SSH key setup
- Restrict NSG rules to specific IP addresses
- Use Azure Bastion for enhanced security (eliminates need for public IP)
- Keep private keys secure and never share them
- Regularly update and patch the Linux VM

## Quick Connection Commands

### Using SSH Key:
```bash
ssh -i ~/.ssh/id_rsa azureuser@<VM_PUBLIC_IP>
```

### Using Password:
```bash
ssh azureuser@<VM_PUBLIC_IP>
```

### Using Azure CLI:
```bash
# Get VM public IP
az vm list-ip-addresses --resource-group <RESOURCE_GROUP> --name <VM_NAME> --output table

# SSH using Azure CLI (automatically handles authentication)
az ssh vm --resource-group <RESOURCE_GROUP> --name <VM_NAME>
```

## Troubleshooting Common Issues
- **Connection timeout**: Check NSG rules and VM status
- **Permission denied**: Verify username and authentication method
- **Host key verification failed**: Remove old entry from `~/.ssh/known_hosts`
- **Network unreachable**: Verify VM has public IP and is running

## Additional Resources
- [Azure VM Documentation](https://docs.microsoft.com/azure/virtual-machines/)
- [SSH Key Management](https://docs.microsoft.com/azure/virtual-machines/linux/mac-create-ssh-keys)
- [Azure Bastion](https://docs.microsoft.com/azure/bastion/bastion-overview)
