using ImageUpload.Data; // Make sure this matches your namespace
using ImageUpload.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Add DbContext
var connectionString = builder.Configuration.GetConnectionString("dbcs");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'dbcs' not found in configuration.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Add Swagger with support for IFormFile and form data
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Basic API info
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ImageUpload API",
        Description = "API for uploading images via base64 or file upload",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@example.com"
        }
    });

    // Fix for IFormFile: Add operation filter to handle multipart/form-data
    options.OperationFilter<FormDataOperationFilter>();

    // Optional: Include XML comments (if you have them enabled)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImageUpload API v1");
        // Optional: Serve Swagger at root: https://localhost:7228/
        // c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImageUpload API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();