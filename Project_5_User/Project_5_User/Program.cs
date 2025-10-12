using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Writers;
using System.Reflection.Metadata.Ecma335;

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




//verify the authentication with authentication module
bool verifyAuth(String? auth_header)
{
    //make sure string is not empty
    if (string.IsNullOrEmpty(auth_header))
        return false;


    //send to authentication module for verification


    return true;

}




//allows the user to request a ride and receive an offer. offer must be confirmed
// /api/requestRide
////input: POST { "userID": "u12345", "pickupLocation": "Conestoga College, Waterloo, ON", "destinationAddress": "Conestoga Mall, Waterloo, ON" }
////output: { "rideID" : "01242", "distance" : "14.58", "durationMinutes" : "1.86", "fare" : "29.04" }
app.MapPost("/requestRide", (RideRequest request, HttpContext context) =>
{
    //authenticate
    //verify the user's authentication token
    var authHeader = context.Request.Headers["Authorization"].ToString();

    //verifyAuth(authHeader);


    //check for an available driver


    //get estimate from /api/estimate 
    ///input : { "pickupAddress": "Conestoga College, Waterloo, ON", "destinationAddress": "Conestoga Mall, Waterloo, ON" }
    ///output : { "distanceKm": 14.58, "durationMinutes": 18.6, "fare": 29.04, "polyline": "..." }
    
    //placeholder info
    var navOutput = new
    {
        distanceKm = 14.58,
        durationMinutes = 18.6,
        fare = 29.04,
        polyline = "..."
    };

    //generate ride id entry with userid, driverid, distance, fare, duration, and time of entry and status = unconfirmed
    int rideID = 1234521;


    //return ride offer for confirmation

    var rideOffer = new
    {
        rideID = rideID,
        distance = navOutput.distanceKm,
        duration = navOutput.durationMinutes,
        fare = navOutput.fare
    };

    return Results.Json(rideOffer);

})
.WithName("requestRide")
.WithOpenApi();


//confirms the ride for the user, activates payment, and dispatches a driver
// /api/confirmRide
////input: { "userID" : "u12345", "rideID" : "01242", "confirm_ride" : "true" }
////output: { "rideID" : "12345", "driver_name" : "John", "ETA" : "17:40", "payment_successful" : "true" }

app.MapPost("/confirmRide", (RideConfirmation confirmation, HttpContext context) =>
{
    //authenticate
    var authHeader = context.Request.Headers["Authorization"].ToString();
    //verifyAuth(authHeader);


    //activate payment with payment module

    //update ride table entry

    //dispatch driver with driver module

    //update ridetable entry

    //return details of ride
    var rideDetails = new
    {
        rideID = confirmation.rideID,
        driverName = "John",
        vehicle = "Biege Chevy Malibu",
        licensePlate = "KJVM 719"

    };

    return Results.Json(rideDetails);

})
.WithName("confirmRide")
.WithOpenApi();

//returns the location of the user's driver
// /api/driverLocation
////input: driverlocation?userID=12345&rideID=12312421
////output: {  "longitude" : "12.1243", "latitude" : "14.2323" }
app.MapGet("/driverLocation", (HttpContext context, int userID, int rideID) =>
{

    //verify the user's authentication token
    var authHeader = context.Request.Headers["Authorization"].ToString();

    //verifyAuth(authHeader);
  

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
        latitude = navigationOutput.latitude,
        longitude = navigationOutput.longitude
    };

    return Results.Json(location);
})
.WithName("GetDriverLocation")
.WithOpenApi();

//finish the ride and rate the driver
// /finishRide
////input: { UserID = 123324, RideID = 32492359, RideCompleted = true, Rating = 5 }
////output: 202 accepted
app.MapPost("/finishRide", (finishRide request, HttpContext context) =>
{
    //authenticate
    //verify the user's authentication token
    var authHeader = context.Request.Headers["Authorization"].ToString();

    //verifyAuth(authHeader);

    //make sure rating is between 1 - 5
    if (request.rating < 1 || request.rating > 5)
        return Results.BadRequest(new {error = "rating must be between 1 and 5"});


    //update table for end time and driver rating (likely sending rating to the driver module)


    //return 202 ok
    return Results.Accepted();
})
.WithName("finishRide")
.WithOpenApi();

app.Run();

public record RideRequest(
    int userID,
    string pickupLocation,
    string destinationAddress
);

public record RideConfirmation(
    int userID,
    int rideID,
    bool confirmRide
);

public record finishRide(
    int userID,
    int rideID,
    bool rideCompleted,
    int rating
);

