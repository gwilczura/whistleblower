namespace UploadImageForProcessing.Authorization
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        string ApiKey;
        public ApiKeyValidator(IConfiguration configuration)
        {
            ApiKey = configuration["Config:ApiKey"];
        }

        public bool IsValid(string apiKey)
        {
            return apiKey?.ToLower() == ApiKey.ToLower();
        }
    }
}
