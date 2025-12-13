git clone https://github.com/Azure-Samples/aci-helloworld.git

docker build aci-helloworld -t mmdemo0101

docker run -d -p 8080:80 mmdemo0101

http://localhost:8080/

az group create -n mmdemo0101 --location centralindia

az acr create -g mmdemo0101 -n mmdemo0101 --sku Basic

az acr login -n mmdemo0101

docker images

docker tag mmdemo0101 mmdemo0101.azurecr.io/mmdemo0101:v1

docker push mmdemo0101.azurecr.io/mmdemo0101:v1

az acr update -n mmdemo0101 --admin-enabled true

az acr credential show -n mmdemo0101

az container create -g mmdemo0101 -n mmdemo0101 --image mmdemo0101.azurecr.io/mmdemo0101:v1 --cpu 1 --memory 1 --registry-login-server mmdemo0101.azurecr.io --registry-username {replace_username} --registry-password {replace_user_password} --ip-address Public --dns-name-label mmdemo0101 --ports 80 --os-type Linux

-- copy ip address and browse the site to see if everything is working as expected

-- clean up the resource

az group delete -n mmdemo0101