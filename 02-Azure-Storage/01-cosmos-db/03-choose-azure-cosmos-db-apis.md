# 03 – Choose Azure Cosmos DB APIs

## Overview
Azure Cosmos DB supports **multiple APIs**, allowing developers to use different data models and query languages while benefiting from the same core features such as global distribution, scalability, and high availability.

Choosing the correct API depends on:
- Application data model
- Query requirements
- Existing technology stack
- Performance and scalability needs

---

## Azure Cosmos DB APIs

### 1. Core (SQL) API
**Best for:** New applications and general-purpose use

#### Key Features
- Stores data as **JSON documents**
- Uses SQL-like query language
- Fully supported and most commonly used API
- Best integration with Azure services

#### Use Cases
- Web and mobile applications
- E-commerce catalogs
- Content management systems
- User profiles

---

### 2. Azure Cosmos DB API for MongoDB
**Best for:** Applications already using MongoDB

#### Key Features
- MongoDB-compatible wire protocol
- Supports MongoDB drivers and tools
- No need to rewrite application code
- Managed service with Cosmos DB benefits

#### Use Cases
- Migrating MongoDB workloads to Azure
- Document-based applications
- Microservices architectures

---

### 3. Azure Cosmos DB API for Cassandra
**Best for:** High-write, large-scale workloads

#### Key Features
- Wide-column data model
- Compatible with Apache Cassandra Query Language (CQL)
- Designed for massive scale and write-heavy operations

#### Use Cases
- IoT telemetry data
- Time-series data
- Logging and event tracking systems

---

### 4. Azure Cosmos DB Table API
**Best for:** Simple key-value storage

#### Key Features
- Key-value data model
- Compatible with Azure Table Storage
- Cost-effective for simple workloads

#### Use Cases
- User preferences
- Device metadata
- Configuration data

---

### 5. Azure Cosmos DB Gremlin API
**Best for:** Graph-based relationships

#### Key Features
- Property graph model
- Uses Apache TinkerPop Gremlin query language
- Optimized for relationship-heavy data

#### Use Cases
- Social networks
- Recommendation engines
- Fraud detection
- Network topology

---

## Comparison of Cosmos DB APIs

| API | Data Model | Query Language | Best For |
|---|---|---|---|
| Core (SQL) | Document | SQL-like | New applications |
| MongoDB | Document | MongoDB queries | MongoDB migration |
| Cassandra | Wide-column | CQL | High-write workloads |
| Table | Key-value | OData | Simple storage |
| Gremlin | Graph | Gremlin | Relationship data |

---

## How to Choose the Right API

### Choose Core (SQL) API if:
- You are building a new application
- You want maximum Azure support and features
- You need flexible JSON schema

### Choose MongoDB API if:
- You already use MongoDB
- You want minimal code changes
- Your team is familiar with MongoDB

### Choose Cassandra API if:
- You need very high write throughput
- You work with time-series or event data

### Choose Table API if:
- Your data is simple and non-relational
- You want a low-cost option

### Choose Gremlin API if:
- Relationships between entities are critical
- You need graph traversal queries

---

## Important Considerations

- All APIs share:
  - Global distribution
  - Automatic scaling
  - High availability
  - Security features
- API choice affects:
  - Query language
  - SDKs and drivers
  - Data modeling approach

---

## Summary
Azure Cosmos DB provides multiple APIs to support different application needs. Selecting the right API ensures optimal performance, scalability, and developer productivity while leveraging Cosmos DB’s global and fully managed platform.

---
