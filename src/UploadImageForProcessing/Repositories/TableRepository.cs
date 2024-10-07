using Azure.Data.Tables;
using System.Linq.Expressions;
using UploadImageForProcessing.Models;

namespace UploadImageForProcessing.Repositories
{
    public class TableRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _tableName;

        public TableRepository(IConfiguration configuration) 
        {
            _configuration = configuration;
            _connectionString = _configuration["Config:StorageConnection"];
            _tableName = _configuration["Config:StorageTableName"];
        }

        public async Task StoreInTableAsync(UploadResult uploadResult)
        {
            TableServiceClient tableServiceClient = new TableServiceClient(_connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(_tableName);
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync<UploadResult>(uploadResult);
        }

        public async Task<IEnumerable<UploadResult>> GetFromTableAsync(long cutoff, int? pageSizeInput, bool? hasAlert, bool? resolvedAlert)
        {
            var partitionKey = "ABC";
            var pageSize = pageSizeInput ?? 30;
            TableServiceClient tableServiceClient = new TableServiceClient(_connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(_tableName);
            await tableClient.CreateIfNotExistsAsync();

            bool allAlerts = hasAlert == null;
            bool allResolveds = hasAlert == null;

            Expression<Func<UploadResult, bool>> filter =
                x => x.PartitionKey == partitionKey && x.FileId > cutoff 
                && (x.HasAlert == hasAlert)
                && (x.AlertResolved == resolvedAlert);

            if (allAlerts && allResolveds)
            {
                filter =
                    x => x.PartitionKey == partitionKey && x.FileId > cutoff;
            }
            else if(allAlerts)
            {
                filter =
                    x => x.PartitionKey == partitionKey && x.FileId > cutoff
                    && (x.AlertResolved == resolvedAlert);
            }
            else if (allResolveds)
            {
                filter =
                    x => x.PartitionKey == partitionKey && x.FileId > cutoff
                    && (x.HasAlert == hasAlert);
            }

            var results = tableClient.Query<UploadResult>(filter, maxPerPage: pageSize)
                .AsPages()
                .First()
                .Values
                .ToArray();
            return results;
        }

        public async Task ResolveAlertAsync(long fileId)
        {
            var partitionKey = "ABC";
            TableServiceClient tableServiceClient = new TableServiceClient(_connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(_tableName);
            await tableClient.CreateIfNotExistsAsync();
            var entity = await tableClient.GetEntityAsync<UploadResult>(partitionKey, fileId.ToString());
            entity.Value.AlertResolved = true;
            var updateResult = await tableClient.UpdateEntityAsync<UploadResult>(entity.Value, Azure.ETag.All);
        }

        public async Task UpdateAlertAsync(long fileId, string alert)
        {
            var partitionKey = "ABC";
            TableServiceClient tableServiceClient = new TableServiceClient(_connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(_tableName);
            await tableClient.CreateIfNotExistsAsync();
            var entity = await tableClient.GetEntityAsync<UploadResult>(partitionKey, fileId.ToString());
            entity.Value.HasAlert = true;
            entity.Value.Alert = alert;
            var updateResult = await tableClient.UpdateEntityAsync<UploadResult>(entity.Value, Azure.ETag.All);
        }
    }
}
