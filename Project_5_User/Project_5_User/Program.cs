using Microsoft.OpenApi.Writers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


//returns the location of the user's driver
//ie /driverlocation?driver=12345
app.MapGet("/driverLocation", (HttpContext context, int driverID) =>
{

    //verify the user's authentication token
    var authHeader = context.Request.Headers["Authorization"].ToString();

    //make sure string is not empty
    if (string.IsNullOrEmpty(authHeader))
        return Results.Unauthorized();

    //send auth token to auth module for verification. return unauthorized if invalid


    //request driver location from the navigation or driver module
    var navigationOutput = new
    {
        latitude = 12.4112,
        longitude = 14.2212
    };


    //return driver location
    var location = new
    {
        driverID = driverID,
        latitude = navigationOutput.latitude,
        longitude = navigationOutput.longitude
    };

    return Results.Json(location);
})
.WithName("GetDriverLocation")
.WithOpenApi()
.RequireAuthorization();
    
    

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
