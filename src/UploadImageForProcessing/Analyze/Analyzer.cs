using Azure;
using Azure.AI.Vision.ImageAnalysis;
using System.Text;

namespace UploadImageForProcessing.Analyze
{
    public class Analyzer
    {
        private readonly IConfiguration configuration;

        public Analyzer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public (string? Caption,float? Confidence) AnalyzeImage(string imageUri)
        {
            string endpoint = configuration["Config:AnalyzeEndpoint"];
            string key = configuration["Config:AnalyzeKey"];

            ImageAnalysisClient client = new ImageAnalysisClient(
                new Uri(endpoint),
                new AzureKeyCredential(key));

            ImageAnalysisResult result = client.Analyze(
                new Uri(imageUri),
                VisualFeatures.Caption | VisualFeatures.Read,
                new ImageAnalysisOptions { GenderNeutralCaption = true });

            return (result?.Caption?.Text, result?.Caption.Confidence);
        }
    }
}
