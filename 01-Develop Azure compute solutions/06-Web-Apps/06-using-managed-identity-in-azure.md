# Using Managed Identity in Azure (Generic Steps)

1. Understand the purpose of Managed Identity, which allows an Azure resource to authenticate to other Azure services without storing credentials, secrets, or connection strings in code.

2. Decide the type of Managed Identity to use, choosing System-Assigned Managed Identity when the identity lifecycle should be tied to the resource, or User-Assigned Managed Identity when the same identity needs to be shared across multiple resources.

3. Create or identify the Azure resource that will use Managed Identity, such as an App Service, Azure Function, Virtual Machine, Container App, Logic App, or Azure Automation account.

4. Enable System-Assigned Managed Identity on the resource using the Azure portal or Azure CLI with the command az <resource> identity assign, which automatically creates an identity in Microsoft Entra ID.

5. Verify that the Managed Identity has been created by retrieving the principal ID of the identity from the resource identity settings or by using Azure CLI identity show commands.

6. Identify the target Azure service that the resource needs to access, such as Azure Key Vault, Azure Storage, Azure SQL Database, Azure Service Bus, Azure Container Registry, or Azure Cosmos DB.

7. Determine the minimum required permission needed on the target service, following the principle of least privilege, for example Reader, Contributor, AcrPull, Storage Blob Data Reader, or Key Vault Secrets User.

8. Assign the appropriate Azure role to the Managed Identity at the correct scope (resource, resource group, or subscription) using Azure role-based access control.

9. Confirm that the role assignment is successfully created and visible in the target resource’s Access Control (IAM) settings.

10. Update the application or service configuration so that authentication is performed using Managed Identity instead of credentials, removing usernames, passwords, access keys, or connection strings that contain secrets.

11. In application code or configuration, use the Azure SDKs or platform features that support Managed Identity authentication, ensuring DefaultAzureCredential or equivalent identity-based authentication is used.

12. If accessing Azure Key Vault, reference secrets securely by either using Managed Identity in code or by using platform-supported Key Vault references where available.

13. Test access from the Azure resource to the target service to confirm that authentication succeeds without providing any secrets.

14. Monitor authentication and access attempts using Azure logs and diagnostics to ensure the Managed Identity is being used correctly and to troubleshoot permission issues.

15. Rotate or revoke permissions by modifying role assignments instead of changing application code or configuration.

16. If the Managed Identity is no longer required, remove the role assignments first to prevent unintended access.

17. Disable the Managed Identity on the resource if it is no longer needed, which automatically removes the identity from Microsoft Entra ID for system-assigned identities.

18. Review security posture periodically to ensure Managed Identities are used consistently and no hard-coded secrets are reintroduced into applications.

19. Use Managed Identity wherever possible to improve security, simplify credential management, and comply with Azure security best practices.

20. Document which resources use Managed Identity and what permissions are granted to maintain operational clarity and governance.
