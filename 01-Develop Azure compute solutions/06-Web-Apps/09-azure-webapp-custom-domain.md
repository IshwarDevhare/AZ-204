# Buying and Configuring a Custom Domain for Azure App Service

1. Understand the purpose of a custom domain: it allows your Azure Web App to be accessed via a branded URL instead of the default azurewebsites.net domain.

2. Decide on the domain name you want to purchase that is available and suitable for your application.

3. Choose a domain registrar to purchase the domain. This can be a third-party registrar (GoDaddy, Namecheap, etc.) or directly via Azure App Service Domain if you want integrated management.

4. Purchase the custom domain:
   - If using a third-party registrar, complete the domain registration and note the DNS management details.
   - If using Azure App Service Domain, navigate to your Web App, go to "Custom domains", select "Buy domain", and follow the purchase flow.

5. Once the domain is purchased, verify domain ownership in Azure App Service:
   - Add a TXT record in the DNS settings of your domain registrar.
   - Azure will provide the specific TXT value to prove ownership.
   - Wait for DNS propagation and confirm verification in the Azure Portal.

6. Configure the custom domain in your Web App:
   - Navigate to the "Custom domains" blade in your Web App.
   - Click "Add custom domain".
   - Enter the purchased domain name and validate ownership.
   - Select the hostname type (root domain or subdomain) and add it to the app.

7. Update DNS records at your domain registrar to point to your Azure Web App:
   - For an A record (root domain), point to the Web App’s IP address.
   - For a CNAME record (subdomain like www), point to the default azurewebsites.net domain.

8. Enable SSL/TLS for your custom domain to secure your traffic:
   - Use Azure App Service Managed Certificate (free) or upload your own certificate.
   - Bind the certificate to your custom domain in the "TLS/SSL settings" blade.

9. Optionally configure HTTP to HTTPS redirection to ensure all traffic is secure.

10. Test the custom domain by accessing your Web App using the new domain name to verify DNS propagation and SSL configuration.

11. Monitor the domain status in Azure to ensure it remains valid and SSL certificates are renewed automatically if using managed certificates.

12. Document the custom domain configuration including DNS records, certificate type, and expiry for operational clarity and maintenance.
