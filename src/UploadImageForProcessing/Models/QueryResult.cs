namespace UploadImageForProcessing.Models
{
    public class QueryResult : MessageResult
    {
        public IEnumerable<UploadResult> Data { get; set; } = new List<UploadResult>();
    }
}
