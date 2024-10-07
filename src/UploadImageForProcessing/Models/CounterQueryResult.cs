namespace UploadImageForProcessing.Models
{
    public class CounterQueryResult : MessageResult
    {
        public IEnumerable<MetricCounter> Data { get; set; } = new List<MetricCounter>();
    }
}
