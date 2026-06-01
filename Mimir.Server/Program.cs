using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Mimir.backend.postgres;
using Mimir.Server.Postgres;
using Mimir.Server.TestScripts;
using Npgsql;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Mimir.backend.postgres.PostgresManager;
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

app.MapPost("api/mimirpostgresgetposts", async (string arguments) =>
{
    string[] argumentArray = arguments.Split('|');
    Console.WriteLine(argumentArray[0] + "    " + DateTime.Parse(argumentArray[1]));
    PostgresManager.PostStruct[] posts = await PostgresManager.GetPosts(int.Parse(argumentArray[0]), DateTime.Parse(argumentArray[1]));
    return posts;
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

app.MapPost("api/mimirpostgresgetforeignaccount", async (string arguments) =>
{
    PostgresManager.ForeignAccountStruct[] foreignAccountInfo = await PostgresManager.GetForeignAccounts(arguments);
    return foreignAccountInfo;
}).WithName("GetPostgresGetForeignAccount");

app.MapPost("api/mimirchecktoken", (string uidToken) =>
{
    string[] uidTokenArray = uidToken.Split(':');
    bool validToken = CheckToken.CheckUserToken(uidTokenArray[1], Int32.Parse(uidTokenArray[0]));
    if (!validToken)
    {
        return @"{""check"":""false""}";
    }
    return @"{""check"":""true""}";
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


// Reposts, Likes, and Comments API's

app.MapPost("api/mimirlikepost", async (string tokenuidpostid) =>
{
    string[] tokenUIDPostidArray = tokenuidpostid.Split(":");
    bool validToken = CheckToken.CheckUserToken(tokenUIDPostidArray[0], Int32.Parse(tokenUIDPostidArray[1]));
    if (!validToken)
    {
        return;
    }
    PostgresManager.AddLike(int.Parse(tokenUIDPostidArray[2]), int.Parse(tokenUIDPostidArray[1]));

}).WithName("LikePost");

app.MapPost("api/mimiraddcomment", async ([FromForm] string uid, [FromForm] string token, [FromForm] string postID, [FromForm] string commentID, [FromForm] string commentText) =>
{
    //bool validToken = CheckToken.CheckUserToken(token, int.Parse(uid));
    //if (!validToken)
    //{
    //    return;
    //}
    PostgresManager.AddComment(uid, commentText, postID, commentID);
}).WithName("AddComment").DisableAntiforgery();

app.MapPost("api/mimirgetcomments", async (int postID) =>
{
    var values = await PostgresManager.GetComments(postID);
    try
    {
        Console.WriteLine(values[0].commentText);
    }
    catch { Console.WriteLine("Its null"); }
    
    return values;
}).WithName("GetComments").DisableAntiforgery();

// Test Endpoints Remove if released

app.MapGet("api/mimirtestscripts", async () =>
{
    CommentJSON.TestCommentJSONDeserialize();
}).WithName("TestScripts");

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();

