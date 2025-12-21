# Azure Cosmos DB – Overview

## What is Azure Cosmos DB?
**Azure Cosmos DB** is a fully managed, globally distributed **NoSQL database service** provided by Microsoft Azure. It is designed to deliver **high availability, low latency, scalability, and flexibility** for modern applications.

Cosmos DB is commonly used for applications that need to handle large amounts of data, users across the world, and fast response times.

---

## Key Characteristics

### 1. Fully Managed Service
- Microsoft handles infrastructure, updates, backups, and scaling
- No need to manage servers or database software

### 2. Global Distribution
- Data can be replicated across multiple Azure regions
- Applications can read and write data close to users for low latency

### 3. High Availability
- Built-in fault tolerance
- Supports automatic failover between regions

### 4. Elastic Scalability
- Scale storage and throughput independently
- Handles sudden traffic spikes without downtime

---

## Data Models and APIs

Azure Cosmos DB supports multiple APIs, allowing developers to use different data models:

| API | Data Model | Common Use Case |
|---|---|---|
| Core (SQL) API | JSON documents | Web & mobile apps |
| MongoDB API | Document-based | Apps using MongoDB |
| Cassandra API | Wide-column | High-write workloads |
| Table API | Key-value | Simple lookup data |
| Gremlin API | Graph | Social networks, recommendations |

---

## Consistency Models

Cosmos DB offers **five consistency levels**, allowing developers to choose between performance and data accuracy:

1. **Strong** – Highest consistency, lowest latency flexibility
2. **Bounded Staleness**
3. **Session** (most commonly used)
4. **Consistent Prefix**
5. **Eventual** – Highest performance, least strict consistency

---

## Throughput and Performance

- Uses **Request Units (RU/s)** to measure performance
- RU/s represents the cost of database operations
- Throughput can be:
  - Manually provisioned
  - Automatically scaled

---

## Security Features

- Data encryption at rest and in transit
- Role-based access control (RBAC)
- Integration with Azure Active Directory
- Network isolation using private endpoints

---

## Common Use Cases

- Web and mobile applications
- Real-time analytics
- IoT and telemetry data
- Gaming leaderboards
- E-commerce catalogs
- Chat and messaging systems

---

## Advantages of Azure Cosmos DB

- Low-latency global access
- Multiple APIs in one service
- High reliability and availability
- Easy scalability
- Strong enterprise security

---

## Limitations

- Can be expensive if not optimized
- Requires understanding of partitioning and RU/s
- Best suited for NoSQL workloads (not traditional relational joins)

---

## When to Use Azure Cosmos DB

Use Azure Cosmos DB when:
- You need global users with fast response times
- Your application requires flexible schemas
- You expect high traffic or rapid growth
- You want a fully managed NoSQL database

---

## Summary

Azure Cosmos DB is a powerful, cloud-native NoSQL database service that supports multiple data models, global distribution, and flexible scalability, making it ideal for modern, large-scale applications.

---
