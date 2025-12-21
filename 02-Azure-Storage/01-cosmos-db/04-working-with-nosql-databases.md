# 04 – Working with NoSQL Databases

## Overview
A **NoSQL database** is a non-relational database designed to store and manage large volumes of data with **high scalability, flexibility, and performance**. Unlike relational databases, NoSQL databases do not rely on fixed schemas or tables with rows and columns.

NoSQL databases are commonly used in **cloud-native, distributed, and real-time applications**.

---

## Key Characteristics of NoSQL Databases

### 1. Schema Flexibility
- Data does not require a fixed schema
- Each record can have different fields
- Easy to evolve data models over time

### 2. Horizontal Scalability
- Scales out by adding more servers
- Designed to handle large traffic and data volumes

### 3. High Performance
- Optimized for fast reads and writes
- Suitable for real-time applications

### 4. Distributed Architecture
- Data is distributed across multiple nodes
- Supports replication and fault tolerance

---

## Types of NoSQL Databases

### 1. Key-Value Databases
- Store data as key–value pairs
- Simple and fast access

**Examples:** Redis, Azure Table API  
**Use Cases:** Session storage, caching, user preferences

---

### 2. Document Databases
- Store data as JSON or BSON documents
- Flexible structure with nested data

**Examples:** Azure Cosmos DB (SQL API), MongoDB  
**Use Cases:** Web apps, content management, product catalogs

---

### 3. Column-Family (Wide-Column) Databases
- Store data in columns instead of rows
- Optimized for large-scale write operations

**Examples:** Cassandra, HBase  
**Use Cases:** IoT data, time-series data, logs

---

### 4. Graph Databases
- Store data as nodes and relationships
- Optimized for relationship queries

**Examples:** Azure Cosmos DB (Gremlin API), Neo4j  
**Use Cases:** Social networks, recommendation systems, fraud detection

---

## Working with NoSQL Data

### Data Modeling
- Focus on **query patterns**, not normalization
- Data is often **denormalized**
- Design data to be read efficiently

### Partitioning
- Data is divided across partitions
- Choosing the right partition key is critical for performance
- Helps distribute load evenly

### Indexing
- Indexes improve query performance
- Most NoSQL databases provide automatic indexing
- Custom indexing can reduce cost and improve speed

---

## Consistency Models

NoSQL databases often provide flexible consistency options:

- **Strong consistency** – Always returns latest data
- **Eventual consistency** – Data syncs over time
- **Session consistency** – Consistent within a user session

Trade-off exists between **consistency, availability, and performance**.

---

## Advantages of NoSQL Databases

- Highly scalable
- Flexible data models
- High availability
- Optimized for cloud and distributed systems
- Handles unstructured and semi-structured data

---

## Limitations of NoSQL Databases

- Limited support for complex joins
- Data duplication may increase storage
- Requires careful data modeling
- Not ideal for traditional transactional systems

---

## When to Use NoSQL Databases

Use NoSQL databases when:
- Data structure changes frequently
- Application needs high scalability
- Low-latency access is required
- Data is unstructured or semi-structured
- Global distribution is needed

---

## NoSQL vs Relational Databases

| Feature | NoSQL | Relational (SQL) |
|---|---|---|
| Schema | Flexible | Fixed |
| Scalability | Horizontal | Vertical |
| Joins | Limited | Strong support |
| Transactions | Limited | ACID-compliant |
| Use Case | Big data, real-time apps | Traditional business apps |

---

## Summary
Working with NoSQL databases requires a different approach compared to relational databases. By focusing on scalability, flexible schemas, and query-based data modeling, NoSQL databases enable modern applications to perform efficiently at global scale.

---
