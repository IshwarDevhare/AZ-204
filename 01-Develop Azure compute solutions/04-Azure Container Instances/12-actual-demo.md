# Azure Container Instances Demo

## Clone the Sample Application

Clone the Microsoft aci-helloworld sample repository:

```bash
git clone https://github.com/Azure-Samples/aci-helloworld.git
cd aci-helloworld
```

## Build and Test Docker Image Locally

### Build the Docker Image

Build the Docker image from the cloned repository:

```bash
docker build aci-helloworld -t mmdemo0101
```

### Run and Test the Container

Start the container and map port 8080 on your host to port 80 in the container:

```bash
docker run -d -p 8080:80 mmdemo0101
```

### Verify the Application

Test the application locally in your browser:

```
http://localhost:8080/
```

You should see the aci-helloworld welcome page.

## Create Azure Resources

### Create a Resource Group

```bash
az group create -n mmdemo0101 --location centralindia
```

### Create an Azure Container Registry

```bash
az acr create -g mmdemo0101 -n mmdemo0101 --sku Basic
```

### Login to ACR

```bash
az acr login -n mmdemo0101
```

## Push Image to Azure Container Registry

### View Local Docker Images

```bash
docker images
```

### Tag the Image for ACR

```bash
docker tag mmdemo0101 mmdemo0101.azurecr.io/mmdemo0101:v1
```

### Push the Image to ACR

```bash
docker push mmdemo0101.azurecr.io/mmdemo0101:v1
```

## Enable Admin Access and Get Credentials

### Enable Admin Account on ACR

```bash
az acr update -n mmdemo0101 --admin-enabled true
```

### Get ACR Credentials

```bash
az acr credential show -n mmdemo0101
```

**Note:** Save the username and password from the output above. You will need these credentials for deploying to ACI.

## Create Container Instance

### Deploy to Azure Container Instances

Deploy the container image from ACR to ACI using the credentials obtained above:

```bash
az container create \
  -g mmdemo0101 \
  -n mmdemo0101 \
  --image mmdemo0101.azurecr.io/mmdemo0101:v1 \
  --cpu 1 \
  --memory 1 \
  --registry-login-server mmdemo0101.azurecr.io \
  --registry-username {replace_username} \
  --registry-password {replace_user_password} \
  --ip-address Public \
  --dns-name-label mmdemo0101 \
  --ports 80 \
  --os-type Linux
```

**Important:** Replace `{replace_username}` and `{replace_user_password}` with the actual credentials from the previous step.

### Verify Deployment

Once the container is created, get its public IP address:

```bash
az container show -g mmdemo0101 -n mmdemo0101 --query ipAddress.fqdn
```

### Access the Application

Open your browser and navigate to the application using the IP address or FQDN:

```
http://mmdemo0101.centralindia.azurecontainer.io
```

Verify that everything is working as expected.

## Clean Up Resources

Delete the resource group and all resources within it:

```bash
az group delete -n mmdemo0101
```
