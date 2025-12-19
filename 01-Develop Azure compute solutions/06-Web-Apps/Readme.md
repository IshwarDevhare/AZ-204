# Azure App Service Web Apps

## Overview

Azure App Service is a fully managed platform for deploying and hosting web applications in the cloud. This module covers essential topics for deploying, managing, securing, and optimizing web apps using Azure App Service.

## Course Topics

### 1. **Deploying Web Apps to Azure App Service**

#### ZIP Package Deployment
- Deploy applications as compressed packages directly to App Service
- Simple and fast deployment method
- No build process on server required
- Supports all App Service platforms (Windows, Linux)

**Use Cases:**
- Quick deployments during development
- CI/CD pipelines with pre-built packages
- Automated deployment scripts

#### Azure Pipelines Integration
- Automated build and deployment pipelines
- Continuous Integration/Continuous Deployment (CI/CD)
- Support for multiple programming languages
- Integration with GitHub, Azure DevOps repositories
- Automated testing and validation before deployment

**Benefits:**
- Reduced manual deployment errors
- Faster release cycles
- Automated quality gates
- Rollback capabilities

### 2. **Source Control Integration**

#### GitHub Synchronization
- Connect App Service directly to GitHub repositories
- Automatic deployment on code push
- Webhook-based deployment triggers
- Support for branch-based deployments
- One-click setup and configuration

**Workflow:**
- Push code to GitHub → Automatic build and deploy
- Deploy from specific branches
- Environmental configurations per branch

#### Microservices Deployment
- Deploy multiple independent services to App Service
- Each service runs in isolated app instance
- Separate scaling and configuration per service
- Service discovery and communication
- Independent versioning and updates

### 3. **Authentication and Authorization**

#### Managed Identities
- Azure-managed credentials for your applications
- No need to store secrets in configuration
- Two types: System-assigned and User-assigned
- Automatic credential rotation by Azure

**Use Cases:**
- Access to Azure SQL Database
- Connect to Key Vault for secrets
- Call other Azure services securely
- Eliminate hardcoded credentials

#### App Authentication
- Built-in authentication providers
- Support for: Azure AD, Microsoft Account, Google, Facebook, Twitter
- Session management and token handling
- Easy integration without custom code

**Features:**
- Single sign-on (SSO)
- Multi-factor authentication support
- User identity available to application
- Automatic token refresh

### 4. **Custom Domain Names and SSL/TLS Certificates**

#### Custom Domain Configuration
- Map custom domains to App Service
- Support for CNAME and A record configuration
- Domain validation and verification
- Multiple domains per App Service

**Steps:**
1. Register/own domain
2. Create DNS record (CNAME or A record)
3. Configure domain in App Service
4. Verify ownership

#### Digital Security Certificates
- Free managed certificates (Azure-managed domains)
- Bring-your-own certificates
- Certificate auto-renewal
- TLS/SSL encryption for data in transit
- Support for wildcard and multi-domain certificates

**Security Benefits:**
- HTTPS enforcement
- Browser trust indicators
- PCI-DSS compliance
- Encrypted communication

### 5. **Application Performance Optimization**

#### Autoscale in Azure
- Automatically adjust instances based on demand
- Metrics-based scaling (CPU, Memory, Request count)
- Time-based scaling for predictable patterns
- Scheduled scaling for known traffic spikes

**Autoscale Rules:**
- Scale-out when metrics exceed threshold
- Scale-in when metrics drop
- Minimum and maximum instance limits
- Cool-down period between scaling actions

**Benefits:**
- Handle traffic spikes automatically
- Reduce costs during low-traffic periods
- Improved application responsiveness
- High availability with multiple instances

#### Performance Tuning
- Content Delivery Network (CDN) integration
- Caching strategies
- Compression of responses
- HTTP/2 and HTTP/2 Server Push
- Application Insights for monitoring
- Database query optimization
- Asynchronous processing for long-running tasks

**Performance Metrics:**
- Response time
- Requests per second
- Error rates
- Resource utilization (CPU, Memory, Disk)

### 6. **Key Features and Capabilities**

| Feature | Details |
|---------|---------|
| **Deployment** | ZIP, Git push, Azure Pipelines, Docker, FTP |
| **Languages** | .NET, Node.js, Python, PHP, Java, Ruby |
| **Staging Slots** | Test before production, zero-downtime swaps |
| **Backup & Restore** | Automated daily backups (Standard+), restore to point-in-time |
| **Monitoring** | Application Insights, metrics, logging, alerts |
| **Networking** | VNet integration, Hybrid connections, Private endpoints |
| **Security** | SSL/TLS, authentication, authorization, firewall rules |
| **Scaling** | Auto-scale, manual scaling, reserved instances |
| **CI/CD** | GitHub, Azure DevOps, Bitbucket, GitLab integration |

## Deployment Methods Comparison

| Method | Speed | Automation | Ideal For |
|--------|-------|-----------|----------|
| **ZIP Package** | Fast | Manual | Development, testing |
| **Azure Pipelines** | Automatic | High | Production, CI/CD |
| **GitHub Sync** | Automatic | High | Continuous deployment |
| **FTP/FTPS** | Slow | Manual | Legacy workflows |
| **Docker** | Medium | High | Containerized apps |

## Security Best Practices

✅ **Enable HTTPS** - Enforce SSL/TLS for all traffic

✅ **Use Managed Identities** - Avoid storing secrets in code

✅ **Enable Authentication** - Use built-in auth providers

✅ **Network Security** - VNet integration, NSG rules

✅ **Monitoring** - Application Insights, alerts on anomalies

✅ **Secrets Management** - Use Azure Key Vault

✅ **Regular Backups** - Enable automatic backups

✅ **Update Dependencies** - Keep frameworks and libraries current

## Performance Optimization Tips

1. **Enable Compression** - Reduce data transfer size
2. **Use CDN** - Serve static content from edge locations
3. **Optimize Database** - Index queries, connection pooling
4. **Async Processing** - Use background jobs for long operations
5. **Caching** - Redis cache for frequently accessed data
6. **Staging Slots** - Test performance before production swap
7. **Monitor Metrics** - Track CPU, memory, response times
8. **Configure Autoscale** - Handle traffic spikes automatically

## Scaling Strategy

### When to Scale Out (Horizontal)
- High concurrent user load
- Stateless applications
- Need high availability
- Sudden traffic spikes

### When to Scale Up (Vertical)
- CPU or memory bottleneck
- Need to upgrade compute resources
- Single large request processing
- Database-intensive operations

### Autoscale Configuration
```
Condition: Average CPU > 70%
Action: Add 1 instance (up to 5 max)
Cool-down: 5 minutes

Condition: Average CPU < 30%
Action: Remove 1 instance (down to 2 min)
Cool-down: 5 minutes
```

## Learning Path

1. **Foundational** - Understand App Service concepts and deployment
2. **Deployment** - Master ZIP and Pipeline deployments
3. **Integration** - Learn GitHub sync and CI/CD
4. **Security** - Implement authentication and HTTPS
5. **Performance** - Configure autoscaling and optimization
6. **Advanced** - Managed identities, custom domains, certificates

## Key Takeaways

- ✓ Multiple deployment methods for different scenarios
- ✓ Built-in authentication eliminates custom code
- ✓ Autoscaling handles variable traffic automatically
- ✓ Managed identities provide secure Azure service access
- ✓ Custom domains and SSL certificates required for production
- ✓ Monitoring and metrics critical for optimization
- ✓ Staging slots enable safe production updates
- ✓ Backup and restore for disaster recovery
