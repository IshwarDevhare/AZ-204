# Microservices Deployment to Azure Web Apps

1. Decide the microservices deployment approach where each microservice is deployed as an independent Azure Web App and all services share a single Linux App Service Plan.

2. Ensure prerequisites are available including an active Azure subscription, Azure CLI, Docker (for container deployments), and independently deployable microservice source code.

3. Authenticate to Azure using Azure CLI by running `az login` and selecting the correct subscription using `az account set --subscription <SUBSCRIPTION_ID>`.

4. Create a resource group to host all microservices using `az group create --name microservices-rg --location eastus`.

5. Create a Linux App Service Plan that will host all microservices using `az appservice plan create --name microservices-plan --resource-group microservices-rg --sku B1 --is-linux`.

6. Create an Azure Container Registry to store container images using `az acr create --resource-group microservices-rg --name microservicesacr --sku Basic --admin-enabled true`.

7. Log in to Azure Container Registry using `az acr login --name microservicesacr`.

8. Build the Docker image for the first microservice using `docker build -t microservicesacr.azurecr.io/orderservice:v1 .`.

9. Push the Docker image to Azure Container Registry using `docker push microservicesacr.azurecr.io/orderservice:v1`.

10. Create an Azure Web App for the microservice using `az webapp create --resource-group microservices-rg --plan microservices-plan --name orderservice-webapp --deployment-container-image-name microservicesacr.azurecr.io/orderservice:v1`.

11. Configure container registry credentials for the Web App using `az webapp config container set --name orderservice-webapp --resource-group microservices-rg --docker-registry-server-url https://microservicesacr.azurecr.io --docker-registry-server-user <ACR_USERNAME> --docker-registry-server-password <ACR_PASSWORD>`.

12. Repeat the image build and push process for additional microservices using commands like `docker build -t microservicesacr.azurecr.io/paymentservice:v1 .` and `docker push microservicesacr.azurecr.io/paymentservice:v1`.

13. Create a Web App for each additional microservice using `az webapp create --resource-group microservices-rg --plan microservices-plan --name paymentservice-webapp --deployment-container-image-name microservicesacr.azurecr.io/paymentservice:v1`.

14. For code-based deployment, create a Web App using `az webapp create --resource-group microservices-rg --plan microservices-plan --name inventoryservice-webapp --runtime DOTNET|8.0`.

15. Package the microservice source code into a ZIP file using `zip -r app.zip .`.

16. Deploy the ZIP package to the Web App using `az webapp deployment source config-zip --resource-group microservices-rg --name inventoryservice-webapp --src app.zip`.

17. Configure environment variables for a microservice using `az webapp config appsettings set --resource-group microservices-rg --name orderservice-webapp --settings SERVICE_NAME=OrderService SERVICE_VERSION=v1`.

18. Validate each microservice deployment by accessing `https://<webapp-name>.azurewebsites.net` in a browser.

19. Update a container-based microservice by building and pushing a new image version and updating the Web App container image using `az webapp config container set`.

20. Update a code-based microservice by rebuilding the ZIP file and redeploying it using ZIP deploy.

21. Manage microservice lifecycle operations such as stopping, starting, or restarting using `az webapp stop`, `az webapp start`, and `az webapp restart`.

22. Remove an individual microservice using `az webapp delete --name <webapp-name> --resource-group microservices-rg`.

23. Clean up all deployed microservices and resources by deleting the resource group using `az group delete --name microservices-rg --yes --no-wait`.
