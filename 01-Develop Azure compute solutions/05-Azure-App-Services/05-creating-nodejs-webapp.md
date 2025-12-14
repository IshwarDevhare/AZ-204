npx express-generator mmdemo0102 --view ejs

cd mmdemo0102

npm install

npm start --> localhost:3000

az group create -n mmdemo0101 --location centralindia

az webapp up -g mmdemo0101 -n mmdemo0101 --sku F1 --os-type windows
