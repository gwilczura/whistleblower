using System.Text.Json.Serialization;

namespace CaptureImage.Models
{
    public class UploadResult
    {
        public string Message { get; set; } = string.Empty;
        public string? Alert { get; set; } = string.Empty;
        public bool HasAlert { get; set; } = false;
        public bool AlertResolved { get; set; } = false;
        public string? Caption { get; set; } = string.Empty;
        public float? CaptionConfidence { get; set; }
        public string Filename { get; set; } = string.Empty;
        public string SasUrl { get; set; } = string.Empty;

        public bool Processed { get; set; } = false;

        public long FileId { get; set; } = default!;

        public string? PartitionKey { get; set; } = default!;

        public string? RowKey { get; set; } = default!;

        public string? ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
}
