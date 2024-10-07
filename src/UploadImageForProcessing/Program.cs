using UploadImageForProcessing.Analyze;
using UploadImageForProcessing.Authorization;
using UploadImageForProcessing.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
builder.Services.AddSingleton<Analyzer>();
builder.Services.AddSingleton<TableRepository>();
builder.Services.AddSingleton<CountersRepository>(); 
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
