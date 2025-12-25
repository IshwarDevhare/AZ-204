using Azure.Identity;
using Azure.Storage.Blobs;

BlobServiceClient GetBlobServiceClient(string accountName)
 => new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new DefaultAzureCredential());
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

BlobContainerClient GetBlobContainerClient(BlobServiceClient client, string containerName)
 => client.GetBlobContainerClient(containerName);

var client = GetBlobServiceClient("mmdemo0101");
Console.WriteLine($"Client URI {client.Uri}");

var container = GetBlobContainerClient(client, "mmdemo0101");
var containerProps = await container.GetPropertiesAsync();

Console.WriteLine($"Client URI {container.Uri}");
Console.WriteLine($"Container Access Level {containerProps.Value.PublicAccess}");
Console.WriteLine($"Container Last Modified {containerProps.Value.LastModified}");
