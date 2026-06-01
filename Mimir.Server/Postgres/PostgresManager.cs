using Mimir.Server.FileStorage;
using Mimir.Server.Postgres;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace Mimir.backend.postgres
{
    internal class PostgresManager
    {
        
        public struct PostStruct
        {
            public int postid { get; set; }
            public int posterUID { get; set; }
            public string? postText { get; set; }
            public string? images { get; set; }
            public int likeAmt { get; set; }
            public int repostAmt { get; set; }
            public int commentAmt { get; set; }
            public DateTime datePosted { get; set;  }
            public int originalPostID { get; set; }

        }

        public struct ForeignAccountStruct
        {
            public string username { get; set; }
            public string pfp { get; set; }
        }

        public struct Comments
        {
            public string commentText { get; set; }
            public int uid { get; set; }
            public int commentOnCommentID { get; set; }
            public int commentID { get; set; }
            [JsonExtensionData]
            public Dictionary<string, JsonElement>? ExtensionData { get; set; }
        }

        static public async Task Main()
        {
            await UserActions();
        }

        static private string GetUserInput()
        {
            return Console.ReadLine();
        }

        static async Task UserActions()
        {
            Console.WriteLine("What action you want to use? ('Help' for options)");
            string _userInput = "test";
            _userInput = GetUserInput();

            switch (_userInput.ToLower())
            {
                case "esc":
                    return;
                case "help":
                    Console.WriteLine("'esc' to leave, 'mu' to manage users, 'login' to log in, 'create' to create a user");
                    break;
                case "mu":
                    await PostgresManageUser.ManageUser(); ;
                    break;
                case "login":
                    //var _loginInfo = await PostgresUserLogIn.UserLogin();
                    //loggedInUID = _loginInfo.Item1;
                    //loginToken = _loginInfo.Item2;
                    break;
                case "create":
                    Console.WriteLine("Username: ");
                    string _username = GetUserInput();
                    Console.WriteLine("Password: ");
                    string _password = GetUserInput();
                    bool _created = await PostgresCreateUser.CreateUser(_username, _password);
                    if (_created == true)
                    {
                        Console.WriteLine("Created User: " + _username);
                    }
                    else
                    {
                        Console.WriteLine("Username Taken.");
                    }
                    break;

            }

            await UserActions();
        }

        static public async Task<string[]> PostgresAPICall(string request, string inputString = "", string inputString2 = "", 
            string inputString3 = "", int inputInt = 0, int inputInt2 = 0, IFormFile file = null)
        {
            switch (request)
            {
                case "login":
                    var tokenUID = PostgresUserLogIn.UserLogin(inputString);
                    //string tokenUIDJson = JsonSerializer.Serialize();
                    string[] tokenUIDStringArray = { tokenUID.Result.Item1.ToString(), tokenUID.Result.Item2};
                    return tokenUIDStringArray;

                case "createuser":
                    string[] userPass = inputString.Split(':');
                    await PostgresCreateUser.CreateUser(userPass[0], userPass[1]);

                    var tokenUIDCreate = await PostgresUserLogIn.UserLogin(inputString);
                    //string tokenUIDJson = JsonSerializer.Serialize();
                    string[] tokenUIDStringArrayCreate = { tokenUIDCreate.Item1.ToString(), tokenUIDCreate.Item2 };
                    return tokenUIDStringArrayCreate;

                case "getaccountdata":
                    var accountData = await PostgresGetUserAccount.GetUserAccountDetails(inputInt);
                    Console.WriteLine("Getting User Account Data");
                    string[] accountDataArray = { "true", accountData[0].ToString(), accountData[1].ToString(), accountData[2].ToString(), accountData[3].ToString(), accountData[4].ToString(), accountData[5].ToString() };
                    return accountDataArray;

                case "desc":
                    await PostgresUpdateUserDesc.UpdateUserAccount(request, inputString, inputString2);
                    return new string[1];

                case "pfp":
                    string pathpfp = ImageUpload.UploadImage(file, "pfp");
                    await PostgresUpdateUserPFP.UpdatePFP(int.Parse(inputString), pathpfp);
                    return new string[1];

                case "banner":
                    string pathbanner = ImageUpload.UploadImage(file, "banner");
                    PostgresUpdateUserBanner.UpdateBanner(int.Parse(inputString), pathbanner);
                    return new string[1];

                case "post":
                    string imagePath = "";
                    if (file != null)
                    {
                        imagePath = ImageUpload.UploadImage(file, "postimage");
                    }      
                    await PostgresAddPost.AddPost(inputString, inputInt, imagePath);
                    return new string[1];
            }

            return null;
        }

        static internal async Task<PostStruct[]> GetPosts(int uid, DateTime dateTime)
        {
            PostStruct[] posts = await PostgresGetPosts.GetPosts(uid, dateTime);
            return posts;
        }

        static internal async Task<ForeignAccountStruct[]> GetForeignAccounts(string inputString)
        {
            Console.WriteLine(inputString);
                string[] wantedUserUIDs = inputString.Split(":");
                ForeignAccountStruct[] foreignAccountData = await PostgresGetForiegnAccount.GetForeignAccountData(wantedUserUIDs);
                return foreignAccountData;
        }

        static internal async void AddLike(int puid, int uid)
        {
            await PostgresAddLike.AddLike(puid, uid);
        }

        static internal async void AddComment(string uid, string commentText, string postID, string commentOnCommentID)
        {
            await PostgresAddComment.AddComment(int.Parse(uid), commentText, int.Parse(postID), int.Parse(commentOnCommentID));
        }

        static internal async Task<List<PostgresManager.Comments>> GetComments(int postID)
        {
            return await PostgresGetComments.GetComments(postID);
        }

        static internal async void AddRepost()
        {

        }
    }
}
