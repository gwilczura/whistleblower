using Azure;
using Azure.Data.Tables;

namespace UploadImageForProcessing.Models
{
    public class MetricCounter : ITableEntity
    {
        public int Count { get; set; } = 0;
        public string PartitionKey { get; set; } = default!;

        public string RowKey { get; set; } = default!;

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
}
