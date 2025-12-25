# Managing Triggers in Azure Cosmos DB Using SQL API via REST

Azure Cosmos DB allows you to create **triggers** — server-side JavaScript functions that can execute **before or after operations** (insert, update, delete) on documents. Using the **SQL API**, you can manage triggers programmatically via the **REST API**.

---

## 1. What is a Trigger?

A **trigger** in Cosmos DB:

* Executes automatically **before (pre)** or **after (post)** an operation
* Is written in **JavaScript**
* Runs within a **single partition**
* Useful for validation, modification, or logging

Types of triggers:

| Type         | Description                                     |
| ------------ | ----------------------------------------------- |
| Pre-Trigger  | Executes before an operation (e.g., validation) |
| Post-Trigger | Executes after an operation (e.g., logging)     |

---

## 2. REST API Overview

To manage triggers via REST API:

* Use **HTTP requests** with appropriate methods: `POST`, `GET`, `PUT`, `DELETE`
* Include **authorization headers** and **Cosmos DB master key**
* Specify **resource type**: `triggers`
* Include **partition key** when needed

**Endpoint format:**

```
https://{account}.documents.azure.com/dbs/{db-id}/colls/{coll-id}/triggers
```

---

## 3. Creating a Trigger

To create a trigger:

* Method: `POST`
* Resource type: `triggers`
* Headers:

```
Authorization: {auth-token}
x-ms-date: {utc-date}
x-ms-version: 2025-12-25
Content-Type: application/json
```

* Body (example: pre-trigger to validate `age`):

```json
{
  "id": "validateAge",
  "triggerType": "Pre",
  "triggerOperation": "All",
  "body": "function() { var doc = getContext().getRequest().getBody(); if(doc.age < 0){ throw new Error('Age cannot be negative'); } }"
}
```

---

## 4. Reading a Trigger

* Method: `GET`
* URL:

```
https://{account}.documents.azure.com/dbs/{db-id}/colls/{coll-id}/triggers/{trigger-id}
```

* Headers: same as above

Response returns the trigger definition.

---

## 5. Replacing a Trigger

To update a trigger:

* Method: `PUT`
* URL: same as reading a trigger
* Body: updated JavaScript code

Example: change validation rule to `age < 18`:

```json
{
  "id": "validateAge",
  "triggerType": "Pre",
  "triggerOperation": "All",
  "body": "function() { var doc = getContext().getRequest().getBody(); if(doc.age < 18){ throw new Error('Age must be at least 18'); } }"
}
```

---

## 6. Deleting a Trigger

* Method: `DELETE`
* URL: same as reading a trigger
* Headers: same as above

This permanently removes the trigger from the container.

---

## 7. Using a Trigger During Operations

When performing an operation (e.g., `POST` to insert a document), include:

```
x-ms-documentdb-pre-trigger-include: {triggerId}   // for pre-triggers
x-ms-documentdb-post-trigger-include: {triggerId}  // for post-triggers
```

Example (Insert document with pre-trigger):

```http
POST https://{account}.documents.azure.com/dbs/{db-id}/colls/{coll-id}/docs
x-ms-documentdb-pre-trigger-include: validateAge
Content-Type: application/json

{
  "id": "user1",
  "name": "Alice",
  "age": 17
}
```

If the pre-trigger fails, the operation is **aborted**.

---

## 8. Best Practices

* Pre-triggers: validate or modify documents before write
* Post-triggers: logging, notifications, or secondary operations
* Keep triggers **small and fast** to avoid RU spikes
* Always test triggers before production use
* Include **error handling** in the JavaScript body

---

## References

* [Azure Cosmos DB Triggers](https://learn.microsoft.com/en-us/azure/cosmos-db/stored-procedures-triggers-udfs#triggers)
* [REST API Reference for Triggers](https://learn.microsoft.com/en-us/rest/api/cosmos-db/)
* [Best practices for triggers](https://learn.microsoft.com/en-us/azure/cosmos-db/performance-tips)

