// See https://aka.ms/new-console-template for more information
using Azure.Containers.ContainerRegistry;
using Azure.Identity;

Console.WriteLine("Hello, World!");

var endpoint = new Uri("https://myregistry.azurecr.io");
var registryClient = new ContainerRegistryClient(endpoint,
new DefaultAzureCredential(),
new ContainerRegistryClientOptions
{
    Audience = ContainerRegistryAudience.AzureResourceManagerPublicCloud
});

var repositryNames = registryClient.GetRepositoryNamesAsync();
// List repositories
await foreach (string repositoryName in repositryNames)
{
    Console.WriteLine("Repository Name - " + repositoryName);

    var repository = registryClient.GetRepository(repositoryName);
    var imageManifests = repository.GetAllManifestPropertiesAsync();
    // List tags
    await foreach (var imageManifest in imageManifests)
    {
        Console.WriteLine($" Found Image Manifest: {imageManifest.Digest}");

        var image = repository.GetArtifact(imageManifest.Digest);

        foreach (var tag in imageManifest.Tags)
        {
            Console.WriteLine($"    with Tag: {tag}");
        }
    }
}

Console.WriteLine("Connected to {0}", endpoint);