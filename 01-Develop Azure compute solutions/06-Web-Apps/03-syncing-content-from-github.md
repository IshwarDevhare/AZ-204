# Sync Web App Content from GitHub

## Overview

Connect your Azure App Service directly to a GitHub repository for continuous deployment. Any push to your repository automatically triggers a build and deployment.

## Prerequisites

- Azure App Service created
- GitHub repository with application code
- GitHub account with repository access
- Repository contains buildable application (.NET, Node.js, Python, etc.)

## Deployment Steps

### Step 1: Create GitHub Personal Access Token (PAT)

If using a private repository, you need a GitHub Personal Access Token.

**On GitHub:**
1. Go to Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click "Generate new token (classic)"
3. Set token name: `Azure App Service Deployment`
4. Select scopes:
   - `repo` (Full control of private repositories)
   - `admin:repo_hook` (Access to webhooks)
5. Click "Generate token"
6. **Copy the token immediately** (you won't see it again)

---

### Step 2: Configure Deployment Source

```powershell
az webapp deployment source config \
  --resource-group <resource-group> \
  --name <app-name> \
  --repo-url https://github.com/<username>/<repo-name> \
  --branch main \
  --git-token <github-pat>
```

**Example:**
```powershell
az webapp deployment source config `
  --resource-group mmdemo0101 `
  --name mmdemo0101 `
  --repo-url https://github.com/ishwardevhare/myapp `
  --branch main `
  --git-token ghp_1234567890abcdefghijklmnopqrstuvwxyz
```

**Parameters:**
- `--resource-group`: Azure resource group name
- `--name`: Web app name
- `--repo-url`: GitHub repository URL (HTTPS format)
- `--branch`: Branch to deploy from (default: main)
- `--git-token`: GitHub Personal Access Token (for private repos)

**For Public Repository (Optional Token):**
```powershell
az webapp deployment source config `
  --resource-group mmdemo0101 `
  --name mmdemo0101 `
  --repo-url https://github.com/ishwardevhare/myapp `
  --branch main
```

---

### Step 3: Verify Deployment Configuration

```powershell
az webapp deployment source show \
  --resource-group <resource-group> \
  --name <app-name>
```

**Example:**
```powershell
az webapp deployment source show `
  --resource-group mmdemo0101 `
  --name mmdemo0101
```

**Expected Response:**
```json
{
  "branch": "main",
  "deployUrl": "https://mmdemo0101.scm.azurewebsites.net/deploy",
  "isManualIntegration": false,
  "isGitHubAction": false,
  "isMercurial": false,
  "kind": "github",
  "repoUrl": "https://github.com/ishwardevhare/myapp",
  "repositoryType": "GitHub",
  "type": "GitHub"
}
```

---

### Step 4: Test Deployment

**Initial Deployment:**
The configuration automatically triggers an initial deployment from your repository.

**Monitor Deployment:**
```powershell
# View deployment history
az webapp deployment list \
  --resource-group <resource-group> \
  --name <app-name>
```

**Example:**
```powershell
az webapp deployment list `
  --resource-group mmdemo0101 `
  --name mmdemo0101
```

**View Real-Time Logs:**
```powershell
az webapp log tail \
  --resource-group <resource-group> \
  --name <app-name>
```

---

### Step 5: Trigger Automatic Deployments

**GitHub Webhook Automatic:**
- Azure automatically creates a GitHub webhook
- Any push to the specified branch triggers deployment
- No additional configuration needed

**Push to Repository:**
```bash
# Make changes to your code
git add .
git commit -m "Update application"
git push origin main
```

The deployment starts automatically within seconds.

---

### Step 6: Monitor Deployments

**View Deployment Status:**
```powershell
az webapp deployment show \
  --resource-group <resource-group> \
  --name <app-name> \
  -d <deployment-id>
```

**View Deployment Logs:**
```powershell
az webapp deployment log download \
  --resource-group <resource-group> \
  --name <app-name> \
  --log-file deployment-logs.zip
```

---

## Complete Setup Script

```powershell
# Create resource group and app service
az group create -n mmdemo0101 --location centralindia
az appservice plan create -g mmdemo0101 -n mmdemo0101 --sku B1
az webapp create -g mmdemo0101 -p mmdemo0101 -n mmdemo0101 --runtime "DOTNET|8.0"

# Configure GitHub deployment
az webapp deployment source config `
  --resource-group mmdemo0101 `
  --name mmdemo0101 `
  --repo-url https://github.com/ishwardevhare/myapp `
  --branch main `
  --git-token <your-github-pat>

# Verify configuration
az webapp deployment source show `
  --resource-group mmdemo0101 `
  --name mmdemo0101

# Check deployment status
az webapp deployment list `
  --resource-group mmdemo0101 `
  --name mmdemo0101
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **"Invalid repository URL"** | Verify GitHub URL format: `https://github.com/username/repo` |
| **"Authentication failed"** | Check GitHub PAT is valid and not expired |
| **"Webhook failed"** | Verify webhook in GitHub Settings → Webhooks |
| **"Build failed"** | Check app has buildable code (package.json, .csproj, etc.) |
| **"Deployment timeout"** | Check app size; large apps may need Standard tier or higher |
| **"Access denied"** | Ensure GitHub PAT has `repo` and `admin:repo_hook` scopes |

---

## Supported Runtimes

GitHub syncing automatically builds and deploys based on detected runtime:

| Framework | Detection | Build Tool |
|-----------|-----------|-----------|
| **.NET** | `.csproj` or `.vbproj` | dotnet CLI |
| **Node.js** | `package.json` | npm / yarn |
| **Python** | `requirements.txt` or `runtime.txt` | pip |
| **PHP** | `index.php` or `composer.json` | composer |
| **Java** | `pom.xml` | Maven |
| **Ruby** | `Gemfile` | Bundler |
| **Go** | `go.mod` | Go CLI |

---

## Disconnect GitHub Deployment

```powershell
az webapp deployment source delete \
  --resource-group <resource-group> \
  --name <app-name>
```

**Example:**
```powershell
az webapp deployment source delete `
  --resource-group mmdemo0101 `
  --name mmdemo0101
```

---

## Best Practices

✅ **Use Main/Production Branch**
- Only sync stable, tested code
- Avoid syncing from development branches

✅ **Enable Continuous Integration**
- Ensure code passes tests before pushing
- Use GitHub Actions for pre-deployment checks

✅ **Monitor Deployments**
- Check deployment history regularly
- Review logs for errors

✅ **Manage Secrets**
- Never commit secrets to repository
- Use Azure Key Vault for sensitive data
- Configure in app settings, not in code

✅ **Use Staging Slots**
- Test deployments in staging slot first
- Swap to production after verification
- Minimize downtime and risk

---

## Alternative: GitHub Actions

For more control, use **GitHub Actions** instead of direct GitHub sync:

**Advantages:**
- Run tests before deployment
- Custom build process
- Multi-stage deployments
- Better error handling

**Setup:**
```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: mmdemo0101
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: .
```

---

## Summary

**GitHub deployment is ideal for:**
- ✓ Continuous deployment workflows
- ✓ Automated builds from source
- ✓ Multi-developer teams
- ✓ Test and staging environments

**Choose ZIP deployment instead if:**
- One-time deployment required
- Binary packages need to be deployed
- Complex build process in local environment
- Faster deployment with pre-built packages needed


## Actual Demo
- sample github link

git clone https://github.com/IshwarDevhare/az-webapp-demo

cd az-webapp-demo

git branch -m main

dotnet new webapp -n mmdemo0101 -f net8.0 -o .

dotnet publish

git add -A

git commit -m "first commit"

git push origin main

