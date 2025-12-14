# Choosing Azure Compute Service

## Microsoft's Official Decision Flowchart

**Reference:** [Choosing an Azure compute service](https://learn.microsoft.com/en-us/azure/architecture/guide/technology-choices/compute-decision-tree)

This flowchart guides you through key decision points to select the right Azure compute service.

## Azure Compute Services Quick Reference

| Service | Best For | Model | Key Feature |
|---------|----------|-------|------------|
| **App Service** | Web apps, APIs | PaaS | Managed, built-in CI/CD |
| **Functions** | Event-driven, serverless | Serverless | Pay-per-execution |
| **Container Instances (ACI)** | Quick containers | Containers | Fast startup |
| **Kubernetes (AKS)** | Microservices at scale | Containers | Orchestration |
| **Virtual Machines** | Legacy, custom OS | IaaS | Full control |
| **Service Fabric** | Stateful microservices | PaaS | State management |
| **Batch** | Parallel processing | PaaS | Large-scale jobs |

## Azure Compute Services at a Glance

| Service | Best For | Deployment Model | Management | Scaling |
|---------|----------|------------------|-----------|---------|
| **App Service** | Web apps, APIs, mobile backends | PaaS | Managed | Automatic/Manual |
| **Functions** | Event-driven, serverless | Serverless | Fully managed | Automatic |
| **Container Instances** | Quick containers, batch jobs | Containers | Minimal | Manual |
| **Kubernetes (AKS)** | Microservices, orchestration | Containers | Managed Kubernetes | Automatic |
| **Virtual Machines** | Custom OS, legacy apps | IaaS | Self-managed | Manual |
| **Service Fabric** | Stateful services, microservices | PaaS | Managed | Automatic |
| **Batch** | Large-scale parallel jobs | PaaS | Managed | Automatic |

## Decision Flowchart Summary

### Level 1: Service Type Decision

```
Does your workload execute within a container?
├─ YES → Container Decision
├─ NO → Is it event-driven or serverless?
│       ├─ YES → Azure Functions
│       └─ NO → Web App Decision
```

### Level 2: Container Decision

```
Do you need to deploy and manage containers?
├─ YES → Do you need orchestration?
│        ├─ YES → Azure Kubernetes Service (AKS)
│        └─ NO → Azure Container Instances (ACI)
└─ NO → Can App Service host your container?
         ├─ YES → App Service (Docker)
         └─ NO → AKS
```

### Level 3: Web App Decision

```
Do you need a web application or API?
├─ YES → Azure App Service
├─ NO → Do you need stateful services?
│       ├─ YES → Service Fabric
│       └─ NO → Virtual Machines
└─ NO → Requires custom OS configuration?
         ├─ YES → Virtual Machines
         └─ NO → App Service
```

## Choosing Azure App Service

### When to Use App Service

✅ **Choose App Service if:**
- Building web applications (ASP.NET, Node.js, Python, PHP, Java, Ruby)
- Creating REST APIs or web services
- Building mobile app backends
- Need automatic scaling (Standard tier+)
- Want built-in CI/CD pipelines
- Need staging slots for testing
- Want integrated monitoring and diagnostics
- Need custom domains with SSL/TLS

### When NOT to Use App Service

❌ **Don't choose App Service if:**
- Need complete OS-level control
- Building microservices with 50+ services
- Require GPU computing
- Application doesn't run over HTTP/HTTPS
- Need stateful long-running operations
- Require custom kernel modules
- Need non-HTTP protocols

## Azure App Service vs Alternatives

### Choose App Service For:
- ✓ Web applications and websites
- ✓ REST APIs and web services
- ✓ Mobile app backends
- ✓ Server-side rendered apps
- ✓ Content management systems

### Choose Functions Instead For:
- Event-triggered workloads
- Short-lived tasks (< 10 minutes)
- Variable/unpredictable traffic
- Pay-per-execution preferred

### Choose Containers (ACI/AKS) For:
- Complex dependencies
- Non-HTTP protocols
- Microservices (10+)
- Custom OS requirements

### Choose Virtual Machines For:
- Legacy applications
- GPU acceleration
- Custom kernel configurations
- Direct hardware access

## Quick Service Comparison

| Need | App Service | Functions | Containers | VMs |
|------|-----------|-----------|-----------|-----|
| **Web App/API** | ✓✓✓ | ✓ | ✓ | ✓ |
| **Event Processing** | ✓ | ✓✓✓ | ✓ | ✗ |
| **Scheduled Jobs** | ✓ | ✓✓✓ | ✓ | ✓ |
| **Microservices** | ✗ | ✓ | ✓✓✓ | ✓ |
| **Custom OS** | ✗ | ✗ | ✓ | ✓✓✓ |
| **Managed** | ✓✓✓ | ✓✓✓ | ✓ | ✗ |

## Quick Decision Checklist

**Is it a web app or API?** → **App Service**

**Is it event-driven/serverless?** → **Functions**

**Is it containerized with 10+ services?** → **AKS**

**Does it need containers with quick setup?** → **Container Instances**

**Does it need legacy/custom OS?** → **Virtual Machines**

## References

- [Choose an Azure compute service](https://learn.microsoft.com/en-us/azure/architecture/guide/technology-choices/compute-decision-tree)
- [Azure App Service documentation](https://learn.microsoft.com/en-us/azure/app-service/)
- [Azure compute services comparison](https://learn.microsoft.com/en-us/azure/architecture/guide/technology-choices/compute-comparison)
