// See https://aka.ms/new-console-template for more information
using CaptureImage.Models;
using Microsoft.Extensions.Configuration;
using OpenCvSharp;
using System.Text.Json;

Console.WriteLine("Video capture starting ...");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var apiKey = configuration["Config:ApiKey"];
var targetUrl = configuration["Config:UploadEndpoint"];
var cameraIndex = configuration["Config:CameraIndex"];
var dimensions = configuration["Config:ImageDimensions"];

var typedCameraIndex = int.Parse(cameraIndex);
var resolutionParts = dimensions.Split("x");
var width = int.Parse(resolutionParts[0]);
var height = int.Parse(resolutionParts[1]);
VideoCapture capture;
Mat frame;

capture = new VideoCapture(typedCameraIndex, VideoCaptureAPIs.FFMPEG);
capture.Open(typedCameraIndex);
capture.FrameWidth = width;
capture.FrameHeight = height;

var client = new HttpClient();


Console.WriteLine("Camera ready");
Console.WriteLine($"Attempting to capture {width}x{height} from device nr:{typedCameraIndex}");

int counter = 1;

while (capture.IsOpened())
{
    if (capture.Grab())
    {
        frame = capture.RetrieveMat();
        var byteArray = frame.ToBytes();

        var request = new HttpRequestMessage(HttpMethod.Post, targetUrl);
        request.Headers.Add("X-API-Key", apiKey);
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(byteArray), "fileData", "file.jpg");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var uploadResult = JsonSerializer.Deserialize<UploadResult>(result, options);
        Console.WriteLine($"{counter++:0000}: {uploadResult?.Filename} - {uploadResult?.Caption} - {uploadResult?.CaptionConfidence}");
    }
    else
    {
        Console.WriteLine($"Failed to capture...");
    }
    await Task.Delay(3000);
}
