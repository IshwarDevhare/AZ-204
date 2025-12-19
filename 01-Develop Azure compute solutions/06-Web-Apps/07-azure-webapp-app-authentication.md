# Adding App Authentication in Azure Web Apps

1. Understand the purpose of App Service Authentication (Easy Auth), which allows your Azure Web App to authenticate users without writing custom authentication code and integrates with identity providers like Azure AD, Microsoft Account, Facebook, Google, or Twitter.

2. Decide which identity provider(s) your app will support, such as Azure Active Directory for enterprise authentication or social logins for consumer applications.

3. Ensure you have an Azure Web App already created that you want to enable authentication for.

4. Navigate to the Azure Portal, select your Web App, and go to the "Authentication" blade under the "Settings" section.

5. Click "Add identity provider" to start configuring authentication for your app.

6. Choose the identity provider you want to configure, for example "Microsoft" to use Azure Active Directory.

7. Configure the selected identity provider by providing necessary details:
   - For Azure AD: choose "Express" (automatic app registration) or "Advanced" (manual app registration) and specify tenant, client ID, and client secret if using manual registration.
   - For social providers: provide the app/client ID and secret obtained from the provider portal.

8. Set the "Authentication / Authorization" status to "On" to enable App Service Authentication for the web app.

9. Configure the action for unauthenticated requests, choosing between:
   - "Log in with <provider>" (redirect users to login)
   - "Allow anonymous requests" (optional for partially public apps)

10. Optionally configure advanced settings such as token store, login flow (redirect or popup), allowed token audiences, or issuer URL for more control over authentication behavior.

11. Save the configuration to apply App Authentication settings to your Web App.

12. Update your application code (if required) to read authenticated user information from the request headers injected by App Service Authentication:
    - `X-MS-CLIENT-PRINCIPAL` header contains user claims
    - Decode the base64-encoded token if necessary to access claims

13. Test the authentication flow by accessing your web app URL:
    - Unauthenticated users should be redirected to the login page of the configured identity provider.
    - Authenticated users should see your application content and claims populated in headers.

14. Monitor authentication logs in the "Authentication / Authorization" blade or Azure App Service diagnostics to troubleshoot login or token issues.

15. For production apps, review and enforce proper session timeout, token validation, and CORS settings if your web app calls APIs from other domains.

16. Update or remove identity providers as needed from the Authentication blade without redeploying the app.

17. Document which identity providers and authentication settings are configured for each web app to maintain security governance and operational clarity.
