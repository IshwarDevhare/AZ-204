# Azure Cosmos DB Consistency Levels

Azure Cosmos DB is a globally distributed, multi-model database that provides **five consistency levels** to control the trade-off between **consistency**, **availability**, and **latency**. Choosing the right consistency level is crucial depending on your application’s needs.

---

## 1. Strong Consistency

- **Definition:** Guarantees **linearizability**, meaning reads always return the **most recent committed write**.  
- **Use Case:** Applications that require absolute accuracy, such as **banking transactions**.  
- **Pros:** Strong correctness; reads are always up-to-date.  
- **Cons:** Higher latency and reduced availability during network partitions.  

---

## 2. Bounded Staleness

- **Definition:** Guarantees reads lag behind writes by at most **K versions** or **T time interval**.  
- **Use Case:** Scenarios where slightly stale data is acceptable but ordering matters, like **social media timelines**.  
- **Pros:** Predictable lag; stronger than eventual consistency.  
- **Cons:** Higher latency than session or eventual consistency; still less strict than strong.

---

## 3. Session Consistency

- **Definition:** Guarantees **monotonic reads and writes** within a single session. Each client sees its own writes consistently.  
- **Use Case:** Personalization scenarios, e.g., **shopping carts** or **user preferences**.  
- **Pros:** Low latency; maintains a good balance between consistency and performance.  
- **Cons:** Only guarantees consistency per session, not globally.  

---

## 4. Consistent Prefix

- **Definition:** Guarantees that reads never see out-of-order writes. If writes were `W1, W2, W3`, a read may return `W1`, `W1,W2` but never `W2` before `W1`.  
- **Use Case:** Event streams or feeds where order matters, e.g., **activity logs**.  
- **Pros:** Preserves order; better performance than bounded staleness.  
- **Cons:** May still be slightly stale.  

---

## 5. Eventual Consistency

- **Definition:** Guarantees that **all replicas will converge eventually**, but reads may return stale data.  
- **Use Case:** Scenarios where **performance and availability are prioritized over immediate consistency**, e.g., **caches, leaderboards**.  
- **Pros:** Lowest latency; highest availability and throughput.  
- **Cons:** Reads may temporarily return outdated data.  

---

## Comparison Table

| Consistency Level     | Guarantees                            | Latency | Availability | Use Case Example                |
|----------------------|--------------------------------------|---------|-------------|--------------------------------|
| Strong               | Linearizability                       | High    | Lower       | Banking, critical transactions |
| Bounded Staleness    | Reads lag by K versions or T interval | Medium  | Medium      | Timelines, feeds               |
| Session              | Monotonic reads/writes per session    | Low     | High        | User sessions, carts           |
| Consistent Prefix    | Ordered reads                         | Low     | High        | Activity streams               |
| Eventual             | Convergence over time                 | Very Low| High        | Caches, leaderboards           |

---

## Choosing the Right Consistency Level

1. **Critical correctness:** Use **Strong**.  
2. **Predictable staleness:** Use **Bounded Staleness**.  
3. **User-specific data:** Use **Session**.  
4. **Ordered data streams:** Use **Consistent Prefix**.  
5. **Performance-focused applications:** Use **Eventual**.  

---

**References:**

- [Azure Cosmos DB Documentation – Consistency Levels](https://learn.microsoft.com/en-us/azure/cosmos-db/consistency-levels)

