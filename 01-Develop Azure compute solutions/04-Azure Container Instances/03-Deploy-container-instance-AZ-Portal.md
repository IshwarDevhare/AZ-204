# Deploy Container Instance using Azure Portal

## Prerequisites
- Active Azure subscription
- Access to Azure Portal

## Steps

1. **Sign in to Azure Portal**
    - Navigate to [portal.azure.com](https://portal.azure.com)
    - Sign in with your Azure credentials

2. **Create Container Instance**
    - Click **"Create a resource"**
    - Search for **"Container Instances"**
    - Select **"Container Instances"** and click **"Create"**

3. **Configure Basic Settings**
    - **Subscription**: Select your subscription
    - **Resource Group**: Create new or select existing
    - **Container Name**: Enter a unique name
    - **Region**: Choose deployment region
    - **Image Source**: Select "Quick start images" or "Other registry"
    - **Image**: Specify container image (e.g., `mcr.microsoft.com/azuredocs/aci-helloworld`)

4. **Configure Networking**
    - **DNS Name Label**: Enter unique DNS label (optional)
    - **Ports**: Configure port mappings
    - **Protocol**: Select TCP or UDP

5. **Advanced Settings** (Optional)
    - **Restart Policy**: Always, Never, or OnFailure
    - **Environment Variables**: Add if needed
    - **Command Override**: Specify custom commands

6. **Review and Create**
    - Review all configurations
    - Click **"Create"** to deploy

7. **Verify Deployment**
    - Navigate to the container instance
    - Check status and get public IP/FQDN
    - Test application accessibility

## Next Steps
- Monitor container logs
- Scale or update as needed
- Configure custom domains if required