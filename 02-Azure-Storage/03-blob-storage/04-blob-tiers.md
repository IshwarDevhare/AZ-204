# Azure Blob Storage Tiers

Azure Blob Storage offers different access tiers to balance **cost** and **access frequency**.

---

## Blob Storage Tiers

| Tier     | Storage Cost       | Access Cost        | Access Latency       | Typical Use Cases                         |
|----------|-----------------|-----------------|------------------|-------------------------------------------|
| **Hot**   | Highest          | Lowest          | Immediate         | Frequently accessed data, active websites, live feeds |
| **Cool**  | Lower than Hot   | Higher than Hot | Immediate         | Infrequently accessed data, backups, disaster recovery |
| **Archive** | Lowest         | Highest         | Hours (rehydration) | Long-term archival, compliance, old backups, historical data |

---

## Key Notes

1. **Hot Tier**  
   - For data accessed frequently  
   - Higher storage cost, lower read/write cost  
   - Use for active datasets and live applications  

2. **Cool Tier**  
   - For data accessed infrequently but still needed immediately  
   - Lower storage cost than Hot, higher read/write cost  
   - Minimum storage duration: 30 days  
   - Use for backups, disaster recovery, infrequently used datasets  

3. **Archive Tier**  
   - For long-term storage, rarely accessed data  
   - Lowest storage cost, high access/retrieval cost  
   - Rehydration required before access, can take hours  
   - Minimum storage duration: 180 days  
   - Use for compliance archives, old backups, historical data  

---

## Additional Notes

- You can **change blob tiers** anytime (Hot ↔ Cool ↔ Archive)  
- Billing includes:
  - Storage (per GB)  
  - Access (per read/write)  
  - Data retrieval (especially for Archive tier)  
- Performance:
  - Hot & Cool → immediate access  
  - Archive → hours to access

---

