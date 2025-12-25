# Using Stored Procedures in Azure Cosmos DB

Azure Cosmos DB supports **stored procedures**, which are JavaScript functions executed within the database engine. They allow you to implement **server-side logic** close to the data, improving performance and ensuring transactional consistency.

---

## 1. What are Stored Procedures?

A **stored procedure** in Cosmos DB:

* Runs on the server within a single partition
* Can perform multiple operations atomically (transactionally)
* Is written in **JavaScript**
* Can be executed using SDKs, REST API, or Azure Portal

### Key Characteristics:

* Executes **atomically within a partition**
* Cannot span multiple partitions
* Ideal for batch operations, complex logic, or custom validation

---

## 2. Creating a Stored Procedure

Stored procedures are associated with a **container**. Each stored procedure has:

* **id**: Unique identifier
* **body**: JavaScript function defining the logic

### Example: Incrementing a Counter

Suppose you have a `Counters` container, and you want to increment a counter for a specific document.

Stored procedure `incrementCounter`:

```javascript
function incrementCounter(counterId) {
    var context = getContext();
    var container = context.getCollection();
    var response = context.getResponse();

    // Query the document by id
    var filterQuery = 'SELECT * FROM c WHERE c.id = "' + counterId + '"';
    var isAccepted = container.queryDocuments(
        container.getSelfLink(),
        filterQuery,
        function(err, documents, responseOptions) {
            if (err) throw new Error("Error: " + err.message);
            if (documents.length !== 1) throw new Error("Document not found");

            var doc = documents[0];
            doc.count = (doc.count || 0) + 1;

            // Replace the document
            var isAccepted = container.replaceDocument(doc._self, doc, function(err, docReplaced) {
                if (err) throw new Error("Error replacing document: " + err.message);
                response.setBody(docReplaced.count);
            });
        }
    );

    if (!isAccepted) throw new Error("Query not accepted by server.");
}
```

---

## 3. Deploying a Stored Procedure

### Using Azure Portal:

1. Go to your **Cosmos DB account**.
2. Select the **container**.
3. Go to **Stored Procedures** under **Settings**.
4. Click **New Stored Procedure**.
5. Provide an **id** and paste your **JavaScript function**.
6. Save.

### Using SDK (Example: Node.js)

```javascript
const { CosmosClient } = require("@azure/cosmos");

const client = new CosmosClient({ endpoint, key });
const database = client.database("MyDatabase");
const container = database.container("Counters");

const sprocDefinition = {
    id: "incrementCounter",
    body: function (counterId) {
        var context = getContext();
        var container = context.getCollection();
        var response = context.getResponse();

        var filterQuery = 'SELECT * FROM c WHERE c.id = "' + counterId + '"';
        var isAccepted = container.queryDocuments(
            container.getSelfLink(),
            filterQuery,
            function (err, documents) {
                if (err) throw err;
                if (documents.length !== 1) throw new Error("Document not found");

                var doc = documents[0];
                doc.count = (doc.count || 0) + 1;

                container.replaceDocument(doc._self, doc, function (err, docReplaced) {
                    if (err) throw err;
                    response.setBody(docReplaced.count);
                });
            }
        );
        if (!isAccepted) throw new Error("Query not accepted.");
    },
};

await container.scripts.storedProcedure.create(sprocDefinition);
```

---

## 4. Executing a Stored Procedure

### Using SDK:

```javascript
const { resource: result } = await container.scripts.storedProcedure("incrementCounter").execute(
    "partitionKeyValue", // Partition key of the document
    ["counter1"]          // Arguments to the stored procedure
);
console.log("Updated count:", result);
```

**Important:** The stored procedure must be executed **within the partition key** where the target document resides.

---

## 5. Best Practices

* Use stored procedures for **transactional operations within a partition**
* Keep stored procedures **lightweight and performant**
* Avoid long-running operations that can cause timeout
* Test thoroughly using the **Azure Portal** before production use
* Combine with **triggers** or **user-defined functions (UDFs)** if needed

---

## References

* [Stored procedures in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/stored-procedures-triggers-udfs)
* [Server-side programming in Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/programming)
* [Best practices for Cosmos DB server-side scripts](https://learn.microsoft.com/en-us/azure/cosmos-db/performance-tips)

