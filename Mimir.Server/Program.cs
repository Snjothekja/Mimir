using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Mimir.backend.postgres;
using Mimir.Server.Postgres;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

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

// Post apis

app.MapPost("api/mimirnewpost", async ([FromForm] string token, [FromForm] string uid, [FromForm] string postText, IFormFile? image) =>
{
    Console.WriteLine("Adding new post");
    bool validToken = CheckToken.CheckUserToken(token, int.Parse(uid));
    if (!validToken)
    {
        return validToken;
    }
    await PostgresManager.PostgresAPICall("post", postText, "", "", int.Parse(uid), 0, image);

    return validToken;
}).WithName("GetPostgresNewPost").DisableAntiforgery();

app.MapPost("api/mimirpostgresgetposts", (string arguments) =>
{

    return "";
}).WithName("GetPostgresGetPosts");

app.MapPost("api/mimirpostgresgetuseraccount", async (string uidAndToken) =>
{
    // Promised array { tokenValidBool, username, pfp, profiledesc, followersamt, followingamt, profilebanner }  
    string[] uidTokenArray = uidAndToken.Split(':');
    bool validToken = CheckToken.CheckUserToken(uidTokenArray[1], Int32.Parse(uidTokenArray[0]));
    if (!validToken)
    {
        Console.WriteLine("Wrong Token to Get User Account");
        string[] invalidToken = { "false", "", "", "", "", "", ""};
        return invalidToken;
    }
    string[] userAccountArray = await PostgresManager.PostgresAPICall("getaccountdata", "", "", "", int.Parse(uidTokenArray[0]));
    return userAccountArray;
}).WithName("GetPostgresGetUserAccount");

app.MapPost("api/mimirpostgresgetforeignaccount", (string arguments) =>
{

    return "";
}).WithName("GetPostgresGetForeignAccount");

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



// Updating Account Information

app.MapPost("api/mimirpostgresupdatedesc", async (string UIDTokenChangeText) =>
{
    // { UID, Token, Wanted Change, Text }
    Console.WriteLine("Updating Account");
    string[] changeTextArray = UIDTokenChangeText.Split(':');

    Console.WriteLine("UID: " + changeTextArray[0] + " Token: " + changeTextArray[1] + " Change: " + changeTextArray[2] + " Text: " + changeTextArray[3]);
    bool validToken = CheckToken.CheckUserToken(changeTextArray[1], Int32.Parse(changeTextArray[0]));
    if (!validToken)
    {
        return false;
    }
    await PostgresManager.PostgresAPICall(changeTextArray[2], changeTextArray[3], changeTextArray[0]);
    return true;
}).WithName("GetPostgresUpdateDesc");

app.MapPost("api/mimirupdatepfp", async ([FromForm] string token, [FromForm] string uid, IFormFile image) =>
{
    Console.WriteLine("Updating PFP: " + image.FileName);
    bool validToken = CheckToken.CheckUserToken(token, Int32.Parse(uid));
    if (!validToken)
    {
        return;
    }
    await PostgresManager.PostgresAPICall("pfp", uid, "", "", 0, 0, image);

}).WithName("UpdatePFP").DisableAntiforgery();

app.MapPost("api/mimirupdatebanner", async ([FromForm] string token, [FromForm] string uid, IFormFile image) =>
{ 
    Console.WriteLine("Updating PFP: " + image.FileName);
    bool validToken = CheckToken.CheckUserToken(token, Int32.Parse(uid));
    if (!validToken)
    {
        return;
    }
    await PostgresManager.PostgresAPICall("banner", uid, "", "", 0, 0, image);
}).WithName("UpdateBanner").DisableAntiforgery();

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
};
