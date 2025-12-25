# Azure Cosmos DB – Resource Model Reference

This document serves as a quick, handy reference for understanding the **Azure Cosmos DB Resource Model**, including its hierarchy, key components, and design considerations.

---

## 1. Cosmos DB Account

The **Cosmos DB account** is the top-level Azure resource and the entry point for all Cosmos DB operations.

### Key Characteristics
- Globally distributed, multi-model database service
- Supports a **single API per account**
- Hosts one or more databases
- Configured with replication, consistency, security, and networking settings

### Account-Level Configuration
- API type (SQL/Core, MongoDB, Cassandra, Table, Gremlin)
- Global distribution (read/write regions)
- Consistency levels:
  - Strong
  - Bounded Staleness
  - Session
  - Consistent Prefix
  - Eventual
- Backup policy (Periodic or Continuous)
- Security (Primary/Secondary Keys, RBAC, Managed Identity)
- Networking (Firewall, VNET, Private Endpoints)

---

## 2. Database

A **database** is a logical namespace for organizing containers.

### Characteristics
- Exists within a Cosmos DB account
- Contains one or more containers
- Can be configured with shared or dedicated throughput

### Throughput Options
| Throughput Type | Description |
|-----------------|-------------|
| Shared          | RU/s shared across all containers |
| Dedicated       | RU/s reserved for the database |

---

## 3. Container

The **container** is the fundamental unit of scalability and performance.

> Naming varies by API:
- SQL API → Container  
- MongoDB API → Collection  
- Cassandra API → Table  
- Table API → Table  
- Gremlin API → Graph  

### Container Features
- Horizontally scalable
- Automatically partitioned
- Stores items (documents, rows, or nodes)

### Key Settings
- Partition key (required)
- Throughput (Manual or Autoscale RU/s)
- Indexing policy
- Time To Live (TTL)
- Unique key constraints

---

## 4. Partition Key

The **partition key** determines how data is distributed across logical and physical partitions.

### Importance
- Drives scalability and performance
- Affects cost and query efficiency
- Used to route requests efficiently

### Best Practices
- Choose a high-cardinality value
- Ensure even data distribution
- Include in most queries

---

## 5. Logical Partition

- All items sharing the same partition key value
- Maximum size: **20 GB**
- Can span one or more physical partitions

---

## 6. Physical Partition

- System-managed storage unit
- Handles storage and throughput
- Maximum size: **50 GB**
- RU/s is distributed across physical partitions

> Physical partitions are fully managed by Azure Cosmos DB.

---

## 7. Item

The **item** is the actual data entity stored in a container.

### Characteristics
- Schema-agnostic
- JSON-based for SQL API
- Identified by:
  - `id`
  - Partition key value

### Example Item (SQL API)
```json
{
  "id": "123",
  "userId": "u001",
  "name": "Alice",
  "email": "alice@example.com"
}
```

## 8. Throughput (RU/s)
Request Units per second (RU/s) represent the cost of operations in Cosmos DB.
Provisioning Models
| Model     | Description                                   |
| --------- | --------------------------------------------- |
| Manual    | Fixed RU/s                                    |
| Autoscale | Automatically scales up to a defined max RU/s |

Throughput Scope

Database-level throughput

Container-level throughput

## 9. Indexing

    Enabled by default
    Automatically indexes all properties
    Configurable per container

Indexing Modes
    Consistent
    Lazy
    None

## 10 Resource hierarchy

Cosmos DB Account
 └── Database
     └── Container
         └── Logical Partition
             └── Item


## 11 API Resource Mapping

| API        | Container Equivalent | Item Equivalent |
| ---------- | -------------------- | --------------- |
| SQL (Core) | Container            | Item (JSON)     |
| MongoDB    | Collection           | Document        |
| Cassandra  | Table                | Row             |
| Table API  | Table                | Entity          |
| Gremlin    | Graph                | Vertex / Edge   |

## 12 Design Consideration:

Select partition keys carefully
Prefer container-level throughput for workload isolation
Use autoscale for unpredictable workloads
Keep related data in the same logical partition
Plan for global distribution from day one

## 13 Summary
Cosmos DB uses a hierarchical resource model
Containers are the main scalability unit
Partitioning directly impacts performance and cost
RU/s governs throughput and billing
The model is consistent across all supported APIs