using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using UploadImageForProcessing.Analyze;
using UploadImageForProcessing.Authorization;
using UploadImageForProcessing.Models;
using UploadImageForProcessing.Repositories;

namespace UploadImageForProcessing.Controllers
{
    [ApiController]
    [ApiKey]
    [Route("[controller]")]
    public class UploadFileController : ControllerBase
    {
        private readonly Analyzer _analyzer;
        private readonly TableRepository _repository;
        private readonly string _connectionString;
        private readonly string _container;
        private readonly string _accountName;
        private readonly string _accountKey;

        public UploadFileController(
            Analyzer analyzer,
            TableRepository repository,
            IConfiguration configuration)
        {
            _analyzer = analyzer;
            _repository = repository;
            _connectionString = configuration["Config:StorageConnection"];
            _container = configuration["Config:ContainerName"];
            _accountName = configuration["Config:AccountName"];
            _accountKey = configuration["Config:AccountKey"];
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromForm] IFormFile fileData, bool processImage)
        {
            string sasUrl = string.Empty;
            string message = "OK";
            (string? Caption, float? Confidence) analyzeResult = (null, null);
            // timestamp serves as unique id (simple solution not suitable for any real use case)
            string rowkey = DateTime.Now.ToString("yyyyMMddHHmmssff");
            var filename = fileData.FileName;
            var filenameParts = filename.Split(".");
            filename = filenameParts[0] + "_" + rowkey + "." + filenameParts[1];
            // files in blob storage will be accessible through URL for 24 haours
            // but they will still be there even after SAS token expires
            var expiresOn = DateTime.UtcNow.AddHours(24);

            try
            {
                using (var stream = new MemoryStream())
                {
                    fileData.CopyTo(stream);
                    stream.Position = 0;
                    var blobServiceClient = new BlobServiceClient(_connectionString);
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_container);
                    BlobClient blobClient = containerClient.GetBlobClient(filename);
                    var uploadInfo = await blobClient.UploadAsync(stream, true);

                    Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
                    {
                        BlobContainerName = _container,
                        BlobName = filename,
                        ExpiresOn = expiresOn,
                    };
                    blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);//User will only be able to read the blob and it's properties
                    var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_accountName, _accountKey)).ToString();
                    sasUrl = blobClient.Uri.AbsoluteUri + "?" + sasToken;
                }

                if (processImage)
                {
                    // send request to Azure Cognitive Services to get caption
                    analyzeResult = _analyzer.AnalyzeImage(sasUrl);
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            // hardcoded global partition key
            string partitionKey = "ABC";
            var fileId = long.Parse(rowkey);

            var uploadResult = new UploadResult
            {
                Message = message,
                Caption = analyzeResult.Caption,
                CaptionConfidence = analyzeResult.Confidence,
                Filename = filename,
                Processed = processImage,
                SasUrl = sasUrl,
                RowKey = rowkey,
                FileId = fileId,
                PartitionKey = partitionKey,
                ExpiresOn = expiresOn
            };

            await _repository.StoreInTableAsync(uploadResult);

            return new OkObjectResult(uploadResult);
        }
    }
}
