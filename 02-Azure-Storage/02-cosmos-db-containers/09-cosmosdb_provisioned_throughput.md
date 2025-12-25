# Provisioning Throughput on a Container in Azure Cosmos DB

Azure Cosmos DB allows you to **provision throughput** for containers and databases. Throughput determines the **number of Request Units (RUs) per second** available to read or write operations. Proper throughput provisioning ensures predictable performance and cost management.

---

## 1. What is Provisioned Throughput?

* **Request Units (RUs):** A measure of throughput in Cosmos DB. Each operation (read, write, query) consumes RUs.
* **Provisioned Throughput:** You allocate a fixed RU/s to a container or database.
* Throughput can be **dedicated to a container** or **shared across a database**.

**Key Points:**

* Guaranteed performance based on allocated RU/s
* Supports **horizontal scaling** across partitions
* Can be **manually adjusted** or **autoscaled**

---

## 2. Provisioning Throughput on a Container

### 2.1 Via Azure Portal

1. Navigate to your **Cosmos DB account**.
2. Select the **database** and then the **container**.
3. Click **Scale & Settings**.
4. Choose **Provisioned throughput**.
5. Enter the desired **RU/s**.
6. Optionally, enable **Autoscale** to let Cosmos DB scale automatically up to a maximum RU/s.
7. Click **Save**.

### 2.2 Via SDK (Example: .NET)

```csharp
using Microsoft.Azure.Cosmos;

CosmosClient client = new CosmosClient(endpoint, key);
Database database = await client.CreateDatabaseIfNotExistsAsync("MyDatabase");

ContainerProperties containerProperties = new ContainerProperties("MyContainer", "/partitionKey");

// Provision 4000 RU/s
Container container = await database.CreateContainerIfNotExistsAsync(
    containerProperties,
    throughput: 4000
);
```

### 2.3 Via ARM Template or REST API

You can also **deploy containers with throughput** using ARM templates or the REST API by specifying the `throughput` property during container creation.

---

## 3. Autoscale vs Manual Throughput

| Feature  | Manual Throughput  | Autoscale Throughput         |
| -------- | ------------------ | ---------------------------- |
| RU/s     | Fixed              | Automatically scales up/down |
| Cost     | Predictable        | Pay for maximum RU/s         |
| Use Case | Steady workloads   | Variable workloads           |
| Max RU/s | Specified manually | Set maximum RU/s             |

---

## 4. Considerations for Partitioned Containers

* Each container must have a **partition key**
* Throughput is **distributed across physical partitions**
* Minimum RU/s per partition: 400 RU/s
* If workload increases, Cosmos DB **adds partitions** automatically for horizontal scaling

---

## 5. Best Practices

* Choose a **partition key with high cardinality** for even RU distribution
* Monitor RU/s consumption to avoid **throttling (HTTP 429)**
* Use **autoscale** for unpredictable workloads
* Combine **Change Feed** or **Triggers** with provisioned throughput for event-driven workloads
* Avoid setting throughput higher than necessary to save cost

---

## 6. Monitoring Throughput

* Azure Portal: **Metrics > Request Units**
* SDK: Monitor `RequestCharge` for each operation
* Alerts: Set up **alerts for high RU consumption or throttling**

---

## References

* [Provision throughput in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/provision-throughput)
* [Best practices for scaling and partitioning](https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview)
* [Autoscale provisioned throughput](https://learn.microsoft.com/en-us/azure/cosmos-db/autoscale-overview)
