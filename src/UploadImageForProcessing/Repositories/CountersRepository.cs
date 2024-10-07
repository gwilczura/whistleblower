using Azure.Data.Tables;
using System.Linq.Expressions;
using UploadImageForProcessing.Models;

namespace UploadImageForProcessing.Repositories
{
    public class CountersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly object _syncLocker = new object();

        public CountersRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["Config:StorageConnection"];
            _tableName = _configuration["Config:CountersTableName"];
        }

        public void SetCounter(string counterId, int count)
        {
            lock (_syncLocker)
            {
                ChangeCounter(counterId, 0, overrideValue: count);
            }
        }

        public void IncreaseCounter(string counterId)
        {
            lock(_syncLocker)
            {
                ChangeCounter(counterId, -1);
            }
        }

        public async Task DecreaseCounter(string counterId)
        {
            lock (_syncLocker)
            {
                ChangeCounter(counterId, -1);
            }
        }

        public async Task<IEnumerable<MetricCounter>> GetCountersAsync()
        {
            TableServiceClient tableServiceClient = new TableServiceClient(_connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(_tableName);
            await tableClient.CreateIfNotExistsAsync();

            var results = tableClient.Query<MetricCounter>()
                .ToArray();
            return results;
        }

        private void ChangeCounter(string counterId, int change, int? overrideValue = null)
        {
            var partitionKey = "ABC";
            TableServiceClient tableServiceClient = new TableServiceClient(_connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(_tableName);
            tableClient.CreateIfNotExists();
            var entity =  tableClient.GetEntityIfExists<MetricCounter>(partitionKey, counterId);
            MetricCounter counter;
            if (entity.HasValue)
            {
                counter = entity.Value!;
                counter.Count = overrideValue ?? (counter.Count + change);
                var updateResult = tableClient.UpdateEntity<MetricCounter>(counter, Azure.ETag.All);
            }
            else
            {
                counter = new MetricCounter()
                {
                    PartitionKey = partitionKey,
                    RowKey = counterId,
                    Count = overrideValue ?? 1
                };
                tableClient.AddEntity(counter);
            }
        }
    }
}
