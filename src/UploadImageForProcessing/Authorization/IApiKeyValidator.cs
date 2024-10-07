namespace UploadImageForProcessing.Authorization
{
    public interface IApiKeyValidator
    {
        bool IsValid(string apiKey);
    }
}
