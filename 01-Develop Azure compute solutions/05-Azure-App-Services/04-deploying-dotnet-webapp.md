dotnet new webapp -n mmdemo0101 -f net8.0

az group create -n mmdemo0101 --location centralindia

az webapp up -g mmdemo0101 -n mmdemo0101 --sku F1 --os-type windows

# Modify the content and update the web appp (index.cshtml for testing)

az webapp up

=======================================