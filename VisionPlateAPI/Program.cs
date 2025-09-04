

using Microsoft.AspNetCore.Http.Features;
using VisionPlateAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Optional: Configure large file uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseStaticFiles(); // Enables serving static files like index.html from wwwroot


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

#region Auto-redirect root to index.html
//Changed to use launchSettings.json to set the launchUrl
// Redirect root to index.html in development mode
//app.MapGet("/", async context =>
//{
//#if DEBUG
//    context.Response.Redirect("/index.html");
//#else
//    await context.Response.WriteAsync("API is running...");
//#endif
//});

#endregion

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/GetRatingPlateInfo", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest("Invalid content type. Must be multipart/form-data.");

    var form = await request.ReadFormAsync();
    var file = form.Files.GetFile("image");

    if (file == null || file.Length == 0)
        return Results.BadRequest("No image uploaded.");

    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

    if (!allowedExtensions.Contains(extension))
        return Results.BadRequest("Unsupported file format.");

    // TODO: Process the image and extract RatingPlate info

    //for demo purposes, we'll return a dummy RatingPlate based on filename
    var brand = file.FileName switch
    {
        var name when name.Contains("Trane", StringComparison.OrdinalIgnoreCase) => "Trane",
        var name when name.Contains("Carrier", StringComparison.OrdinalIgnoreCase) => "Carrier",
        var name when name.Contains("Lennox", StringComparison.OrdinalIgnoreCase) => "Lennox",
        _ => "Unknown"
    };
    var ratingPlate = new RatingPlate
    {

        Manufacturer = brand,
        Model = "ABC",
        Serial = "123"
    };


    //var ratingPlate = await service.ExtractRatingPlateInfoAsync(file);
    return Results.Ok(ratingPlate);
})
.Accepts<IFormFile>("multipart/form-data")
.Produces<RatingPlate>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
