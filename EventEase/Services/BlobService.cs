using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace EventEase.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            var containerName = configuration["AzureBlobContainerName"] ?? "venueimages";

            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            var blobClient = _containerClient.GetBlobClient($"{Guid.NewGuid()}{Path.GetExtension(fileName)}");
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = GetContentType(fileName) });
            return blobClient.Uri.ToString();
        }

        private static string GetContentType(string fileName)
        {
            return fileName.ToLower() switch
            {
                var f when f.EndsWith(".jpg") || f.EndsWith(".jpeg") => "image/jpeg",
                var f when f.EndsWith(".png") => "image/png",
                var f when f.EndsWith(".gif") => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}

