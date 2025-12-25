# Change Feed in Azure Cosmos DB

Azure Cosmos DB provides a **Change Feed**, which is a persistent record of changes (inserts and updates) to documents in a container. It allows applications to **react to data changes in near real-time** without continuously querying the database.

---

## 1. What is Change Feed?

The **Change Feed**:

* Provides an ordered list of documents that were **created or updated**
* Does **not include deletes** by default (can be tracked with soft-delete strategies)
* Is **partition-aware**, meaning each partition has its own feed
* Supports **streaming processing**, event-driven architectures, and ETL pipelines

---

## 2. How Change Feed Works

* Every container in Cosmos DB maintains a **Change Feed**
* Each document change generates a **log entry** in the feed
* Consumers can **read the feed incrementally** using SDKs, Azure Functions, or the REST API

**Flow Example:**

1. Document inserted/updated in the container
2. Change Feed logs the change in the corresponding partition
3. Application reads the feed and processes changes

---

## 3. Accessing Change Feed

### 3.1 Using SDK (Example: .NET)

```csharp
var iterator = container.GetChangeFeedIterator<MyDocument>(
    ChangeFeedStartFrom.Beginning(),
    ChangeFeedMode.Incremental);

while (iterator.HasMoreResults)
{
    foreach (var doc in await iterator.ReadNextAsync())
    {
        Console.WriteLine($"Document ID: {doc.Id}");
    }
}
```

**Parameters:**

* `ChangeFeedStartFrom.Beginning()` – starts from the beginning of the feed
* `ChangeFeedMode.Incremental` – only new changes

---

### 3.2 Using Azure Functions

* Trigger type: **Cosmos DB Trigger**
* Automatically receives documents from the Change Feed
* Example:

```csharp
[FunctionName("ProcessChanges")]
public static void Run(
    [CosmosDBTrigger(
        databaseName: "MyDatabase",
        collectionName: "MyContainer",
        ConnectionStringSetting = "CosmosDBConnection",
        LeaseCollectionName = "leases")] IReadOnlyList<Document> input)
{
    foreach (var doc in input)
    {
        Console.WriteLine($"Processing document: {doc.Id}");
    }
}
```

* The **lease container** tracks which changes have already been processed

---

## 4. Key Concepts

| Concept              | Description                                                     |
| -------------------- | --------------------------------------------------------------- |
| **Lease**            | Keeps track of which documents have been processed in a feed    |
| **Partition Key**    | Change Feed is partition-aware, each partition has its own feed |
| **Start Position**   | Where the feed begins: beginning, now, or a specific timestamp  |
| **Incremental Mode** | Only new changes since the last read                            |

---

## 5. Best Practices

* Use **leases** to enable distributed processing across multiple consumers
* Process **batches** of changes for efficiency
* Handle **exceptions** carefully to avoid missing updates
* Monitor **RU consumption**, as reading Change Feed consumes throughput
* Avoid long-running operations per document to maintain high throughput

---

## 6. Use Cases

* Event-driven architectures (trigger downstream processing)
* Real-time analytics and dashboards
* Synchronization between Cosmos DB and external systems
* ETL pipelines for data warehousing

---

## References

* [Change Feed in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/change-feed)
* [Azure Functions Cosmos DB Trigger](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2)
* [Best practices for Change Feed](https://learn.microsoft.com/en-us/azure/cosmos-db/change-feed#best-practices)

