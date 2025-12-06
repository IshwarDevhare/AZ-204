# OCI Artifacts Demo

## Step 1: Login to Azure Container Registry

Get an access token and login using ORAS:

```powershell
# Get ACR access token
$password = az acr login -n deployment1 --expose-token --output tsv --query accessToken

# Login to registry using ORAS
oras login deployment1.azurecr.io --password $password
```

## Step 2: Push Simple Artifact

Create a simple readme file and push it to ACR:

```powershell
# Create the simple readme file/artifact
echo "My ReadMe" > oci-push-readme.md

# Push artifact to ACR
oras push deployment1.azurecr.io/samples/artifact:readme --artifact-type readme/example .\oci-push-readme.md
```

## Step 3: View Artifact Manifest

Fetch and view the artifact manifest:

```powershell
oras manifest fetch --pretty deployment1.azurecr.io/samples/artifact:readme
```

## Step 4: Download Artifact

Pull the artifact from ACR to a local directory:

```powershell
oras pull -o .\07-download\ deployment1.azurecr.io/samples/artifact:readme
```