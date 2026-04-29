using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Mimir.backend.postgres;
using Mimir.Server.Postgres;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

var api = app.MapGroup("/api");
app.MapPost("api/mimirpostgreslogin", async (string userAndPass) =>
{
    Console.WriteLine("Getting Mimir Postgres Log In");
    Console.WriteLine("Got this User and Pass: " + userAndPass);
    string[] _postGresAPICall = await PostgresManager.PostgresAPICall("login", userAndPass);
    Console.WriteLine(_postGresAPICall[0].ToString() + " " + _postGresAPICall[1].ToString());
    var uidToken = ( _postGresAPICall[0], _postGresAPICall[1] );
    return _postGresAPICall;

}).WithName("GetPostgresLogin");

app.MapPost("api/mimirpostgrescreateuser", async (string userAndPass) =>
{
    Console.WriteLine("Getting Mimir Postgres Create User");
    Console.WriteLine("Got this User and Pass: " + userAndPass);
    string[] _postGresAPICall = await PostgresManager.PostgresAPICall("createuser", userAndPass);
    Console.WriteLine(_postGresAPICall[0].ToString() + " " + _postGresAPICall[1].ToString());
    var uidToken = (_postGresAPICall[0], _postGresAPICall[1]);
    return _postGresAPICall;

}).WithName("GetPostgresCreateUser");

app.MapPost("api/mimirpostgresgetposts", (string arguments) =>
{

    return "";
}).WithName("GetPostgresGetPosts");

app.MapPost("api/mimirpostgresupdateaccount", async (string UIDTokenChangeText) =>
{
    // { UID, Token, Wanted Change, Text }
    string[] changeTextArray = UIDTokenChangeText.Split(':', 3);
    bool validToken = CheckToken.CheckUserToken(changeTextArray[1], Int32.Parse(changeTextArray[0]));
    if (!validToken)
    {
        return false;
    }
    await PostgresManager.PostgresAPICall(changeTextArray[2], changeTextArray[3]);
    return true;
}).WithName("GetPostgresUpdateAccount");

app.MapPost("api/mimirpostgresgetuseraccount", async (string uidAndToken) =>
{
    // Promised array { tokenValidBool, username, pfp, profiledesc, followersamt, followingamt }  
    string[] uidTokenArray = uidAndToken.Split(':');
    bool validToken = CheckToken.CheckUserToken(uidTokenArray[1], Int32.Parse(uidTokenArray[0]));
    if (!validToken)
    {
        Console.WriteLine("Wrong Token to Get User Account");
        string[] invalidToken = { "false", "", "", "", "", ""};
        return invalidToken;
    }
    string[] userAccountArray = await PostgresManager.PostgresAPICall("getaccountdata", "", "", "", int.Parse(uidTokenArray[0]));
    return userAccountArray;
}).WithName("GetPostgresGetUserAccount");

app.MapPost("api/mimirpostgresgetforeignaccount", (string arguments) =>
{

    return "";
}).WithName("GetPostgresGetForeignAccount");

app.MapPost("api/mimiruploadfile", (FileStream fileStream) =>
{

    return "";
}).WithName("GetUploadFile");

api.MapGet("weatherforecast", () =>
{
    Console.WriteLine("Got Weather Forecast");
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
.WithName("GetWeatherForecast");

api.MapPost("api/mimirchecktoken", (string uidToken) =>
{
    string[] uidTokenArray = uidToken.Split(':');
    bool validToken = CheckToken.CheckUserToken(uidTokenArray[1], Int32.Parse(uidTokenArray[0]));
    if (!validToken)
    {
        return false;
    }
    return true;
}).WithName("CheckToken");

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
};
