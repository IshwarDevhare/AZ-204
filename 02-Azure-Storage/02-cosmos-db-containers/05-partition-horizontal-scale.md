# Partitioning and Horizontal Scaling in Azure Cosmos DB

Azure Cosmos DB is a globally distributed, multi-model database service designed to scale elastically and provide high availability. To handle large volumes of data and high request rates, **partitioning** and **horizontal scaling** are key concepts.

## 1. Partitioning in Azure Cosmos DB

Partitioning is the process of distributing data across multiple physical partitions to achieve **scalability** and **performance**.

### 1.1 What is a Partition?

A **partition** is a physical storage unit in Cosmos DB that holds a subset of your data. Partitions allow Cosmos DB to:

* Scale storage automatically
* Improve read/write performance
* Handle high throughput

### 1.2 Partition Key

The **partition key** is a property in your items used to distribute data across partitions. It should have:

* High **cardinality** (many unique values)
* Even **data distribution** to prevent hotspots
* Immutable values (once set, it cannot change)

Example: If you store `Order` items, `customerId` can be a good partition key:

{
"id": "order1",
"customerId": "cust123",
"orderDate": "2025-12-25",
"total": 100
}

Here, `customerId` ensures that orders are evenly distributed among partitions.

### 1.3 Choosing a Partition Key

* Avoid keys with low cardinality (e.g., `country` if most users are from one country)
* Avoid keys with "hot" values (frequently accessed values that cause throttling)
* Think about query patterns (queries should include the partition key for efficiency)

## 2. Horizontal Scaling in Azure Cosmos DB

Horizontal scaling is **scaling out** by adding more physical partitions or throughput instead of scaling a single node vertically.

### 2.1 Request Units (RUs) and Throughput

* Azure Cosmos DB uses **Request Units (RUs)** to measure throughput.
* You can provision throughput at the **container** or **database** level.
* Cosmos DB automatically spreads the RUs across partitions based on the partition key.

### 2.2 Scaling Throughput

1. **Manual scaling**: Increase or decrease RUs in the Azure portal or via SDK.
2. **Autoscale**: Cosmos DB automatically adjusts throughput based on workload, up to a maximum RU/s.

Steps in the portal:

1. Go to your **Cosmos DB account**.
2. Select the **container** or **database**.
3. Click **Scale & Settings**.
4. Adjust **Provisioned throughput** or enable **Autoscale**.

### 2.3 Benefits of Horizontal Scaling

* Handles millions of operations per second
* Supports **multi-region replication**
* Ensures low latency globally

## 3. Practical Example

Suppose we have an `Orders` container:

{
"id": "order1",
"customerId": "cust123",
"orderDate": "2025-12-25",
"total": 100
}

1. **Partition Key:** `customerId`
2. **Throughput:** 4000 RU/s (manual or autoscale)

Cosmos DB automatically:

* Assigns `customerId` values to partitions
* Distributes RU/s across partitions
* Ensures low latency and high availability

## 4. Best Practices

* Choose **high-cardinality** partition keys
* Avoid partition key changes
* Monitor **RU consumption** to prevent throttling
* Use **query patterns** that filter by partition key whenever possible
* Consider **multi-region writes** if global low latency is needed

## References

* [Partitioning in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview)
* [Scale throughput in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/provision-throughput)
* [Best practices for partitioning and scaling](https://learn.microsoft.com/en-us/azure/cosmos-db/performance-tips)

 **Summary:**
Partitioning enables Cosmos DB to distribute data across multiple physical partitions, while horizontal scaling ensures your database can handle growing workloads efficiently. Proper partition key selection and RU management are crucial for optimal performance.
